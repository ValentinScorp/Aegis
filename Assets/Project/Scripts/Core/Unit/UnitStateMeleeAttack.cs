
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateMeleeAttack : IUnitState
    {
        private Unit _self;

        private float _attackCooldownTimer;
        private bool _damageDealed;

        public UnitStateMeleeAttack(Unit owner)
        {
            _attackCooldownTimer = 0f;
            _self = owner;
        }
        public void Enter()
        {
            _self.StopMovement();

            // 
            _self.PerformAttack(_self.AttackTarget);
            _damageDealed = false;
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
            if (_self.AttackTarget is Unit unit) {
                if (!unit.Health.IsAlive) {
                    _self.StateMachine.SetState(_self.StateMachine.Idle);
                    return;
                }
            }
            if (!_self.CanAttack(_self.AttackTarget)) {
                // Debug.Log("Cant attack! Need chase!");
                _self.ChaseTarget = _self.AttackTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
                return;
            }
            _attackCooldownTimer += deltaTime;
            if (!_damageDealed && _attackCooldownTimer >= _self.AttackTime * _self.AttackEventTime) {
                DealDamage();
            }
            if (_attackCooldownTimer >= _self.AttackTime) {
                // Debug.Log("New Attack!");
                _self.PerformAttack(_self.AttackTarget);
                _attackCooldownTimer = 0f;
                _damageDealed = false;
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {

        }

        internal void DealDamage()
        {
            if (_self.AttackTarget == null) return;
            _self.PerformAttackDamage(_self.AttackTarget);
            _damageDealed = true;
        }
    }
}