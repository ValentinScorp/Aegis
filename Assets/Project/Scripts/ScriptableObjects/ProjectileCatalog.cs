using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.View
{
    [CreateAssetMenu(menuName = "Aegis/Projectile Catalog")]
    public class ProjectileCatalog : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string Id;
            public ProjectileView Prefab;
            // public int PoolSize;
        }

        public Entry[] Items;

        private Dictionary<string, ProjectileView> _prefabMap;

        public ProjectileView GetPrefab(string id)
        {
            foreach(var item in Items) {
                if (id == item.Id) {
                    return item.Prefab;
                }
            }
            return null;
        }
    }
}