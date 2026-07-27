using System;

namespace Aegis.Core
{
    public class UnitStateMachine
    {
        private IUnitState _currentState;
        public IUnitState Current => _currentState;
        public UnitStateIdle Idle;
        public UnitStateChase Chase;
        public UnitStateAttack Attack;
        public UnitStateMeleeAttack MeleeAttack;
        public UnitStateRangedAttack RangedAttack;
        public UnitStateWalk Walk;
        public UnitStateDead Dead;

        public event Action<UnitAction> StateChanged;
        public UnitAction State { get; private set; }
        public UnitStateMachine(Unit owner)
        {
            Idle = new UnitStateIdle(owner);
            Chase = new UnitStateChase(owner);
            Attack = new UnitStateAttack(owner);
            MeleeAttack = new UnitStateMeleeAttack(owner);
            RangedAttack = new UnitStateRangedAttack(owner);
            Walk = new UnitStateWalk(owner);
            Dead = new UnitStateDead(owner);

            SetState(Idle);
        }
        public void SetState(IUnitState newState)
        {
            if (_currentState == newState)
                return;

            // Debug.Log($"Settin new satate {newState}");
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