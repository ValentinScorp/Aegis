using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class WorldUpdater : MonoBehaviour
    {
        [SerializeField] private float _interationsIntervalTime = 0.5f;
        [SerializeField] private float _actionsIntervalTime = 0.1f;
        private readonly List<IWorldUpdatable> _tickables = new();
        private float _interactionsTimer;
        private float _actionsTimer;

        private void OnEnable()
        {
            Register(World.Instance);
        }
        private void OnDestroy()
        {
            Unregister(World.Instance);
        }

        private void Update()
        {
            _interactionsTimer += Time.deltaTime;
            _actionsTimer += Time.deltaTime;

            if (_interactionsTimer >= _interationsIntervalTime) {
                _interactionsTimer = 0f;
                UpdateInteractions();
            }

            if (_actionsTimer >= _actionsIntervalTime) {
                _actionsTimer = 0f;           
                UpdateActions(Time.deltaTime);                
            }
        }

        private void UpdateInteractions()
        {
            for (int i = _tickables.Count - 1; i >= 0; i--)
                _tickables[i].OnInteractionsUpdate();
        }
        private void UpdateActions(float deltaTime)
        {
            for (int i = _tickables.Count - 1; i >= 0; i--)
                _tickables[i].OnActionsUpdate(deltaTime);
        }

        public void Register(IWorldUpdatable t) => _tickables.Add(t);
        public void Unregister(IWorldUpdatable t) => _tickables.Remove(t);
    }
}
