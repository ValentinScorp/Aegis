
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class Unit : WorldEntity, IFactionMember, IDamageable
    {
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
        public float SearchRadius => Config?.SearchRadius ?? 6.0f;
        public float ChaseRadius => Config?.ChaseRadius ?? 7.0f;
        public float AttackRadius => Config?.AttackRadius ?? 2.0f;
        public float AttackTime => Config?.AttackTime ?? 1.5f;
        public float AttackDamage => Config?.AttackDamage ?? 12.0f;
        public float AttackEventTime => Config?.AttackEventTime ?? 0.5f;
        public bool CanShoot => Config?.CanShoot ?? false;
        public float ShootRadius => Config?.ShootRadius ?? 10.0f;

        public Vector3 FixedPosition { get; set; }
        private WorldEntity _closestTarget;
        public WorldEntity AttackTarget;
        public WorldEntity ChaseTarget;

        public bool SelectedByPlayer { get; private set; }

        public event Action<bool> WasSelectedByPlayer;
        public event Action ExecutedStopMovement;
        public event Action<Vector3> AttackBegin;
        public event Action<Vector3> ShootBegin;
        public event Action<Vector3> WalkTo;
        public event Action<Vector3> ChaseTo;
        public event Action AttackEnd;
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

        public Unit(Vector3 position, int factionId, EntityType type, UnitConfig config)
        {
            FactionId = factionId;
            EntityType = type;

            Config = config;

            Health = new Health(config.MaxHealth);

            StateMachine = new UnitStateMachine(this);
            Position = position;
            FixedPosition = Position;
        }
        public void SetConfig(UnitConfig config)
        {
            Config = config;
            if (Health == null) {
                Health = new Health(config.MaxHealth);
                Health.Depleted += OnHealthDepleted;
            }
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
            WalkTo?.Invoke(destination);
        }
        public void PerformChase(WorldEntity entity)
        {
            ChaseTo?.Invoke(entity.Position);
        }
        public void StopMovement()
        {
            ExecutedStopMovement?.Invoke();
        }
        public void PerformAttack(WorldEntity entity)
        {
            AttackBegin?.Invoke(entity.Position);
        }
        public void PerformShoot(WorldEntity entity)
        {
            ShootBegin?.Invoke(entity.Position);
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