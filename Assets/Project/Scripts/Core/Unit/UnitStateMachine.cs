using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitStateMachine
    {
        private readonly Dictionary<UnitState, IUnitState> _states = new();
        private IUnitState _currentState;
        private UnitState _currentType;
        public IUnitState Current => _currentState;
        public UnitState CurrentType => _currentType;
        private readonly UnitStateTransition _transitions;

        public UnitStateMachine(Unit owner)
        {
            Register(new UnitStateIdle(owner));
            Register(new UnitStateWalk(owner));
            Register(new UnitStateChase(owner));
            Register(new UnitStateAttack(owner));
            Register(new UnitStateDead(owner));

            _transitions = new UnitStateTransition(owner, this);

            SetState(UnitState.Idle);
        }
        private void Register(IUnitState state)
        {
            _states[state.State] = state;
        }
        public void SetState(UnitState type)
        {
            if (_currentType == type)
                return;

            if (!_states.TryGetValue(type, out var newState))
            {
                UnityEngine.Debug.LogWarning($"[UnitStateMachine] State {type} not registered");
                return;
            }

            _currentState?.Exit();
            _currentState = newState;
            _currentType = type;
            _currentState.Enter();
        }
        public T GetState<T>() where T : class, IUnitState
        {
            foreach (var state in _states.Values)
            {
                if (state is T typed)
                    return typed;
            }
            return null;
        }
        public void UpdateActions(float deltaTime)
        {
            _currentState?.OnActionsUpdate(deltaTime);
        }
        public void UpdateInteractions(WorldEntity closestTarget)
        {
            _currentState?.OnInteractionsUpdate(closestTarget);
            _transitions.Evaluate(closestTarget);
        }
    }
}