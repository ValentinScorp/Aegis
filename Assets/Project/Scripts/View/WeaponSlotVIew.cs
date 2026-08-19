
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class WeaponSlotView : MonoBehaviour
    {
        [SerializeField] private WeaponSlotType _socketType;
        [SerializeField] private Transform _socket;
        private GameObject _currentWeaponInstance;
        private GameObject _equippedPrefab;

        public WeaponSlotType SlotType { get; private set; }
        private static readonly Quaternion BowRotationOffset = Quaternion.Euler(90f, 0f, 0f);

        private void Awake()
        {
        }
        public void Attach(GameObject instance)
        {
            if (instance == null || _socket == null) return;
            instance.transform.SetParent(_socket, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.SetActive(true);
        }
        public void EquipWeapon(GameObject weaponPrefab)
        {
            if (weaponPrefab == null || _socket == null) return;
            if (_equippedPrefab == weaponPrefab && _currentWeaponInstance != null) return;

            UnequipWeapon();

            _currentWeaponInstance = Instantiate(weaponPrefab, _socket);
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