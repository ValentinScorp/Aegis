using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private WeaponTypeId _typeId;
        [SerializeField] private Animator _animator;
        private static readonly int ShootNameHash = Animator.StringToHash("Shoot");
        public WeaponTypeId WeaponType => _typeId;
        private void Awake()
        {
            if (_animator == null)
                Debug.LogWarning($"[{nameof(WeaponView)}] Animator not assigned on {name}.", this);
        }
        public void PlayShootAnimation(float animSpeed)
        {
            if (_animator == null || animSpeed <= 0.01f) return;

            _animator.speed = animSpeed;
            _animator.Play(ShootNameHash, 0, 0f);
        }
    }
}
