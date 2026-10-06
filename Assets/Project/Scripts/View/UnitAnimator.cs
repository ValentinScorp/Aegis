using System.Collections.Generic;
using Aegis.Core;
using Aegis.Utilities;
// using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Aegis.View
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private float _layerBlendSpeed = 6f;
        [SerializeField] private float _releaseSeconds = 0.25f;
        private Animator _animator;
        private Unit _unit;
        private int _currentStateHash;
        private float _upperWeight;
        private bool _wasAiming;
        private bool _releasing;
        private float _releaseTimer;
        private float _phaseTime;

        private static readonly int BowDrawHash = Animator.StringToHash("BowDraw");
        private static readonly int BowReleaseHash = Animator.StringToHash("BowRelease");
        private static readonly int DrawSpeedHash = Animator.StringToHash("DrawSpeed");
        private static readonly int ReleaseSpeedHash = Animator.StringToHash("ReleaseSpeed");

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
        public bool IsAimAnimationActive => _wasAiming || _releasing || _upperWeight > 0.001f;
        public void SetWalkSpeed(float speed) => _animator.SetFloat(WalkSpeedHash, speed);
        public float ReleaseSeconds => _releaseSeconds;

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
            _wasAiming = false;
            _releasing = false;
            _upperWeight = 0f;
            _animator.SetLayerWeight(_upperBodyLayer, 0f);
            PlayOnce(DeathHash);
        }
        public void UpdateAimAnimation(bool isAiming, bool cancelled, float dt)
        {
            float drawSeconds = Mathf.Max(0.05f, _unit != null ? _unit.BowDrawSeconds : 0.8f);

            if (isAiming) {
                if (!_wasAiming) {
                    _animator.SetFloat(DrawSpeedHash, GetClipLength("Bow_Draw") / drawSeconds);
                    _animator.Play(BowDrawHash, _upperBodyLayer, 0f);
                    _releasing = false;
                    _phaseTime = 0f;
                }
                _phaseTime += dt;
                _upperWeight = Mathf.MoveTowards(_upperWeight, 1f, dt * _layerBlendSpeed);
            } else {
                if (_wasAiming && !cancelled) {
                    _animator.SetFloat(ReleaseSpeedHash, GetClipLength("Bow_Release") / _releaseSeconds);
                    _animator.Play(BowReleaseHash, _upperBodyLayer, 0f);
                    _releasing = true;
                    _phaseTime = 0f;
                }
                if (_releasing) {
                    _phaseTime += dt;
                    float k = Mathf.Clamp01(_phaseTime / _releaseSeconds);
                } else {
                    _upperWeight = Mathf.MoveTowards(_upperWeight, 0f, dt * _layerBlendSpeed);
                }
            }
            _wasAiming = isAiming;
            _animator.SetLayerWeight(_upperBodyLayer, _upperWeight);
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