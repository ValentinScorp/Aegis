using System.Collections.Generic;
using System.Linq;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class UnitWeaponryView : MonoBehaviour
    {
        [SerializeField] private WeaponPrefabCatalog _weaponPrefabCatalog;
        [SerializeField] private WeaponAttachCatalog _weaponsAttachCatalog;
        [SerializeField] private WeaponHolsterConfig _holsterConfig;
        private Dictionary<HandSocketId, HandSocketView> _handSockets;
        private Dictionary<HolsterSocketId, HolsterSocketView> _holsterSockets;
        private UnitWeaponry _weaponry;
        private Dictionary<WeaponEquipId, WeaponView> _weaponInstances = new();

        private void Awake()
        {
            _handSockets = GetComponentsInChildren<HandSocketView>(true).ToDictionary(s => s.Id);
            _holsterSockets = GetComponentsInChildren<HolsterSocketView>(true).ToDictionary(s => s.Id);

            if (_weaponPrefabCatalog == null)
                Debug.LogError($"[{nameof(UnitWeaponryView)}] WeaponPrefabCatalog not assigned on {name}!", this);

            if (_holsterConfig == null)
                Debug.LogError($"[{nameof(UnitWeaponryView)}] WeaponHolsterConfig not assigned on {name}!", this);
        }
        private void OnDestroy() => ClearAll();

        public void Bind(UnitWeaponry weaponry)
        {
            if (weaponry is null) return;
            _weaponry = weaponry;
            SpawnWeaponInstances(_weaponry);
            Refresh(_weaponry);
            _weaponry.Changed += Refresh;
        }
        public void Unbind()
        {
            ClearAll();
            _weaponry = null;
        }
        void SpawnWeaponInstances(UnitWeaponry weaponry)
        {
            SpawnWeapon(weaponry.WeaponSets[WeaponSetId.Primary].MainWeapon, WeaponEquipId.PrimaryMain);
            SpawnWeapon(weaponry.WeaponSets[WeaponSetId.Primary].OffWeapon, WeaponEquipId.PrimaryOff);
            SpawnWeapon(weaponry.WeaponSets[WeaponSetId.Secondary].MainWeapon, WeaponEquipId.SecondaryMain);
            SpawnWeapon(weaponry.WeaponSets[WeaponSetId.Secondary].OffWeapon, WeaponEquipId.SecondaryOff);
        }
        private void SpawnWeapon(WeaponConfig cfg, WeaponEquipId equipId)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id)) {
                // Debug.LogWarning($"[{nameof(UnitWeaponryView)}] No WeaponConfig/Id for slot '{equipId}' on {name}.");
                return;
            }
            var prefab = _weaponPrefabCatalog.GetPrefab(cfg.Id);
            if (prefab == null) {
                // Debug.LogWarning($"[{nameof(UnitWeaponryView)}] Prefab for '{cfg.Id}' not found in catalog (slot '{equipId}') on {name}.");
                return;
            }

            var go = Instantiate(prefab, transform);
            go.name = cfg.Id;
            go.SetActive(false);

            var weaponView = go.GetComponent<WeaponView>();
            if (weaponView == null) {
                Debug.LogError($"[{nameof(UnitWeaponryView)}] Prefab '{cfg.Id}' has no WeaponView component!", go);
                Destroy(go);
                return;
            }
            _weaponInstances[equipId] = weaponView;
        }
        public void Refresh(UnitWeaponry weaponry)
        {
            if (weaponry == null) {
                Debug.LogError($"[{nameof(UnitWeaponryView)}] Refresh called with null weaponry on {name}.");
                return;
            }
            _weaponry = weaponry;

            DetachAllFromSockets();

            bool unholsterPrim = false;
            bool unholsterSec = false;

            if (!_weaponry.IsHolstered) {
                switch (_weaponry.ActiveSet) {
                    case WeaponSetId.Primary: unholsterPrim = true; break;
                    case WeaponSetId.Secondary: unholsterSec = true; break;
                    default:
                        Debug.LogError($"[{nameof(UnitWeaponryView)}] Unhandled ActiveSet '{_weaponry.ActiveSet}' on {name}.");
                        break;
                }
            }
            PlaceSet(WeaponSetId.Primary, unholsterPrim);
            PlaceSet(WeaponSetId.Secondary, unholsterSec);
        }
        private void PlaceSet(WeaponSetId set, bool unholstered)
        {
            var mainWeapon = _weaponry.GetWeaponType(set, WeaponRoleId.Main);
            var offWeapon = _weaponry.GetWeaponType(set, WeaponRoleId.Off);

            var mainEquip = set == WeaponSetId.Primary ? WeaponEquipId.PrimaryMain : WeaponEquipId.SecondaryMain;
            var offEquip = set == WeaponSetId.Primary ? WeaponEquipId.PrimaryOff : WeaponEquipId.SecondaryOff;

            if (unholstered) {
                var mainHand = WeaponGrip.MainWeaponHandSlot(mainWeapon);
                TryPlaceHand(mainEquip, mainHand);
                TryPlaceHand(offEquip, HandSocketId.Left);
            } else {
                var mainHolster = _holsterConfig.GetHolster(mainWeapon, WeaponSetId.Primary, WeaponRoleId.Main);
                var offHolster = _holsterConfig.GetHolster(offWeapon, WeaponSetId.Primary, WeaponRoleId.Off);
                if (set == WeaponSetId.Secondary) {
                    mainHolster = _holsterConfig.GetHolster(mainWeapon, set, WeaponRoleId.Main, mainHolster);
                    offHolster = _holsterConfig.GetHolster(offWeapon, set, WeaponRoleId.Off, offHolster);
                }
                TryPlaceHolster(mainEquip, mainHolster);
                TryPlaceHolster(offEquip, offHolster);
            }
        }
        public WeaponView GetActiveHandWeapon()
        {
            if (_weaponry == null) return null;

            var equipId = _weaponry.ActiveSet == WeaponSetId.Primary
                ? WeaponEquipId.PrimaryMain
                : WeaponEquipId.SecondaryMain;

            return _weaponInstances.TryGetValue(equipId, out var view) ? view : null;
        }
        private void TryPlaceHolster(WeaponEquipId equipId, HolsterSocketId holsterId)
        {
            if (!_weaponInstances.TryGetValue(equipId, out var instance)) return; // немає зброї в слоті — нема що ставити, це норма
            if (holsterId == HolsterSocketId.None) return; // холстер навмисно не потрібен

            if (!_holsterSockets.TryGetValue(holsterId, out var socket)) {
                Debug.LogWarning($"[{nameof(UnitWeaponryView)}] Holster socket '{holsterId}' not found on {name}.");
                return;
            }
            socket.AttachWeapon(instance, _weaponsAttachCatalog);
        }

        private void TryPlaceHand(WeaponEquipId equipId, HandSocketId handId)
        {
            if (!_weaponInstances.TryGetValue(equipId, out var instance)) {
                // Debug.LogWarning($"[{nameof(UnitWeaponryView)}] Weapon instance of '{equipId}' not found in [_weaponInstances].");
                return;
            }

            if (!_handSockets.TryGetValue(handId, out var socket)) {
                // Debug.LogWarning($"[{nameof(UnitWeaponryView)}] Hand socket '{handId}' not found on {name}.");
                return;
            }
            socket.AttachWeapon(instance, _weaponsAttachCatalog);
        }
        void DetachAllFromSockets()
        {
            foreach (var socket in _handSockets.Values)
                DetachFromSocket(socket);
            foreach (var socket in _holsterSockets.Values)
                DetachFromSocket(socket);
        }
        void DetachFromSocket(WeaponSocketView socket)
        {
            var weapon = socket.UnattachWeapon();
            if (weapon == null) return;

            weapon.transform.SetParent(transform, false);
            weapon.gameObject.SetActive(false);
        }

        private void ClearAll()
        {
            DetachAllFromSockets();
            foreach (var view in _weaponInstances.Values)
                if (view != null) Destroy(view.gameObject);
            _weaponInstances.Clear();
        }
    }
}