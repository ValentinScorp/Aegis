using System;
using Aegis.Core;
using UnityEngine;
using UnityEngine.AI;
using Aegis.Utilities;

namespace Aegis.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitAgentMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private Unit _unit;
        private bool _isMoving;
        private float _positionSyncEpsilon = 0.0001f;
        private Vector3 _lastFramePos;
        private bool _hasLastFramePos;
        private Vector3 _lastSyncedPosition;
        private Vector3 _realVelocity;
        public Vector3 RealVelocity => _realVelocity;
        public float AgentSpeed => _agent.isActiveAndEnabled ? _agent.velocity.magnitude : 0f;
        public float NormalizedSpeed => _agent.speed > 0f ? _agent.velocity.magnitude / _agent.speed : 0f; // 0..1
        public bool IsWalking => _isMoving;

        public event Action<Vector3> MovementCompleted;

        private void Awake()
        {
            _agent = ComponentResolver.Require(this, GetComponent<NavMeshAgent>());
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

            UpdateRealVelocity(current);

            if ((current - _lastSyncedPosition).sqrMagnitude > _positionSyncEpsilon) {
                _lastSyncedPosition = current;
                _unit.Position = current;
            }
            _unit.Velocity = _realVelocity;
        }
        private void UpdateRealVelocity(Vector3 current)
        {
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            if (_hasLastFramePos) {
                Vector3 raw = (current - _lastFramePos) / dt;
                raw.y = 0f;

                // телепорт або спавн не повинні давати шалену швидкість
                if (raw.sqrMagnitude > 30f * 30f) raw = Vector3.zero;

                _realVelocity = Vector3.Lerp(_realVelocity, raw, 0.3f); // згладжування
            }

            _lastFramePos = current;
            _hasLastFramePos = true;
        }

        public void Bind(Unit unit)
        {
            if (unit is null) return;
            _unit = unit;
            _agent.speed = unit.MoveSpeed;
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
        public void DisableAgent()
        {
            Stop();
            _agent.enabled = false;
        }
        public void EnableAgent()
        {
            _agent.enabled = true;
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