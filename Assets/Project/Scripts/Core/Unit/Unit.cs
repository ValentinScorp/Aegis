
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Aegis.Core
{
    public class Unit : WorldEntity, IFactionMember, IDamageable
    {
        // ─── Fields ───────────────────────────────────────────
        private readonly UnitCommonConfig _common;
        private BodyHealth _bodyHealth;
        private WorldEntity _closestTarget;

        // ─── Identity & config ────────────────────────────────
        public FactionId FactionId { get; private set; }
        public UnitType EntityType { get; private set; }
        public UnitConfig Config { get; private set; }

        // ─── Core systems ─────────────────────────────────────
        public UnitStats Stats { get; }
        public UnitWeaponry Weaponry { get; }
        public BodyHealth BodyHealth => _bodyHealth;
        public bool IsAlive => BodyHealth.IsAlive;
        public UnitStateMachine StateMachine { get; private set; }

        // ─── Derived combat / movement stats ──────────────────
        // public float MaxHealth => _common.BaseHealth + Stats.GetStat(StatType.Strength) * _common.HealthPerStrength;
        public float MoveSpeed => _common.MoveSpeed; // поки без формули від Speed — про це наступним кроком
        public float SearchRadius => _common.SearchRadius;
        public float ChaseRadius => _common.ChaseRadius; 
        public float AttackDamage => Weaponry.Damage > 0.01f ? Weaponry.Damage : _common.UnarmedDamage;
        public float AttackRange => Weaponry.GetAttackRange();
        public bool CanShoot => Weaponry.HasBow;
        public float WalkAnimationSpeedMultiplier => _common.WalkAnimationSpeedMultiplier;
        public float AttackTime => Weaponry.AttackTime > 0.01f ? Weaponry.AttackTime : _common.UnarmedCooldown;
        public float AttackEventTime => Weaponry.AttackEventTime > 0.01f ? Weaponry.AttackEventTime : 0.5f;

        // ─── Runtime state ────────────────────────────────────
        public Vector3 FixedPosition { get; set; }
        public WorldEntity AttackTarget { get; set; }
        public WorldEntity ChaseTarget { get; set; }
        public WorldEntity ClosestTarget { get; set; }
        public bool SelectedByPlayer { get; private set; }
        public UnitControlMode ControlMode { get; private set; } = UnitControlMode.Indirect;

        // ─── Events ───────────────────────────────────────────
        public event Action<BodyPartId, float, float> BodyPartHealthChanged;
        public event Action<float, float> HealthChanged;
        public event Action<bool> WasSelectedByPlayer;
        public event Action ExecutedStopMovement;
        public event Action<Vector3> WalkTo;
        public event Action<Vector3> ChaseTo;
        public event Action Died;
        public event Action<UnitActionEvent> ActionPerformed;
        public event Action<Vector3> ProjectileLaunched;
        public event Action<UnitControlMode> ControlModeChanged;
        public event Action<Vector3> DirectMoveRequested;

        public Unit(Vector3 position, FactionId factionId, UnitType type, UnitConfig config, UnitCommonConfig common)
        {
            FactionId = factionId;
            EntityType = type;
            Config = config;
            _common = common;

            Stats = new UnitStats(config.BaseStrength, config.BaseSpeed, config.BaseSpirit);

            Weaponry = new UnitWeaponry(config.MainHandPrimary,
                                        config.OffHandPrimary,
                                        config.MainHandSecondary,
                                        config.OffHandSecondary);

            _bodyHealth = new BodyHealth(headMax: 50f, torsoMax: 100f, armMax: 60f, legMax: 70f);
            _bodyHealth.InitEvents();
            _bodyHealth.PartChanged += OnBodyPartHealthChanged;
            _bodyHealth.Depleted += OnHealthDepleted;

            StateMachine = new UnitStateMachine(this);
            Position = position;
            FixedPosition = Position;
        }

        // ─── Health / death ───────────────────────────────────

        public void TakeDamage(BodyPartId partId, float amount)
        {
            if (!_bodyHealth.Get(BodyPartId.Head).IsAlive && !_bodyHealth.Get(BodyPartId.Torso).IsAlive)
                return;

            _bodyHealth.TakeDamage(partId, amount);
        }
        public void Heal(BodyPartId partId, float amount)
        {
            BodyHealth.Heal(partId, amount);
        }
        private void OnBodyPartHealthChanged(BodyPartId partId, float cur, float max)
        {
            BodyPartHealthChanged?.Invoke(partId, cur, max);
            HealthChanged(BodyHealth.GetCurrentAllParts(), BodyHealth.GetMaxAllParts());
        }
        private void OnHealthDepleted()
        {
            PerformDeath();
        }
        public void PerformDeath()
        {
            StateMachine.SetState(UnitState.Dead);
            SelectedByPlayer = false;
            WasSelectedByPlayer?.Invoke(false);
            Died?.Invoke();
        }

        // ─── Selection & control ──────────────────────────────
        public void Select(bool selected)
        {
            if (!BodyHealth.IsAlive) return;

            SelectedByPlayer = selected;
            WasSelectedByPlayer?.Invoke(selected);
        }
        public void SetControlMode(UnitControlMode mode)
        {
            if (ControlMode == mode) return;

            if (mode == UnitControlMode.Direct)
                StopMovement();

            ControlMode = mode;
            ControlModeChanged?.Invoke(mode);
        }

        // ─── Movement ─────────────────────────────────────────        
        public void PerformWalk(Vector3 destination)
        {
            if (!BodyHealth.IsAlive) return;

            FixedPosition = destination;

            var walk = StateMachine.GetState<UnitStateWalk>();
            if (walk != null)
                walk.Destination = destination;

            StateMachine.SetState(UnitState.Walk);
            // Debug.Log("Performing walk!");
            WalkTo?.Invoke(destination);
        }
        public void PerformChase(WorldEntity entity)
        {
            ChaseTo?.Invoke(entity.Position);
        }
        public void PerformDirectMove(Vector3 worldDirection)
        {
            if (!BodyHealth.IsAlive) return;
            if (ControlMode != UnitControlMode.Direct) return;

            if (worldDirection.sqrMagnitude > 0.0001f) {
                StateMachine.SetState(UnitState.Walk);
                DirectMoveRequested?.Invoke(worldDirection);
            } else {
                StateMachine.SetState(UnitState.Idle);
            }
        }
        public void StopMovement()
        {
            ExecutedStopMovement?.Invoke();
        }
        public void MovementComplete(Vector3 position)
        {
            Position = position;

            StateMachine.SetState(UnitState.Idle);
        }

        // ─── Combat ───────────────────────────────────────────
        public void PerformAttackAction(WorldEntity target)
        {
            ActionPerformed?.Invoke(new UnitActionEvent(UnitAction.Attack, target.Position));
        }
        public void StopAttackAction() => ActionPerformed?.Invoke(new UnitActionEvent(UnitAction.Idle, Vector3.zero));
        public void PerformProjectileLaunch(WorldEntity target)
        {
            if (AttackTarget == null) return;
            ProjectileLaunched?.Invoke(target.Position);
        }
        public void PerformAttackImpact(WorldEntity target, BodyPartId partId)
        {
            if (target == null) return;

            if (Weaponry.BowActive)
                PerformProjectileLaunch(target);
            else {
                ApplyDamage(target, partId, Weaponry.Damage);
            }
        }
        public void ApplyProjectileDamage(WorldEntity target, BodyPartId partId)
        {
            ApplyDamage(target, partId, Weaponry.Damage);
        }
        private void ApplyDamage(WorldEntity target, BodyPartId partId, float damage)
        {
            if (target != null && target is Unit unit) {
                unit.TakeDamage(partId, damage);
            }
        }

        // ─── AI / interactions ────────────────────────────────
        public void UpdateInteractions(IReadOnlyList<WorldEntity> allEntities)
        {
            if (!BodyHealth.IsAlive) return;

            WorldEntity closest = null;
            float closestSqrDist = SearchRadius * SearchRadius;

            foreach (var e in allEntities) {
                if (e is IFactionMember fmEntity) {
                    if (fmEntity == this || fmEntity.FactionId == FactionId)
                        continue;
                    else if (fmEntity is IDamageable damageable && !damageable.IsAlive)
                        continue;
                } else continue;

                float sqrDist = (e.Position - Position).sqrMagnitude;
                if (sqrDist < closestSqrDist && HasLineOfSight(e)) {
                    closestSqrDist = sqrDist;
                    closest = e;
                }
            }
            ClosestTarget = closest;

            StateMachine.UpdateInteractions(closest);
        }

        private bool HasLineOfSight(WorldEntity target)
        {
            if (_common.ObstacleMask.value == 0) return true; // маска не налаштована — перевірку не робимо

            Vector3 eyeOffset = Vector3.up * _common.EyeHeight;
            Vector3 from = Position + eyeOffset;
            Vector3 to = target.Position + eyeOffset;

            return !Physics.Linecast(from, to, _common.ObstacleMask, QueryTriggerInteraction.Ignore);
        }
        public bool CanAttack(WorldEntity entity)
        {
            if (entity is Unit unit) {
                if (unit.FactionId != FactionId) {
                    float distSqr = (entity.Position - Position).sqrMagnitude;
                    if (distSqr < (AttackRange * AttackRange) && HasLineOfSight(entity)) {
                        return true;
                    }
                }
            }
            return false;
        }

        internal void UpdateActions(float deltaTime)
        {
            StateMachine.UpdateActions(deltaTime);
        }
    }
}