using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateChase : IUnitState
    {
        private Unit _self;
        private Vector3 _lastPosition;

        public UnitStateChase(Unit owner)
        {
            _self = owner;
        }
        public void Enter(Vector3? destination = null)
        {
            Debug.Log("Enter Chase State!");

            if (_self.ChaseTarget == null) {
                _self.StateMachine.SetState(_self.StateMachine.Idle);
                return;
            }
            _self.UpdateChaseTargetPosition(_self.ChaseTarget);
            _lastPosition = _self.ChaseTarget.Position;
        }
        public void Exit()
        {
            Debug.Log("Exit Chase State!");

            _self.ChaseTarget = null;
        }

        public void OnActionsUpdate(float deltTime)
        {
            if (_self.CanAttack(_self.ChaseTarget)) {
                // _self.AttackTarget = _self.ChaseTarget;                
                _self.StateMachine.SetState(_self.StateMachine.Idle);
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            if (_lastPosition != _self.ChaseTarget.Position) {
                _self.UpdateChaseTargetPosition(_self.ChaseTarget);
                _lastPosition = _self.ChaseTarget.Position;
            }
        }
    }
}