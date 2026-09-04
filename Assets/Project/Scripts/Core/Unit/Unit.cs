
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Aegis.Core
{
    public class Unit : WorldEntity, IFactionMember, IDamageable
    {
        public UnitStats Stats { get; }
        public UnitWeaponry Weaponry { get; }
        private readonly UnitCommonConfig _common;

        private Health _health;
        public Health Health {
            get {
                if (_health == null)
                    Debug.LogWarning($"[Unit] Спроба звернутись до Health до того, як він створений ({EntityType}). Викличте SetConfig() раніше.");
                return _health;
            }
            private set => _health = value;
        }
        public int FactionId { get; private set; }
        public UnitType EntityType { get; set; }
        public UnitConfig Config;
        public float MaxHealth => _common.BaseHealth + Stats.GetStat(StatType.Strength) * _common.HealthPerStrength;
        public float MoveSpeed => _common.MoveSpeed; // поки без формули від Speed — про це наступним кроком
        public float SearchRadius => _common.SearchRadius;
        public float ChaseRadius => _common.ChaseRadius;        public float AttackDamage => Weaponry.Damage > 0.01f ? Weaponry.Damage : _common.UnarmedDamage;

        public bool CanShoot => Weaponry.HasBow;
        public float AttackRange => Weaponry.GetAttackRange();
        public float WalkAnimationSpeedMultiplier => _common.WalkAnimationSpeedMultiplier;

        public float AttackTime => Weaponry.AttackTime > 0.01f ? Weaponry.AttackTime : _common.UnarmedCooldown;
        public float AttackEventTime => Weaponry.AttackEventTime > 0.01f ? Weaponry.AttackEventTime : 0.5f;

        public Vector3 FixedPosition { get; set; }
        private WorldEntity _closestTarget;
        public WorldEntity AttackTarget;
        public WorldEntity ChaseTarget;

        public bool SelectedByPlayer { get; private set; }

        public UnitControlMode ControlMode { get; private set; } = UnitControlMode.Indirect;

        public event Action<bool> WasSelectedByPlayer;
        public event Action ExecutedStopMovement;
        public event Action<float, float> HealthChanged;
        public event Action<Vector3> WalkTo;
        public event Action<Vector3> ChaseTo;
        public event Action Died;
        public event Action<UnitActionEvent> ActionPerformed;
        public event Action<Vector3> ProjectileLaunched;
        public event Action<UnitControlMode> ControlModeChanged;
        public event Action<Vector3> DirectMoveRequested;

        public UnitStateMachine StateMachine { get; private set; }

        public WorldEntity ClosestTarget {
            get => _closestTarget;
            set {
                if (_closestTarget != value) {
                    _closestTarget = value;
                    // LookedAt?.Invoke(_closestTarget);
                }
            }
        }

        public Unit(Vector3 position, int factionId, UnitType type, UnitConfig config, UnitCommonConfig common)
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

            Health = new Health(MaxHealth);
            Health.Changed += (current, max) => HealthChanged?.Invoke(current, max);
            Health.Depleted += OnHealthDepleted;

            StateMachine = new UnitStateMachine(this);
            Position = position;
            FixedPosition = Position;
        }

        private void OnHealthDepleted()
        {
            PerformDeath();
        }
        public void TakeDamage(float amount)
        {
            if (!Health.IsAlive) return;

            Health.TakeDamage(amount);
        }
        public void Heal(float amount)
        {
            Health.Heal(amount);
        }
        public void MovementComplete(Vector3 position)
        {
            Position = position;

            StateMachine.SetState(UnitState.Idle);
        }
        public void Select(bool selected)
        {
            if (!Health.IsAlive) return;

            SelectedByPlayer = selected;
            WasSelectedByPlayer?.Invoke(selected);
        }
        public void PerformWalk(Vector3 destination)
        {
            if (!Health.IsAlive) return;

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
            // Debug.Log("Performing chase!");

            ChaseTo?.Invoke(entity.Position);
        }
        public void SetControlMode(UnitControlMode mode)
        {
            if (ControlMode == mode) return;

            if (mode == UnitControlMode.Direct)
                StopMovement();

            ControlMode = mode;
            ControlModeChanged?.Invoke(mode);
        }
        /// <summary>
        /// Прямий рух гравця (Odyssey-режим). worldDirection — вже
        /// нормалізований напрямок у світових координатах (без Y),
        /// порахований у View з урахуванням орієнтації камери.
        /// </summary>
        public void PerformDirectMove(Vector3 worldDirection)
        {
            if (!Health.IsAlive) return;
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
        public void PerformAttackImpact(WorldEntity target)
        {
            if (target == null) return;

            if (Weaponry.BowActive)
                PerformProjectileLaunch(target);
            else {
                ApplyDamage(target, Weaponry.Damage);
            }
        }
        public void ApplyProjectileDamage(WorldEntity target)
        {
            ApplyDamage(target, Weaponry.Damage);
        }
        private void ApplyDamage(WorldEntity target, float damage)
        {
            if (target != null && target is Unit unit) {
                unit.TakeDamage(damage);
            }
        }
        public void PerformDeath()
        {
            StateMachine.SetState(UnitState.Dead);
            SelectedByPlayer = false;
            WasSelectedByPlayer?.Invoke(false);
            Died?.Invoke();
        }
        public void UpdateInteractions(IReadOnlyList<WorldEntity> allEntities)
        {
            if (!Health.IsAlive) return;

            WorldEntity closest = null;
            float closestSqrDist = SearchRadius * SearchRadius;

            foreach (var e in allEntities) {
                if (e is IFactionMember fmEntity) {
                    if (fmEntity == this || fmEntity.FactionId == FactionId)
                        continue;
                    else if (fmEntity is IDamageable damageable && !damageable.Health.IsAlive)
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
        /// <summary>
        /// Перевіряє, чи немає перешкод (стін, рельєфу тощо) між очима цього
        /// юніта і ціллю. Physics.Linecast — тому й дорого викликати щокадрово,
        /// але UpdateInteractions і так тіктиться раз на _interationsIntervalTime
        /// (WorldUpdater), тож це прийнятно без додаткового троттлінгу.
        /// </summary>
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