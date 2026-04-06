using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitStateChase : IUnitState
    {
         private Unit _self;

        public UnitStateChase(Unit owner)
        {
            _self = owner;
        }
        public void Enter()
        {
            _self.PerformMovementTo(_self.ChaseTarget.Position);
        }

        public void Exit()
        {
        }

        public void OnActionsUpdate(float deltTime)
        {
            if (_self.CanAttack(_self.ChaseTarget)) {
                _self.AttackTarget = _self.ChaseTarget;
                _self.StateMachine.SetState(_self.StateMachine.Attack);
            }
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            _self.PerformMovementTo(_self.ChaseTarget.Position);
        }
    }
}