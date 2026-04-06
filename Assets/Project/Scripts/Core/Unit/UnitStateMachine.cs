
using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitStateMachine
    {
        private IUnitState _currentState;
        public IUnitState Idle;
        public IUnitState Chase;
        public IUnitState Attack;
        public IUnitState Walk;

        public UnitStateMachine(Unit owner)
        {
            Idle = new UnitStateIdle(owner);
            Chase = new UnitStateChase(owner);
            Attack = new UnitStateChase(owner);
            Walk = new UnitStateWalk(owner);

            SetState(Idle);
        }
        public void SetState(IUnitState newState)
        {
            if (_currentState == newState)
                return;

            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }
        public void UpdateActions(float deltaTime)
        {
            _currentState?.OnActionsUpdate(deltaTime);
        }
        public void UpdateInteractions(WorldEntity closestUnit)
        {
            _currentState?.OnInteractionsUpdate(closestUnit);
        }
    }
}