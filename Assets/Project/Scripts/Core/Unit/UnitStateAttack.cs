
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
        public void Enter()
        {
            _self.StopMovement();
            _self.PerformAttack(_self.AttackTarget.Position);
            // Debug.Log("Enter Attack State!");
        }
        public void Exit()
        {
            // Debug.Log("Exit Attack State!");
            _self.StopAttack();
            _attackCooldownTimer = 0f;
        }

        public void OnActionsUpdate(float deltaTime)
        {
            // Debug.Log($"deltaTime: {deltaTime}, timer: {_attackCooldownTimer}");

            if (_self.AttackTarget == null) {
                _self.StateMachine.SetState(_self.StateMachine.Idle);
                return;
            }
            if (!_self.CanAttack(_self.AttackTarget)) {
                // Debug.Log("Cant attack! Need chase!");
                _self.ChaseTarget = _self.AttackTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
                return;
            }
            _attackCooldownTimer += deltaTime;
            // Debug.Log($"{_attackCooldownTimer}");
            if (_attackCooldownTimer >= _self.AttackTime) {
                // Debug.Log("New Attack!");
                _self.PerformAttack(_self.AttackTarget.Position);
                _attackCooldownTimer = 0f;
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {

        }
    }
}