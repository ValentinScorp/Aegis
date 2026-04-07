
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateAttack : IUnitState
    {
        private Unit _self;

        private float _attackCooldownTimer;

        public UnitStateAttack(Unit owner)
        {
            _attackCooldownTimer = 0f;
            _self = owner;
        }
        public void Enter(Vector3? destination = null)
        {
            _self.PerformAttack();
            Debug.Log("Enter Attack State!");
        }
        public void Exit()
        {
            Debug.Log("Exit Attack State!");
            
        }

        public void OnActionsUpdate(float deltTime)
        {
            if (!_self.CanAttack(_self.AttackTarget)) {
                _self.StateMachine.SetState(_self.StateMachine.Idle);
                return;
            }
            _attackCooldownTimer += deltTime;
            if (_attackCooldownTimer >= _self.AttackTime)
                _self.PerformAttack();
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {

        }
    }
}