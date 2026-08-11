using System;
using System.Collections.Generic;
using Aegis.Core;
using Aegis.Utilities;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Aegis.View
{
    public class EntityAnimator : MonoBehaviour
    {
        [SerializeField] private ClipAction _swordHitClip;
        private Animator _animator;
        private Unit _unit;
        private int _currentStateHash;
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int SwordAttackHash = Animator.StringToHash("SwordAttack");
        private static readonly int BowShootHash = Animator.StringToHash("BowShoot");
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        private static readonly int WalkSpeedHash = Animator.StringToHash("WalkSpeed");
        private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

        private readonly Dictionary<int, float> _clipLengths = new();

        public bool IsWalking => _currentStateHash == WalkHash;
        public void SetWalkSpeed(float speed) => _animator.SetFloat(WalkSpeedHash, speed);

        private void Awake()
        {
            _animator = ComponentResolver.Require(this, GetComponentInChildren<Animator>());
            CacheClipLengths();
        }
        private void OnDestroy()
        {
        }
        public void Bind(Unit unit)
        {
            _unit = unit;
        }
        public void Unbind()
        {
            _unit = null;
        }

        private void CacheClipLengths()
        {
            foreach (var clip in _animator.runtimeAnimatorController.animationClips)
                _clipLengths[Animator.StringToHash(clip.name)] = clip.length;
        }

        public void PlayAttack(WeaponType weaponType, float attackTime)
        {
            int attackHash = weaponType switch {
                WeaponType.Bow => BowShootHash,
                _ => SwordAttackHash, // TODO: додати гілки для Dagger/Spear, коли з'являться свої кліпи
            };

            // float clipLength = _clipLengths.TryGetValue(attackHash, out var len) ? len : 1f;
            float clipLength = GetClipLength("Human_HitSword");
            // Debug.Log(clipLength);

            float animSpeed = attackTime > 0f ? clipLength / attackTime : 1f;
            // Debug.Log(animSpeed);
            _animator.SetFloat(AttackSpeedHash, animSpeed);
            PlayOnce(attackHash);
        }
        public float GetClipLength(string clipName)
        {
            RuntimeAnimatorController controller = _animator.runtimeAnimatorController;

            foreach (AnimationClip clip in controller.animationClips) {
                // Debug.Log($"Кліп: \"{clip.name}\"  |  Довжина: {clip.length}");
                if (clip.name == clipName) {
                    return clip.length;
                }
            }
            Debug.LogWarning($"No clip {clipName} found!");
            return 1.0f;
        }
        public void PlayIdle()
        {
            PlayLooping(IdleHash);
        }
        public void PlayWalk(float speed)
        {
            SetWalkSpeed(speed);
            PlayLooping(WalkHash);
        }
        public void PlayDeath()
        {
            PlayOnce(DeathHash);
        }
        private void PlayOnce(int stateHash)
        {
            _currentStateHash = 0;
            _animator.Play(stateHash, 0, 0f);
            _currentStateHash = stateHash;
        }
        private void PlayLooping(int stateHash)
        {
            if (_currentStateHash == stateHash) return;
            _animator.Play(stateHash, 0, float.NegativeInfinity);
            _currentStateHash = stateHash;
        }
    }
}