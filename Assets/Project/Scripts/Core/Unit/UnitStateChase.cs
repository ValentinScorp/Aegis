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
        public void Enter()
        {
            //  Debug.Log("Enter Chase State!");

            if (_self.ChaseTarget == null) {
                _self.StateMachine.SetState(_self.StateMachine.Idle);
                return;
            }
            _self.PerformChase(_self.ChaseTarget);
            _lastPosition = _self.ChaseTarget.Position;
        }
        public void Exit()
        {
            // Debug.Log("Exit Chase State!");
            _self.StopMovement();
            // _self.ChaseTarget = null;
        }

        public void OnActionsUpdate(float deltTime)
        {
            if (_self.CanAttack(_self.ChaseTarget)) {
                _self.AttackTarget = _self.ChaseTarget;                
                _self.StateMachine.SetState(_self.StateMachine.Attack);
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            if ((_self.Position - _self.FixedPosition).sqrMagnitude > (_self.ChaseRadius * _self.ChaseRadius)) {
                _self.PerformWalk(_self.FixedPosition); 
                return;        
            }

            if (_lastPosition != _self.ChaseTarget.Position) {
                _self.PerformChase(_self.ChaseTarget);
                // Debug.Log($"{_self.ChaseTarget}");
                _lastPosition = _self.ChaseTarget.Position;
            }
        }
    }
}