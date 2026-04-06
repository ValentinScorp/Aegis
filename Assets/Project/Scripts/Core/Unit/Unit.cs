
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class Unit : WorldEntity, IFactionMember
    {
        public int FactionId { get; private set; }
        public float SearchRadius = 10.0f;
        public float AttackRadius = 2.0f;
        public float AttackTime = 1.2f;
        private WorldEntity _closestTarget;
        public WorldEntity AttackTarget;
        public WorldEntity ChaseTarget;

        public bool selectedByPlayer { get; private set; }

        public event Action<bool> WasSelectedByPlayer;
        public event Action<WorldEntity> PerformedAttack;
        public event Action<Vector3> MovedTo;
        public event Action<WorldEntity> LookedAt;
        public UnitStateMachine StateMachine { get; private set; }
        
        public WorldEntity ClosestTarget {
            get => _closestTarget;
            set {
                if (_closestTarget != value) {
                    _closestTarget = value;
                    LookedAt?.Invoke(_closestTarget);
                }
            }
        }
        public Unit(int factionId)
        {
            FactionId = factionId;
            StateMachine = new UnitStateMachine(this);
        }
        public void MovementComplete(Vector3 position)
        {
            Position = position;
        }
        public void Select(bool selected)
        {
            selectedByPlayer = selected;
            WasSelectedByPlayer?.Invoke(selected);
        }
        public void PerformMovementTo(Vector3 destination)
        {
            MovedTo?.Invoke(destination);
        }
        public void LookAtEntity(WorldEntity target)
        {
            ClosestTarget = target;
        }
        public void PerformAttack()
        {
            PerformedAttack?.Invoke(ClosestTarget);
        }
        public void UpdateInteractions(IReadOnlyList<WorldEntity> allEntities)
        {
            WorldEntity closest = null;
            float closestSqrDist = SearchRadius * SearchRadius;

            foreach (var e in allEntities) {
                if (e is IFactionMember fmEntity) {
                    if (fmEntity == this || fmEntity.FactionId == FactionId) continue;
                } else continue;

                float sqrDist = (e.Position - Position).sqrMagnitude;
                if (sqrDist < closestSqrDist) {
                    closestSqrDist = sqrDist;
                    closest = e;
                }
            }
            ClosestTarget = closest;

            StateMachine.UpdateInteractions(ClosestTarget);
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