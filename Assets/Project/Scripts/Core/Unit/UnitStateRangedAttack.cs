using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateRangedAttack : IUnitState
    {
        private Unit _self;
        private float _shootCooldownTimer;
        private bool _projectileLaunched;

        public UnitStateRangedAttack(Unit owner)
        {
            _self = owner;
        }

        public void Enter()
        {
            _self.StopMovement();
            _self.PerformShoot(_self.AttackTarget);
            _shootCooldownTimer = 0f;
            _projectileLaunched = false;
        }

        public void Exit()
        {
            _self.StopShoot();
            _shootCooldownTimer = 0f;
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

            _shootCooldownTimer += deltaTime;

             if (!_projectileLaunched && _shootCooldownTimer >= _self.ShootTime * _self.ShootEventTime) {
                _self.PerformProjectileLaunch(_self.AttackTarget);
                _projectileLaunched = true;
            }

            if (_shootCooldownTimer >= _self.ShootTime) {
                _shootCooldownTimer = 0f;
                _self.PerformShoot(_self.AttackTarget);                
                _projectileLaunched = false;
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
        }
    }
}