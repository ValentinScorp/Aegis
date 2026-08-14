using System.Collections.Generic;
using System.Linq;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    public class UnitEquipmentView : MonoBehaviour
    {
        [SerializeField] private WeaponPrefabCatalog _weaponPrefabs;
        [SerializeField] private WeaponHolsterConfig _holsters;

        private Dictionary<EquipmentSlotType, EquipmentSlotView> _slots;
        private Unit _unit;

        // інстанси по id зброї (живуть поки view живий)
        private readonly Dictionary<string, GameObject> _instances = new();

        // що зараз у якому слоті (щоб не шукати по ієрархії)
        private readonly Dictionary<EquipmentSlotType, string> _slotOccupant = new();

        private void Awake()
        {
            _slots = GetComponentsInChildren<EquipmentSlotView>().ToDictionary(s => s.SlotType);
        }

        public void Bind(Unit unit)
        {
            _unit = unit;
            EnsureInstances(unit);
            Refresh();
        }

        public void Unbind()
        {
            ClearAll();
            _unit = null;
        }

        /// <summary>Створити префаби для всієї зброї юніта один раз.</summary>
        void EnsureInstances(Unit unit)
        {
            void Ensure(WeaponConfig cfg)
            {
                if (cfg == null || string.IsNullOrEmpty(cfg.Id)) return;
                if (_instances.ContainsKey(cfg.Id)) return;

                var prefab = _weaponPrefabs.GetPrefab(cfg.Id);
                if (prefab == null) return;

                var go = Instantiate(prefab);
                go.name = cfg.Id;
                go.SetActive(false);
                _instances[cfg.Id] = go;
            }

            var w = unit.Weaponry;
            
            Ensure(w.Primary?.MainHand);
            Ensure(w.Primary?.OffHand);
            Ensure(w.Secondary?.MainHand);
            Ensure(w.Secondary?.OffHand);
        }

        public void Refresh()
        {
            if (_unit == null) return;

            DetachAllFromSlots();

            var occupied = new HashSet<EquipmentSlotType>();
            var w = _unit.Weaponry;
            var active = w.Active;
            var other = active == w.Primary ? w.Secondary : w.Primary;

            // Active — в руках
            PlaceInSlot(active?.MainHand, EquipmentSlotType.HandRight, occupied);
            PlaceInSlot(active?.OffHand,  EquipmentSlotType.HandLeft,  occupied);

            // Інший сет — у holster
            PlaceHolstered(other?.MainHand, occupied);
            PlaceHolstered(other?.OffHand,  occupied);
        }

        /// <summary>Все з рук у holster (idle / sheathe).</summary>
        public void SheatheAll()
        {
            if (_unit == null) return;
            DetachAllFromSlots();

            var occupied = new HashSet<EquipmentSlotType>();
            var w = _unit.Weaponry;
            PlaceHolstered(w.Primary?.MainHand, occupied);
            PlaceHolstered(w.Primary?.OffHand, occupied);
            PlaceHolstered(w.Secondary?.MainHand, occupied);
            PlaceHolstered(w.Secondary?.OffHand, occupied);
        }

        void PlaceHolstered(WeaponConfig cfg, HashSet<EquipmentSlotType> occupied)
        {
            if (cfg == null) return;
            _holsters.TryGetHolsters(cfg.WeaponType, out var primary, out var secondary);
            var slot = !occupied.Contains(primary) ? primary : secondary;
            PlaceInSlot(cfg, slot, occupied);
        }

        void PlaceInSlot(WeaponConfig cfg, EquipmentSlotType slotType, HashSet<EquipmentSlotType> occupied)
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