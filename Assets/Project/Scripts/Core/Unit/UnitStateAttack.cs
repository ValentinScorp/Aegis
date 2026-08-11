// UnitStateAttack.cs
namespace Aegis.Core
{
    public class UnitStateAttack : IUnitState
    {
        private readonly Unit _self;
        private float _cooldownTimer;
        private bool _damageDone;

        public UnitStateAttack(Unit owner) => _self = owner;

        public void Enter()
        {
            _self.StopMovement();
            _self.PerformAttackAction(_self.AttackTarget);
            _cooldownTimer = 0f;
            _damageDone = false;
        }

        public void Exit()
        {
            _self.StopAttackAction();
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
            if (distSqr > _self.AttackRange * _self.AttackRange) {
                _self.ChaseTarget = _self.AttackTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);
                return;
            }

            _cooldownTimer += deltaTime;
            if (!_damageDone && _cooldownTimer >= _self.AttackTime * _self.AttackEventTime) {
                _self.PerformAttackImpact(_self.AttackTarget);
                _damageDone = true;
            }

            if (_cooldownTimer >= _self.AttackTime) {
                _cooldownTimer = 0f;
                _self.PerformAttackAction(_self.AttackTarget);
                _damageDone = false;
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget) { }
    }
}