using System.Collections.Generic;
using Aegis.Core;
using Aegis.Utilities;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Aegis.View
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private ClipAction _swordHitClip;
        [SerializeField, Range(0f, 1f)] private float _bowDrawNormalizedTime = 0.45f; // підберіть на око: кадр, де тятива натягнута
        [SerializeField] private float _drawSpeed = 2.5f;   // швидкість фази натягу (1/сек по normalizedTime)
        [SerializeField] private float _releaseSpeed = 3.5f; // швидкість фази спуску
        [SerializeField] private float _layerBlendSpeed = 6f;
        private Animator _animator;
        private Unit _unit;
        private int _currentStateHash;
        private float _upperWeight;
        private float _bowTime;
        private bool _upperActive;

        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int SwordAttackHash = Animator.StringToHash("SwordAttack");
        private static readonly int BowShootHash = Animator.StringToHash("BowShoot");
        private static readonly int WalkHash = Animator.StringToHash("Walk");

        private static readonly int WalkSpeedHash = Animator.StringToHash("WalkSpeed");
        private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

        private static readonly int BowShootUpperHash = Animator.StringToHash("BowShoot");
        private static string UpperBodyLayerName = "UpperBody";
        private int _upperBodyLayer = -1;

        private readonly Dictionary<int, float> _clipLengths = new();

        public bool IsWalking => _currentStateHash == WalkHash;
        public void SetWalkSpeed(float speed) => _animator.SetFloat(WalkSpeedHash, speed);

        private void Awake()
        {
            _animator = ComponentResolver.Require(this, GetComponentInChildren<Animator>());
            _upperBodyLayer = _animator.GetLayerIndex(UpperBodyLayerName);
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
            foreach (var clip in _animator.runtimeAnimatorController.animationClips) {
                _clipLengths[Animator.StringToHash(clip.name)] = clip.length;
            }
        }
        public float PlayAttack(string weaponAnimation, float attackTime)
        {
            var anim = WeaponAnimationCatalog.Get(weaponAnimation);
            int clipHash = Animator.StringToHash(anim.ClipName);
            float clipLength = _clipLengths.TryGetValue(clipHash, out var len) ? len : GetClipLength(anim.ClipName);

            float animSpeed = attackTime > 0f ? clipLength / attackTime : 1f;
            _animator.SetFloat(AttackSpeedHash, animSpeed);
            PlayOnce(anim.StateHash);
            return animSpeed;
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
            // Debug.Log($"PlayIdle called, current={_currentStateHash}, target={IdleHash}");
            PlayLooping(IdleHash);
        }
        public void PlayWalk(float speed)
        {
            SetWalkSpeed(speed);
            // Debug.Log($"PlayWalk called, current={_currentStateHash}, target={WalkHash}");
            PlayLooping(WalkHash);
        }
        public void PlayDeath()
        {
            PlayOnce(DeathHash);
        }
        public void UpdateAimAnimation(bool isAiming, bool cancelled, float dt)
        {
            if (isAiming) {
                if (!_upperActive) _bowTime = 0f;          // кожен натяг починається з нуля
                _upperActive = true;
                _upperWeight = Mathf.MoveTowards(_upperWeight, 1f, dt * _layerBlendSpeed);

                float drawSeconds = Mathf.Max(0.05f, _unit != null ? _unit.BowDrawSeconds : 0.8f);
                float drawSpeed = _bowDrawNormalizedTime / drawSeconds;
                _bowTime = Mathf.MoveTowards(_bowTime, _bowDrawNormalizedTime, dt * drawSpeed);
            } else if (_upperActive) {
                if (cancelled) {
                    // скасований натяг: тятива повертається назад, без анімації пострілу
                    _bowTime = Mathf.MoveTowards(_bowTime, 0f, dt * _releaseSpeed);
                    if (_bowTime <= 0f) _upperActive = false;
                } else {
                    _bowTime += dt * _releaseSpeed;
                    if (_bowTime >= 1f) { _bowTime = 1f; _upperActive = false; }
                }
            } else {
                _upperWeight = Mathf.MoveTowards(_upperWeight, 0f, dt * _layerBlendSpeed);
            }

            _animator.SetLayerWeight(_upperBodyLayer, _upperWeight);
            if (_upperWeight > 0.001f)
                _animator.Play(BowShootUpperHash, _upperBodyLayer, _bowTime);
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