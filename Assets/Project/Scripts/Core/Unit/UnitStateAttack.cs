// UnitStateAttack.cs
namespace Aegis.Core
{
    public class UnitStateAttack : IUnitState
    {
        private readonly Unit _self;
        private float _cooldownTimer;
        private bool _eventFired;

        public UnitStateAttack(Unit owner) => _self = owner;

        public void Enter()
        {
            _self.StopMovement();
            _self.PerformAttackAction(_self.AttackTarget);
            _cooldownTimer = 0f;
            _eventFired = false;
        }

        public void Exit()
        {
            _self.StopAttackAnimation();
            _cooldownTimer = 0f;
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

            _cooldownTimer += deltaTime;
            if (!_eventFired && _cooldownTimer >= _self.AttackTime * _self.AttackEventTime) {
                _self.PerformAttackEffect(_self.AttackTarget);
                _eventFired = true;
            }

            if (_cooldownTimer >= _self.AttackTime) {
                _cooldownTimer = 0f;
                _self.PerformAttackAction(_self.AttackTarget);
                _eventFired = false;
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget) { }
    }
}