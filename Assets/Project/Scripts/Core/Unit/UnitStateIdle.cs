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
        public void Enter()
        {
            // Debug.Log("Enter Idle");            
        }
        public void Exit()
        {
            // Debug.Log("Exit Idle");            
        }
        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            if (closestTarget == null) return;

            float distSqr = (closestTarget.Position - _self.Position).sqrMagnitude;

            if (distSqr > _self.AttackRange * _self.AttackRange) {
                _self.ChaseTarget = closestTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
                return;
            }

            _self.AttackTarget = closestTarget;
            _self.StateMachine.SetState(_self.CanShoot ? _self.StateMachine.RangedAttack : _self.StateMachine.MeleeAttack);
        }
        public void OnActionsUpdate(float deltaTime)
        {
        }

    }
}