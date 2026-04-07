using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateIdle : IUnitState
    {
        private Unit _self;

        public UnitStateIdle(Unit unit)
        {
            _self = unit;
        }
        public void Enter(Vector3? destination = null)
        {
            Debug.Log("Enter Idle");            

            _self.AttackTarget = null;
            _self.ChaseTarget = null;
        }
        public void Exit()
        {
            Debug.Log("Exit Idle");            
        }
        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            if (closestTarget == null) {
                Debug.Log("Closest target is null");
                return;
            }

            if (_self.CanAttack(closestTarget)) {
                Debug.Log("Idle state can attack");
                _self.AttackTarget = closestTarget;
                _self.StateMachine.SetState(_self.StateMachine.Attack);
            } else {
                Debug.Log("Idle state cannot attack");
                _self.ChaseTarget = closestTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
            }
        }
        public void OnActionsUpdate(float deltaTime)
        {
        }

    }
}