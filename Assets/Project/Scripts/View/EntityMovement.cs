using System;
using Aegis.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Aegis.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private Unit _unit;
        private bool _isMoving;
        private float _positionSyncEpsilon = 0.0001f;
        private Vector3 _lastSyncedPosition;

        public float Velocity => _agent.velocity.magnitude;
        public bool IsWalking => _isMoving;

        public event Action<Vector3> MovementCompleted;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null) Debug.LogError("NavMeshAgent not found on EntityMovement!");
        }

        private void Update()
        {
            if (!_isMoving) return;

            if (_agent.pathPending) return;

            if (_agent.ReachedDestinationOrGaveUp())
                OnWalkFinished();
        }

        private void LateUpdate()
        {
            if (_unit == null) return;

            var current = transform.position;
            if ((current - _lastSyncedPosition).sqrMagnitude > _positionSyncEpsilon) {
                _lastSyncedPosition = current;
                _unit.Position = current;
            }
        }

        public void Bind(Unit unit)
        {
            if (unit is null) return;
            _unit = unit;
            MovementCompleted += _unit.MovementComplete;
        }

        public void Unbind()
        {
            MovementCompleted -= _unit.MovementComplete;
            _unit = null;
        }

        public void MoveTo(Vector3 destination)
        {
            Stop();

            if (!_agent.SetDestination(destination)) {
                Debug.LogWarning("EntityMovement: failed to set destination");
                return;
            }

            _isMoving = true;
        }
        public void LookAt(Vector3 targetPosition)
        {
            if (_isMoving) return;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);
        }
        public void Stop()
        {
            if (_isMoving) {
                _isMoving = false;
            }

            if (_agent != null && _agent.isActiveAndEnabled)
                _agent.ResetPath();
        }        

        private void OnWalkFinished()
        {
            _isMoving = false;

            if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
                MovementCompleted?.Invoke(transform.position);
        }

        private void OnDestroy()
        {
            if (_isMoving) Stop();
        }
    }
}