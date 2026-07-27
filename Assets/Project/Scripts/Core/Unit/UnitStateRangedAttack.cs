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

            if (distSqr > _self.AttackRadius * _self.AttackRadius) {
                _self.ChaseTarget = _self.AttackTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
                return;
            }

            _shootCooldownTimer += deltaTime;
            Debug.Log($"attack time:{_self.AttackTime}, event time: {_self.AttackEventTime}");
            if (!_projectileLaunched && _shootCooldownTimer >= _self.AttackTime * _self.AttackEventTime) {
                _self.PerformProjectileLaunch(_self.AttackTarget);
                _projectileLaunched = true;
            }

            if (_shootCooldownTimer >= _self.AttackTime) {
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