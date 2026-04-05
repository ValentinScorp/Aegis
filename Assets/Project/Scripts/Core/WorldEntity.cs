using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class WorldEntity
    {
        private float AttackRadius = 2.0f;
        private float LookRadius = 10.0f;

        private WorldEntity _currentLookTarget;

        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public int factionId { get; internal set; }
        public bool selectedByPlayer { get; private set; }

        public event Action<bool> SelectedByPlayer;
        public event Action PerformedAttack;
        public event Action<Vector3> MovedTo;
        public event Action<WorldEntity> LookedAt;

        
        public WorldEntity CurrentLookTarget {
            get => _currentLookTarget;
            set {
                if (_currentLookTarget != value) {
                    _currentLookTarget = value;
                    LookedAt?.Invoke(_currentLookTarget);
                }
            }
        }

        public void SetPosition(Vector3 position)
        {
            if (this.position != position) {
                this.position = position;
            }
        }
        public void Select(bool selected)
        {
            selectedByPlayer = selected;
            SelectedByPlayer?.Invoke(selected);
        }
        public void PerformMovementTo(Vector3 destination)
        {
            MovedTo?.Invoke(destination);
        }
        public void LookAtEntity(WorldEntity target)
        {
            CurrentLookTarget = target;
        }
        public void PerformAttack()
        {
            PerformedAttack?.Invoke();
        }
        public void ScanForTarget(IReadOnlyList<WorldEntity> allEntities)
        {
            WorldEntity closest = null;
            float closestSqrDist = LookRadius * LookRadius;

            foreach (var e in allEntities) {
                if (e == this || e.factionId == factionId) continue;

                float sqrDist = (e.position - position).sqrMagnitude;
                if (sqrDist < closestSqrDist) {
                    closestSqrDist = sqrDist;
                    closest = e;
                }
            }
            CurrentLookTarget = closest;
        }

        internal void ProcessAction(float deltaTime)
        {
            // todo attack speed implement
            // _attackCooldownTimer -= deltaTime;
            if (CurrentLookTarget == null) return;

            float distSqr = (CurrentLookTarget.position - position).sqrMagnitude;
            if (distSqr <= AttackRadius * AttackRadius) {
                PerformAttack();
            }
        }

    }
}
