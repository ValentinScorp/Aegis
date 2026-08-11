using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateChase : IUnitState
    {
        public UnitState State => UnitState.Chase;
        private Unit _self;
        private Vector3 _lastTargetPosition;

        public UnitStateChase(Unit owner)
        {
            _self = owner;
        }
        public void Enter()
        {
            if (_self.ChaseTarget == null)
                return;

            _self.PerformChase(_self.ChaseTarget);
            _lastTargetPosition = _self.ChaseTarget.Position;
        }
        public void Exit()
        {
            // Debug.Log("Exit Chase State!");
            _self.StopMovement();
        }

        public void OnActionsUpdate(float deltTime)
        {
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            if (_self.ChaseTarget == null)
                return;

            // Оновлюємо шлях, тільки якщо ціль зрушилася
            if (_lastTargetPosition != _self.ChaseTarget.Position) {
                _self.PerformChase(_self.ChaseTarget);
                _lastTargetPosition = _self.ChaseTarget.Position;
            }
        }
    }
}