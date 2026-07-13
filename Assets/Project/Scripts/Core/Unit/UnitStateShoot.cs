using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateShoot : IUnitState
    {
        private Unit _self;
        private float _attackCooldownTimer;
        private bool _damageDealed;

        public UnitStateShoot(Unit owner)
        {
            _self = owner;
        }

        public void Enter()
        {
            _self.StopMovement();
            _self.PerformAttack(_self.AttackTarget);
            _attackCooldownTimer = 0f;
            _damageDealed = false;
        }

        public void Exit()
        {
            _self.StopAttack();
            _attackCooldownTimer = 0f;
        }

        public void OnActionsUpdate(float deltaTime)
        {
            if (_self.AttackTarget == null) {
                _self.StateMachine.SetState(_self.StateMachine.Idle);
                return;
            }

            if (_self.AttackTarget is Unit unit && !unit.Health.IsAlive) {
                _self.StateMachine.SetState(_self.StateMachine.Idle);
                return;
            }

            float distSqr = (_self.AttackTarget.Position - _self.Position).sqrMagnitude;

            if (distSqr > _self.ShootRadius * _self.ShootRadius) {
                _self.ChaseTarget = _self.AttackTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
                return;
            }

            _attackCooldownTimer += deltaTime;

             if (!_damageDealed && _attackCooldownTimer >= _self.AttackEventTime * _self.AttackTime) {
                _self.PerformShoot(_self.AttackTarget);
                _damageDealed = true;
            }

            if (_attackCooldownTimer >= _self.AttackTime) {
                _self.PerformAttack(_self.AttackTarget);
                _attackCooldownTimer = 0f;
                _damageDealed = false;
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
        }
    }
}