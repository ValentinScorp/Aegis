using System.Collections.Generic;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class WeaponSocketView : MonoBehaviour
    {
        [SerializeField] protected Transform _socket;
        private GameObject _equippedWeapon;

        public Transform Socket => _socket != null ? _socket : transform;
        public bool IsOccupied => _equippedWeapon != null;
        public GameObject EquippedWeapon => _equippedWeapon;

        private void Awake()
        {
        }
        public void AttachWeapon(GameObject instance)
        {
            if (instance == null || _socket == null) return;

            if (_equippedWeapon == instance) {
                Debug.LogWarning("[WeaponSocketView] Trying to attach same weapon to occupied slot! Ignoring AttachWeapon!");
                return;
            }
            if (_equippedWeapon != null) {
                Debug.LogWarning("[WeaponSocketView] Trying to attach weapon to occupied slot! Ignoring AttachWeapon!");
                return;
            }
            _equippedWeapon = instance;

            instance.transform.SetParent(_socket, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.SetActive(true);
        }

        public GameObject UnattachWeapon()
        {
            if (_equippedWeapon == null) return null;

            var weapon = _equippedWeapon;
            weapon.transform.SetParent(null, true);
            _equippedWeapon = null;
            return weapon;
        }
    }
}