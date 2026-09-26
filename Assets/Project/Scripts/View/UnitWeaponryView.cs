using System.Collections.Generic;
using System.Linq;
using Aegis.Core;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

namespace Aegis.View
{
    public class UnitWeaponryView : MonoBehaviour
    {
        [SerializeField] private WeaponPrefabCatalog _weaponPrefabCatalog;
        [SerializeField] private WeaponHolsterConfig _holsterConfig;
        private Dictionary<HandSocketId, HandSocketView> _handSockets;
        private Dictionary<HolsterSocketId, HolsterSocketView> _holsterSockets;
        private UnitWeaponry _weaponry;
        private Dictionary<WeaponEquipId, GameObject> _weaponInstances = new();

        private void Awake()
        {
            _handSockets = GetComponentsInChildren<HandSocketView>().ToDictionary(s => s.Id);
            _holsterSockets = GetComponentsInChildren<HolsterSocketView>().ToDictionary(s => s.Id);
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
            if (cfg == null || string.IsNullOrEmpty(cfg.Id)) return;

            var prefab = _weaponPrefabCatalog.GetPrefab(cfg.Id);
            if (prefab == null) return;

            var go = Instantiate(prefab, transform);
            go.name = cfg.Id;
            go.SetActive(false);
            _weaponInstances[equipId] = go;
        }
        public void Refresh(UnitWeaponry weaponry)
        {
            if (weaponry == null) {
                Debug.LogError($"[{nameof(UnitWeaponryView)}] Refresh called with null weaponry on {name}.");
                return;
            }
            _weaponry = weaponry;

            DetachAllFromSlots();

            bool handPrimary = false;
            bool handSecondary = false;

            if (!_weaponry.IsHolstered) {
                switch (_weaponry.ActiveSet) {
                    case WeaponSetId.Primary:
                        handPrimary = true;
                        break;
                    case WeaponSetId.Secondary:
                        handSecondary = true;
                        break;
                    default:
                        Debug.LogError($"[{nameof(UnitWeaponryView)}] Unhandled ActiveSet '{_weaponry.ActiveSet}' on {name}.");
                        break;
                }
            }
            PlaceSet(WeaponSetId.Primary, hand: handPrimary);
            PlaceSet(WeaponSetId.Secondary, hand: handSecondary);
        }
        private void PlaceSet(WeaponSetId set, bool hand)
        {
            var mainWeapon = _weaponry.GetWeaponType(set, WeaponRoleId.Main);
            var offWeapon = _weaponry.GetWeaponType(set, WeaponRoleId.Off);

            var mainEquip = set == WeaponSetId.Primary ? WeaponEquipId.PrimaryMain : WeaponEquipId.SecondaryMain;
            var offEquip = set == WeaponSetId.Primary ? WeaponEquipId.PrimaryOff : WeaponEquipId.SecondaryOff;

            if (hand) {
                var mainHand = WeaponGrip.HandSlot(mainWeapon);
                TryPlaceHand(mainEquip, mainHand);
                TryPlaceHand(offEquip, HandSocketId.Left);
            } else {
                var mainHolster = _holsterConfig.GetHolster(mainWeapon, set, WeaponRoleId.Main);
                var offHolster = _holsterConfig.GetHolster(offWeapon, set, WeaponRoleId.Off);
                TryPlaceHolster(mainEquip, mainHolster);
                TryPlaceHolster(offEquip, offHolster);
            }
        }
        private void TryPlaceHolster(WeaponEquipId equipId, HolsterSocketId holsterId)
        {
            if (!_weaponInstances.TryGetValue(equipId, out var instance)) return; // немає зброї в слоті — нема що ставити, це норма
            if (holsterId == HolsterSocketId.None) return; // холстер навмисно не потрібен

            if (!_holsterSockets.TryGetValue(holsterId, out var socket)) {
                Debug.LogWarning($"[{nameof(UnitWeaponryView)}] Holster socket '{holsterId}' not found on {name}.");
                return;
            }
            PlaceInSlot(instance, socket);
        }

        private void TryPlaceHand(WeaponEquipId equipId, HandSocketId handId)
        {
            if (!_weaponInstances.TryGetValue(equipId, out var instance)) return;

            if (!_handSockets.TryGetValue(handId, out var socket)) {
                Debug.LogWarning($"[{nameof(UnitWeaponryView)}] Hand socket '{handId}' not found on {name}.");
                return;
            }
            PlaceInSlot(instance, socket);
        }
        private void PlaceInSlot(GameObject weaponInst, WeaponSocketView holsterSocket)
        {
            weaponInst.transform.SetParent(holsterSocket.transform, false);
        }
        void DetachAllFromSlots()
        {
            foreach (var id in _weaponInstances.Keys.ToList())
                Detach(id);
        }
        void Detach(WeaponEquipId equipId)
        {
            if (!_weaponInstances.TryGetValue(equipId, out var go)) return;
            go.transform.SetParent(transform, false);
            go.SetActive(false);
        }

        private void ClearAll()
        {
            DetachAllFromSlots();
            foreach (var go in _weaponInstances.Values)
                if (go != null) Destroy(go);
            _weaponInstances.Clear();
        }
    }
}