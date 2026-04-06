using System;
using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitStateIdle : IUnitState
    {
        private Unit _self;

        public UnitStateIdle(Unit unit)
        {
            _self = unit;
        }
        public void Enter()
        {            
        }
        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            if (_self.CanAttack(closestTarget)) {
                _self.AttackTarget = closestTarget;
                _self.StateMachine.SetState(_self.StateMachine.Attack);
            } else {
                _self.ChaseTarget = closestTarget;
                _self.StateMachine.SetState(_self.StateMachine.Chase);                
            }
        }
        public void OnActionsUpdate(float deltaTime)
        {
        }
        public void Exit()
        {
        }
    }
}