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
        public void PlayIdle() 
        {
            Debug.Log("Playing Idle!");
            Play(IdleHash);
        }
        public void PlayAttack() 
        {
            Debug.Log("Playing Attack!");
            Play(AttackHash);
        }
        public void PlayWalk() 
        {
            Debug.Log("Playing Walk!");
            Play(WalkHash);
        }
        private void Play(int stateHash, int layer = 0, float normalizedTime = 0)
        {
            if (_currentStateHash == stateHash) return;
            _currentStateHash = stateHash;
            _animator.Play(stateHash, layer, normalizedTime);
        }
    }
}