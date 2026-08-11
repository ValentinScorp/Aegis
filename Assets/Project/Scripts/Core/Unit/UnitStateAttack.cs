// UnitStateAttack.cs
namespace Aegis.Core
{
    public class UnitStateAttack : IUnitState
    {
        public UnitState State => UnitState.Attack;
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
            if (_self.AttackTarget == null)
                return;

            _cooldownTimer += deltaTime;

            if (!_damageDone && _cooldownTimer >= _self.AttackTime * _self.AttackEventTime)
            {
                _self.PerformAttackImpact(_self.AttackTarget);
                _damageDone = true;
            }

            if (_cooldownTimer >= _self.AttackTime)
            {
                _cooldownTimer = 0f;
                _damageDone = false;
                _self.PerformAttackAction(_self.AttackTarget);
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget) { }
    }
}