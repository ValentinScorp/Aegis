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
        private int _currentStateHash;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
        private AnimationEventReceiver _receiver;

        private float _attackAnimationLength;
        private float _attackEventNormalizedTime = 0.5f;
        public event Action AttackFrame;
        public float AttackEventNormalizedTime => _attackEventNormalizedTime;

        private void Awake()
        {
            if (_animator == null) Debug.Log("No animator attached!");
            if (_movement is null) Debug.Log("No movement script attached!");
            // _animator = GetComponentInChildren<Animator>();
            // if (_animator == null) Debug.LogWarning("Can't find <Animator> in children!");

            // _movement = GetComponent<EntityMovement>();
            _receiver = GetComponentInChildren<AnimationEventReceiver>();
            if (_receiver != null) {
                _receiver.ReleaseArrow += OnAttackFrame;
                _receiver.SwordHit += OnAttackFrame;
            }

            _attackAnimationLength = _attackClip != null ? _attackClip.length : 1f;
            _attackEventNormalizedTime = GetEventNormalizedTime(_attackClip, "MeleeHitEvent");
            Debug.Log($"Attack event at: {_attackEventNormalizedTime}");
        }
        private void OnDestroy()
        {
            if (_receiver != null) {
                _receiver.ReleaseArrow -= OnAttackFrame;
                _receiver.SwordHit -= OnAttackFrame;
            }
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
        private float GetEventNormalizedTime(AnimationClip clip, string eventName)
        {
            foreach (var evt in clip.events) {
                if (evt.functionName == eventName)
                    return evt.time / clip.length;
            }

            Debug.LogWarning($"Event '{eventName}' not found in '{clip.name}'!");
            return 0.5f;
        }
        void OnAttackFrame()
        {
            AttackFrame?.Invoke();
        }
    }
}