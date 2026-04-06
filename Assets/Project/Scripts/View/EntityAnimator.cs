using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class EntityAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private EntityMovement _movement;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null) Debug.LogWarning("Can't find <Animator> in children!");

            _movement = GetComponent<EntityMovement>();

            _movement.OnWalkStarted += () => StartWalk();
            _movement.OnWalkStopped += () => StopWalk();
        }
        private void OnDestroy()
        {
            if (_movement == null) return;
            _movement.OnWalkStarted -= StartWalk;
            _movement.OnWalkStopped -= StopWalk;
        }

        private void Update()
        {
            if (_movement.IsWalking)
                _animator.SetFloat("WalkSpeed", _movement.Velocity);
        }

        public void PlayAttack(WorldEntity target)
        {
            _animator.SetTrigger("PerformAttack");
        }        
        private void StartWalk()
        {
            _animator.SetBool("PerformWalk", true);
        }
        private void StopWalk()
        {
            _animator.SetBool("PerformWalk", false);
        }
    }
}