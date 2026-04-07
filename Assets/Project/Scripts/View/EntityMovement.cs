using System;
using System.Collections;
using Aegis.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Aegis.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;

        private Unit _unit;

        private Coroutine _walkCoroutine;
        private float _positionSyncEpsilon = 0.0001f;
        private Vector3 _lastSyncedPosition;

        public float Velocity => _agent.velocity.magnitude;
        public bool IsWalking => _walkCoroutine != null;

        public event Action OnWalkStarted;
        public event Action OnWalkStopped;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null) Debug.LogError("NavMeshAgent not found on EntityMovement!");
        }

        public void Bind(Unit unit)
        {
            if (unit is null) return;

            _unit = unit;
        }
        public void Unbind()
        {
            _unit = null;
        }

        private void LateUpdate()
        {
            var current = transform.position;
            if ((current - _lastSyncedPosition).sqrMagnitude > _positionSyncEpsilon) {
                _lastSyncedPosition = current;
                _unit.Position = current;
            }
        }

        public void MoveTo(Vector3 destination)
        {
            Stop();
            _walkCoroutine = StartCoroutine(WalkRoutine(destination));
        }
        private IEnumerator WalkRoutine(Vector3 destination)
        {
            if (!_agent.SetDestination(destination)) {
                Debug.LogWarning("EntityMovement: failed to set destination");
                yield break;
            }
            OnWalkStarted?.Invoke();

            try {
                yield return new WaitUntil(() => this != null && !_agent.pathPending);
                yield return new WaitUntil(() => this == null || _agent.ReachedDestinationOrGaveUp());
                OnWalkFinished();
            } finally { }

            OnWalkFinished();
        }

        public void Stop()
        {
            if (_walkCoroutine != null) {
                StopCoroutine(_walkCoroutine);
                _walkCoroutine = null;
            }

            if (_agent != null && _agent.isActiveAndEnabled)
                _agent.ResetPath();            
        }

        public void LookAt(Vector3 targetPosition)
        {
            if (_walkCoroutine != null) return;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);
        }        

        private void OnWalkFinished()
        {
            OnWalkStopped?.Invoke();

            if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
                _unit?.MovementComplete(transform.position);
        }

        private void OnDestroy()
        {
            Stop();
        }
    }
}