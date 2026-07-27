
using System;
using System.Collections.Generic;
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
        public EntityType EntityType { get; set; }
        public UnitConfig Config;
        public float MaxHealth => _common.BaseHealth + Stats.GetStat(StatType.Strength) * _common.HealthPerStrength;
        public float MoveSpeed => _common.MoveSpeed; // поки без формули від Speed — про це наступним кроком
        public float SearchRadius => _common.SearchRadius;
        public float ChaseRadius => _common.ChaseRadius;
        public float AttackDamage => Weaponry.Active.MainHand != null ? Weaponry.Active.MainHand.Damage : _common.UnarmedDamage;

        public bool CanShoot => Weaponry.HasAnyRanged;
        public float AttackRadius => Weaponry.Active.MainHand != null ? Weaponry.Active.AttackRange : 1.0f;
        public float AttackTime => Weaponry.Active.MainHand != null ? Weaponry.Active.AttackTime : _common.UnarmedDamage;
        public float WalkAnimationSpeedMultiplier => _common.WalkAnimationSpeedMultiplier;
        public float AttackEventTime => Weaponry.Active.MainHand != null ? Weaponry.Active.AttackEventTime : 0.5f;

        public Vector3 FixedPosition { get; set; }
        private WorldEntity _closestTarget;
        public WorldEntity AttackTarget;
        public WorldEntity ChaseTarget;

        public bool SelectedByPlayer { get; private set; }

        public event Action<bool> WasSelectedByPlayer;
        public event Action ExecutedStopMovement;
        public event Action<float, float> HealthChanged;
        public event Action<Vector3> AttackBegin;
        public event Action<Vector3> ShootBegin;
        public event Action<Vector3> ProjectileLaunched;
        public event Action<Vector3> WalkTo;
        public event Action<Vector3> ChaseTo;
        public event Action AttackEnd;
        public event Action ShootEnd;
        public event Action Died;

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

        public Unit(Vector3 position, int factionId, EntityType type, UnitConfig config, UnitCommonConfig common)
        {
            FactionId = factionId;
            EntityType = type;
            Config = config;
            _common = common;

            Stats = new UnitStats(config.BaseStrength, config.BaseSpeed, config.BaseSpirit);

            var primary = new WeaponSet(config.MainHandPrimary, config.OffHandPrimary);
            var secondary = new WeaponSet(config.MainHandSecondary, config.OffHandSecondary);
            Weaponry = new UnitWeaponry(primary, secondary);

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

            StateMachine.SetState(StateMachine.Idle);
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
            StateMachine.Walk.Destination = destination;
            StateMachine.SetState(StateMachine.Walk);
            // Debug.Log("Performing walk!");
            WalkTo?.Invoke(destination);
        }
        public void PerformChase(WorldEntity entity)
        {
            // Debug.Log("Performing chase!");

            ChaseTo?.Invoke(entity.Position);
        }
        public void StopMovement()
        {
            ExecutedStopMovement?.Invoke();
        }
        public void PerformAttack(WorldEntity entity)
        {
            // Debug.Log("Performing attack!");

            AttackBegin?.Invoke(entity.Position);
        }
        public void PerformShoot(WorldEntity entity)
        {
            ShootBegin?.Invoke(entity.Position);
        }
        public void PerformProjectileLaunch(WorldEntity target)
        {
            if (AttackTarget == null) return;
            Debug.Log("Launch 1");
            ProjectileLaunched?.Invoke(target.Position);
        }
        public void PerformAttackDamage(WorldEntity target)
        {
            if (target is Unit unit && unit.Health.IsAlive)
                unit.Health.TakeDamage(AttackDamage);
        }
        public void PerformDeath()
        {
            StateMachine.SetState(StateMachine.Dead);
            SelectedByPlayer = false;
            WasSelectedByPlayer?.Invoke(false);
            Died?.Invoke();
        }
        public void StopAttack()
        {
            AttackEnd?.Invoke();
        }
        public void StopShoot()
        {
            ShootEnd?.Invoke();
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
                if (sqrDist < closestSqrDist) {
                    closestSqrDist = sqrDist;
                    closest = e;
                }
            }
            ClosestTarget = closest;

            StateMachine.UpdateInteractions(closest);
        }
        public bool CanAttack(WorldEntity entity)
        {
            if (entity is Unit unit) {
                if (unit.FactionId != FactionId) {
                    float distSqr = (entity.Position - Position).sqrMagnitude;
                    if (distSqr < (AttackRadius * AttackRadius)) {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CanShootTarget(WorldEntity entity)
        {
            if (!CanShoot) return false;

            if (entity is Unit unit) {
                if (unit.FactionId != FactionId) {
                    float distSqr = (entity.Position - Position).sqrMagnitude;
                    if (distSqr <= (unit.AttackRadius * unit.AttackRadius)) {
                        return true;
                    }
                }
            }
            return false;
        }

        internal void UpdateActions(float deltaTime)
        {
            StateMachine.UpdateActions(deltaTime);
            // todo attack speed implement
            // _attackCooldownTimer -= deltaTime;
            //     if (CurrentLookTarget == null) return;

            //     float distSqr = (CurrentLookTarget.Position - Position).sqrMagnitude;            
            //     if (distSqr <= AttackRadius * AttackRadius) {
            //         PerformAttack();
            //     }
        }


    }
}