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
        private WorldEntity _entity;
        public WorldEntity Entity => _entity;

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
            if (_entity == null) return;

            var current = transform.position;
            if ((current - _lastSyncedPosition).sqrMagnitude > _positionSyncEpsilon) {
                _entity.SetPosition(current);
                _lastSyncedPosition = current;
            }
            if (_entity.CurrentLookTarget != null) {
                Vector3 dir = _entity.CurrentLookTarget.position - transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.001f) {
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }
        }
        public void Bind(WorldEntity entity)
        {
            _entity = entity;
            transform.position = entity.position;
            _lastSyncedPosition = transform.position;

            _entity.SelectedByPlayer += OnPlayerSelection;
            _entity.MovedTo += OnMoveAction;
            _entity.PerformedAttack += OnAttackAction;
            _entity.LookedAt += OnLookAt;
        }
        public void Unbind()
        {
            if (_entity != null) {
                _entity.SelectedByPlayer -= OnPlayerSelection;
                _entity.MovedTo -= OnMoveAction;
                _entity.PerformedAttack -= OnAttackAction;
                _entity.LookedAt -= OnLookAt;
            }
            _entity = null;
        }
        private void OnPlayerSelection(bool selected)
        {
            var selectable = GetComponent<Selectable>();
            if (selectable) {
                selectable.Select(selected);
            } else {
                Debug.LogWarning("<Selectable> Component not found on <EntityView>!");
            }
        }
        private void OnMoveAction(Vector3 destination)
        {
            StopWalking();
            _WalkCoroutine = StartCoroutine(WalkRoutine(destination));
        }
        private void OnAttackAction()
        {
            StopWalking();
            _animator.SetTrigger("PerformAttack");
        }

        private void StopWalking()
        {
            if (_WalkCoroutine != null) {
                StopCoroutine(_WalkCoroutine);
                _WalkCoroutine = null;
            }
            if (_agent != null && _agent.isActiveAndEnabled) {
                _agent.ResetPath();
            }

            if (_animator != null)
                _animator.SetBool("PerformWalk", false);
        }
        private IEnumerator WalkRoutine(Vector3 destination)
        {
            if (!_agent.SetDestination(destination)) {
                Debug.LogWarning("Failed to set destination");
                yield break;
            }
            _animator.SetBool("PerformWalk", true);

            try {
                yield return new WaitUntil(() => this != null && !_agent.pathPending);
                yield return new WaitUntil(() => this == null || _agent.ReachedDestinationOrGaveUp());
                OnWalkFinished();
            } finally { }

            if (this != null) OnWalkFinished();
        }
        private void OnWalkFinished()
        {
            _animator.SetBool("PerformWalk", false);
            if (_agent.pathStatus == NavMeshPathStatus.PathComplete) {
                OnDestinationReached?.Invoke();
            }
        }
        private void OnLookAt(WorldEntity target)
        {
            if (target is null) return;
            
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f) {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
