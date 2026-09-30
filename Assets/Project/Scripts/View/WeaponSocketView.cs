using UnityEngine;

namespace Aegis.View
{
    public abstract class WeaponSocketView : MonoBehaviour
    {
        [SerializeField] protected Transform _socket;

        private WeaponView _equippedWeapon;

        public Transform Socket => _socket != null ? _socket : transform;
        public WeaponView EquippedWeapon => _equippedWeapon;
        public bool IsOccupied => _equippedWeapon != null;

        protected abstract (Vector3 pos, Quaternion rot) GetAttachOffset(WeaponAttachConfig config);

        public void AttachWeapon(WeaponView instance, WeaponAttachCatalog attachCatalog)
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

            var attachConfig = attachCatalog != null ? attachCatalog.GetConfig(instance.WeaponType) : null;
            var (pos, rot) = attachConfig != null
                ? GetAttachOffset(attachConfig)
                : (Vector3.zero, Quaternion.identity);

            instance.transform.SetParent(_socket, false);
            instance.transform.localPosition = pos;
            instance.transform.localRotation = rot;
            instance.gameObject.SetActive(true);
        }

        public WeaponView UnattachWeapon()
        {
            if (_equippedWeapon == null) return null;

            var weapon = _equippedWeapon;
            weapon.transform.SetParent(null, true);
            _equippedWeapon = null;
            return weapon;
        }
    }
}