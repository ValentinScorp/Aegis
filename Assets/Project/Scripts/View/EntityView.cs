using System;
using UnityEngine;
using UnityEngine.AI;
using Aegis.Core;
using System.Collections;

namespace Aegis.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityView : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        public event Action OnDestinationReached;
        private Vector3 _lastSyncedPosition;
        private float _positionSyncEpsilon = 0.0001f;

        private Coroutine _WalkCoroutine;

        public WorldEntity Entity { get; private set; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null) Debug.LogError("Can't find NavMeshAgent on EntityView!");
        }


        private void OnDestroy()
        {
            Unbind();
            StopWalking();
        }
        private void Update()
        {
            if (_animator != null && _agent != null) {
                _animator.SetFloat("WalkSpeed", _agent.velocity.magnitude);
            }
        }
        private void LateUpdate()
        {
            if (Entity == null) return;

            var current = transform.position;
            if ((current - _lastSyncedPosition).sqrMagnitude < _positionSyncEpsilon) return;

            Entity.SetPosition(current);
            _lastSyncedPosition = current;
        }
        public void Bind(WorldEntity entity)
        {
            Entity = entity;
            transform.position = entity.position;
            _lastSyncedPosition = transform.position;
        }
        public void Unbind()
        {
            Entity = null;
        }
        public void MoveTo(Vector3 worldPosition)
        {
             StopWalking();
            _WalkCoroutine = StartCoroutine(WalkRoutine(worldPosition));            
        }
        private void StopWalking()
        {
            if (_WalkCoroutine != null) {
                StopCoroutine(_WalkCoroutine);
                _WalkCoroutine = null;
            }

            if (_animator != null)
                _animator.SetBool("isWalking", false);
        }
        private IEnumerator WalkRoutine(Vector3 destination)
        {
            if (!_agent.SetDestination(destination)) {
                Debug.LogWarning("Failed to set destination");
                yield break;
            }
            _animator.SetBool("isWalking", true);
            
            try {
                yield return new WaitUntil(() => !_agent.pathPending);
                yield return new WaitUntil(() => _agent.ReachedDestinationOrGaveUp());
                OnWalkFinished();
            } finally {}
        }
        private void OnWalkFinished()
        {
            _animator.SetBool("isWalking", false);
            if (_agent.pathStatus == NavMeshPathStatus.PathComplete) {
                OnDestinationReached?.Invoke();
            }            
        }
    }
}
