using System;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class EntityAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private int _currentStateHash;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int WalkHash = Animator.StringToHash("Walk");
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
            }
            var info = _animator.GetCurrentAnimatorStateInfo(0);
        }
        public void PlayAttack() => PlayOnce(AttackHash);
        public void PlayIdle() => PlayLooping(IdleHash);
        public void PlayWalk() => PlayLooping(WalkHash);

        private void PlayOnce(int stateHash)
        {
            _currentStateHash = 0;
            _animator.Play(stateHash, 0, 0f);
            _currentStateHash = stateHash;
        }

        private void PlayLooping(int stateHash)
        {
            if (_currentStateHash == stateHash) return;
            _currentStateHash = stateHash;
            _animator.Play(stateHash, 0, float.NegativeInfinity);
        }
    }
}