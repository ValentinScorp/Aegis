using System.Collections.Generic;
using Aegis.Core;
using UnityEngine;

namespace Aegis.View
{
    [CreateAssetMenu(fileName = "WeaponsAttachCatalog", menuName = "Aegis/Weapons Attach Config Catalog")]
    public class WeaponAttachCatalog : ScriptableObject
    {
        [SerializeField] private WeaponAttachConfig[] _configs;

        private Dictionary<WeaponTypeId, WeaponAttachConfig> _lookup;

        private void OnEnable()
        {
            _lookup = new Dictionary<WeaponTypeId, WeaponAttachConfig>();
            foreach (var cfg in _configs) {
                if (cfg == null) continue;
                if (!_lookup.TryAdd(cfg.WeaponType, cfg))
                    Debug.LogWarning($"[{nameof(WeaponAttachCatalog)}] Duplicate entry for WeaponTypeId '{cfg.WeaponType}' in {name}.", this);
            }
        }

        public WeaponAttachConfig GetConfig(WeaponTypeId typeId)
        {
            if (_lookup == null) OnEnable(); // на випадок виклику до OnEnable (напр. в edit mode)

            if (_lookup.TryGetValue(typeId, out var cfg))
                return cfg;

            Debug.LogWarning($"[{nameof(WeaponAttachCatalog)}] No WeaponAttachConfig found for WeaponTypeId '{typeId}'.");
            return null;
        }
    }
}