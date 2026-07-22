
using UnityEngine;

namespace Aegis.View
{
    public class EquipmentSlotView : MonoBehaviour
    {
        [SerializeField] public EquipmentSlotType Type;
        [SerializeField] private Transform handSocket;
        private GameObject _currentWeaponInstance;
        private EntityAnimator _weaponAnimator;

        private static readonly Quaternion BowRotationOffset = Quaternion.Euler(90f, 0f, 0f);

        private void Awake()
        {
        }
        public void EquipWeapon(GameObject weaponPrefab)
        {
            if (_currentWeaponInstance != null)
                Destroy(_currentWeaponInstance);

            _currentWeaponInstance = Instantiate(weaponPrefab, handSocket);
            _currentWeaponInstance.transform.localPosition = Vector3.zero;
            _currentWeaponInstance.transform.localRotation = BowRotationOffset;

            _weaponAnimator = _currentWeaponInstance.GetComponent<EntityAnimator>();
        }

        public void PlayBowShoot()
        {
            if (_weaponAnimator != null)
                _weaponAnimator.PlayShoot(1.0f);
        }
    }
}