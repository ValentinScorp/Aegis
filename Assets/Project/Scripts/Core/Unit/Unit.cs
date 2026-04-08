
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class Unit : WorldEntity, IFactionMember
    {
        public int FactionId { get; private set; }
        public float SearchRadius = 6.0f;
        public float ChaseRadius = 7.0f;
        public float AttackRadius = 2.0f;
        public float AttackTime = 1.5f;
        public Vector3 FixedPosition { get; set; }
        private WorldEntity _closestTarget;
        public WorldEntity AttackTarget;
        public WorldEntity ChaseTarget;

        public bool SelectedByPlayer { get; private set; }

        public event Action<bool> WasSelectedByPlayer;
        public event Action ExecutedStopMovement;
        public event Action<Vector3> AttackBegin;
        public event Action<Vector3> WalkTo;
        public event Action<Vector3> ChaseTo;
        public event Action AttackEnd;
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
        public Unit(Vector3 position, int factionId)
        {
            FactionId = factionId;
            StateMachine = new UnitStateMachine(this);
            Position = position;
            FixedPosition = Position;
        }
        public void MovementComplete(Vector3 position)
        {
            Position = position;

            StateMachine.SetState(StateMachine.Idle);
        }
        public void Select(bool selected)
        {
            SelectedByPlayer = selected;
            WasSelectedByPlayer?.Invoke(selected);
        }
        public void PerformWalk(Vector3 destination)
        {
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
        public void PerformAttack(Vector3 targetPosition)
        {
            AttackBegin?.Invoke(targetPosition);
        }
        public void StopAttack()
        {
            AttackEnd?.Invoke();
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