using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private WeaponTypeId _typeId;
        [SerializeField] private Animator _animator;

        private static readonly int DrawHash = Animator.StringToHash("Draw");
        private static readonly int ReleaseHash = Animator.StringToHash("Release");
        private static readonly int IdleHash = Animator.StringToHash("Idle");


        private static readonly int ShootNameHash = Animator.StringToHash("Shoot");
        public WeaponTypeId WeaponType => _typeId;
        private void Awake()
        {
            if (_animator == null)
                Debug.LogWarning($"[{nameof(WeaponView)}] Animator not assigned on {name}.", this);
        }
        // public void PlayShootAnimation(float animSpeed)
        // {
        //     if (_animator == null || animSpeed <= 0.01f) return;

        //     _animator.speed = animSpeed;
        //     _animator.Play(ShootNameHash, 0, 0f);
        // }
        private void PlayState(int hash, string clipName, float seconds)
        {
            if (_animator == null) return;
            _animator.speed = GetClipLength(clipName) / Mathf.Max(0.05f, seconds);
            _animator.Play(hash, 0, 0f);
        }
        public void PlayDraw(float seconds) => PlayState(DrawHash, "Prop_Bow_Draw", seconds);
        public void PlayRelease(float seconds) => PlayState(ReleaseHash, "Prop_Bow_Release", seconds);
        public void PlayIdle()
        {
            if (_animator == null) return;
            _animator.speed = 1f;
            _animator.Play(IdleHash, 0, 0f);
        }
        public void SetNormalizedTime(float t)
        {
            if (_animator == null) return;
            _animator.speed = 0f;                       // кліп тільки скролимо вручну
            _animator.Play(ShootNameHash, 0, t);
        }
        private float GetClipLength(string clipName)
        {
            foreach (var clip in _animator.runtimeAnimatorController.animationClips)
                if (clip.name == clipName) return clip.length;
            Debug.LogWarning($"No clip {clipName} on {name}");
            return 1f;
        }
    }
}
