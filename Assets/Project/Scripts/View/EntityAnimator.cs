using System;
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
        }
        private void OnDestroy()
        {
        }

        private void Update()
        {
            if (_movement.IsWalking) {
                _animator.SetFloat("WalkSpeed", _movement.Velocity);
                _animator.SetFloat("ChaseSpeed", _movement.Velocity);
            }
        }

        public void PlayAttack() { _animator.SetTrigger("PerformAttack"); }  
        public void StopAttack() { _animator.ResetTrigger("PerformAttack"); }  
        public void PlayWalk() { _animator.SetBool("PerformWalk", true); }
        public void StopWalk() { _animator.SetBool("PerformWalk", false); }
    

        public void PlayChase() { _animator.SetBool("PerformChase", true); }
        public void StopChase()  { _animator.SetBool("PerformChase", false); }
    }
}