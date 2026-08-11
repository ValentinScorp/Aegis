
using UnityEngine;

namespace Aegis.View
{
    public class EquipmentSlotView : MonoBehaviour
    {
        [SerializeField] public EquipmentSlotType Type;
        [SerializeField] private Transform handSocket;
        private GameObject _currentWeaponInstance;
        private GameObject _equippedPrefab;

        private static readonly Quaternion BowRotationOffset = Quaternion.Euler(90f, 0f, 0f);

        private void Awake()
        {
        }
        public void EquipWeapon(GameObject weaponPrefab)
        {
            if (weaponPrefab == null || handSocket == null) return;
            if (_equippedPrefab == weaponPrefab && _currentWeaponInstance != null) return; 

            UnequipWeapon();

            _currentWeaponInstance = Instantiate(weaponPrefab, handSocket);
            _currentWeaponInstance.transform.localPosition = Vector3.zero;
            _currentWeaponInstance.transform.localRotation = Quaternion.identity; // BowRotationOffset
            _equippedPrefab = weaponPrefab;
        }

        public void UnequipWeapon()
        {
            if (_currentWeaponInstance == null) return;
            Destroy(_currentWeaponInstance);
            _currentWeaponInstance = null;
            _equippedPrefab = null;
        }
    }
}