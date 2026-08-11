using System;
using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    [CreateAssetMenu(menuName = "Aegis/Unit Config Registry")]
    public class UnitConfigRegistry : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public UnitType Type;
            public UnitConfig Config;
        }
        [SerializeField] private Entry[] _entries;

        internal UnitConfig GetConfig(UnitType type)
        {
            foreach(var e in _entries) {
                if (e.Type == type) {
                    return e.Config;
                }
            }
            return null;
        }
    }
}


    // [CreateAssetMenu(menuName = "Aegis/Entity View Registry")]
    // public class EntityViewRegistry : ScriptableObject
    // {
    // [Serializable]
    // public struct Entry
    // {
    //     public EntityType Type;
    //     public EntityView Prefab;
    //     public UnitConfig Config;
    // }

    // [SerializeField] private Entry[] _entries;

    // private Dictionary<EntityType, EntityView> _prefabMap;
    // private Dictionary<EntityType, UnitConfig> _configMap;

    // private void OnEnable()
    // {
    //     _prefabMap = new();
    //     _configMap = new();

    //     foreach (var e in _entries) {
    //         _prefabMap[e.Type] = e.Prefab;
    //         if (e.Config != null)
    //             _configMap[e.Type] = e.Config;
    //     }
    // }

    // public EntityView GetPrefab(EntityType type)
    // {
    //     if (_prefabMap.TryGetValue(type, out var prefab)) return prefab;
    //     Debug.LogWarning($"No prefab for EntityType: {type}");
    //     return null;
    // }

    // public UnitConfig GetConfig(EntityType type)
    // {
    //     if (_configMap.TryGetValue(type, out var config)) return config;
    //     Debug.LogWarning($"No config for EntityType: {type}");
    //     return null;
    // }
    // }
