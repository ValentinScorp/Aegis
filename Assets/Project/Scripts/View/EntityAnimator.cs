using System;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class EntityAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private EntityMovement _movement;
        [SerializeField] private AnimationClip _attackClip;
        [SerializeField] private AnimationClip _shootClip;
        private int _currentStateHash;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int ShootHash = Animator.StringToHash("Shoot");
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
        private static readonly int ShootSpeedHash = Animator.StringToHash("ShootSpeed");

        private float _attackAnimationLength;
        private float _shootAnimationLength;
        public event Action MeleeHit;
        public event Action RangedRelease;

        private void Awake()
        {
            if (_animator == null) Debug.Log("No animator attached!");
            if (_movement is null) Debug.Log("No movement script attached!");
            // _animator = GetComponentInChildren<Animator>();
            // if (_animator == null) Debug.LogWarning("Can't find <Animator> in children!");

            // _movement = GetComponent<EntityMovement>();
            _attackAnimationLength = _attackClip != null ? _attackClip.length : 1f;
            _shootAnimationLength = _shootClip != null ? _shootClip.length : 1f;
            // TODO Remove Animation Events
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
        public void PlayAttack(float attackTime)
        {
            float speed = _attackAnimationLength / attackTime;
            _animator.SetFloat(AttackSpeedHash, speed);
            PlayOnce(AttackHash);
        }
        public void PlayShoot(float shootTime)
        {
            float speed = _shootAnimationLength / shootTime;
            _animator.SetFloat(ShootSpeedHash, speed);
            PlayOnce(AttackHash);
        }
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
        private float GetClipLength(string clipName)
        {
            foreach (var clip in _animator.runtimeAnimatorController.animationClips) {
                if (clip.name == clipName)
                    return clip.length;
            }

            Debug.LogWarning($"Clip '{clipName}' not found!");
            return 1f;
        }

    }
}