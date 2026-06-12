using System;
using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    [CreateAssetMenu(menuName = "Aegis/Entity View Registry")]
    public class EntityViewRegistry : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public EntityType Type;
            public EntityView Prefab;
        }

        [SerializeField] private Entry[] _entries;

        private Dictionary<EntityType, EntityView> _map;

        private void OnEnable()
        {
            _map = new();
            foreach (var e in _entries)
                _map[e.Type] = e.Prefab;
        }

        public EntityView GetPrefab(EntityType type)
        {
            if (_map.TryGetValue(type, out var prefab)) return prefab;
            Debug.LogWarning($"No prefab registered for EntityType: {type}");
            return null;
        }
    }
}