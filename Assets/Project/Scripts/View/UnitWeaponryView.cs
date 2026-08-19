using System.Collections.Generic;
using System.Linq;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class UnitWeaponryView : MonoBehaviour
    {
        [SerializeField] private WeaponPrefabCatalog _weaponPrefabs;
        [SerializeField] private WeaponHolsterConfig _holsters;

        private Dictionary<WeaponSlotType, WeaponSlotView> _slots;
        private UnitWeaponry _weaponry;

        // інстанси по id зброї (живуть поки view живий)
        private readonly Dictionary<string, GameObject> _instances = new();

        // що зараз у якому слоті (щоб не шукати по ієрархії)
        private readonly Dictionary<WeaponSlotType, string> _slotOccupant = new();

        private void Awake()
        {
            _slots = GetComponentsInChildren<WeaponSlotView>().ToDictionary(s => s.SlotType);
        }

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
            void SpawnWeapon(WeaponConfig cfg)
            {
                if (cfg == null || string.IsNullOrEmpty(cfg.Id)) return;

                var prefab = _weaponPrefabs.GetPrefab(cfg.Id);
                if (prefab == null) return;

                var go = Instantiate(prefab, transform);
                go.name = cfg.Id;
                go.SetActive(false);
                _instances[cfg.Id] = go;
            }            
            
            SpawnWeapon(weaponry.Primary?.MainHand);
            SpawnWeapon(weaponry.Primary?.OffHand);
            SpawnWeapon(weaponry.Secondary?.MainHand);
            SpawnWeapon(weaponry.Secondary?.OffHand);
        }

        public void Refresh(UnitWeaponry weaponry)
        {
            if (weaponry == null) return;
            _weaponry = weaponry;

            DetachAllFromSlots();

            var occupied = new HashSet<WeaponSlotType>();
            var active = _weaponry.Active;
            var other = active ==_weaponry.Primary ? _weaponry.Secondary : _weaponry.Primary;

            // Active — в руках
            PlaceInSlot(active?.MainHand, WeaponSlotType.HandRight, occupied);
            PlaceInSlot(active?.OffHand,  WeaponSlotType.HandLeft,  occupied);

            // Інший сет — у holster
            PlaceHolstered(other?.MainHand, occupied);
            PlaceHolstered(other?.OffHand,  occupied);
        }

        /// <summary>Все з рук у holster (idle / sheathe).</summary>
        public void SheatheAll()
        {
            if (_weaponry == null) return;
            DetachAllFromSlots();

            var occupied = new HashSet<WeaponSlotType>();
            var w = _weaponry;
            PlaceHolstered(_weaponry.Primary?.MainHand, occupied);
            PlaceHolstered(_weaponry.Primary?.OffHand, occupied);
            PlaceHolstered(_weaponry.Secondary?.MainHand, occupied);
            PlaceHolstered(_weaponry.Secondary?.OffHand, occupied);
        }

        void PlaceHolstered(WeaponConfig cfg, HashSet<WeaponSlotType> occupied)
        {
            if (cfg == null) return;
            _holsters.TryGetHolsters(cfg.WeaponType, out var primary, out var secondary);
            var slot = !occupied.Contains(primary) ? primary : secondary;
            PlaceInSlot(cfg, slot, occupied);
        }

        void PlaceInSlot(WeaponConfig cfg, WeaponSlotType slotType, HashSet<WeaponSlotType> occupied)
        {
            if (cfg == null) return;
            if (!_instances.TryGetValue(cfg.Id, out var go)) return;
            if (!_slots.TryGetValue(slotType, out var slot)) return;

            // якщо слот зайнятий іншою зброєю — знімаємо
            if (_slotOccupant.TryGetValue(slotType, out var oldId) && oldId != cfg.Id)
                Detach(oldId);

            slot.Attach(go); // parent + local pose, SetActive(true)
            _slotOccupant[slotType] = cfg.Id;
            occupied.Add(slotType);
            go.SetActive(true);
        }

        void DetachAllFromSlots()
        {
            foreach (var id in _instances.Keys.ToList())
                Detach(id);
            _slotOccupant.Clear();
        }

        void Detach(string weaponId)
        {
            if (!_instances.TryGetValue(weaponId, out var go)) return;
            go.transform.SetParent(transform, false); // «склад» на юніті
            go.SetActive(false);

            // прибрати з _slotOccupant
            foreach (var kv in _slotOccupant.ToList())
                if (kv.Value == weaponId)
                    _slotOccupant.Remove(kv.Key);
        }

        void ClearAll()
        {
            DetachAllFromSlots();
            foreach (var go in _instances.Values)
                if (go != null) Destroy(go);
            _instances.Clear();
        }

        private void OnDestroy() => ClearAll();
    }
}