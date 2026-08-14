using System;
using UnityEngine;

namespace Aegis.View
{
    [CreateAssetMenu(menuName = "Aegis/Weapon Prefab Catalog")]
    public class WeaponPrefabCatalog : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string Id;          // sword_iron, bow_hunter
            public GameObject Prefab;
        }
        public Entry[] Items;

        public GameObject GetPrefab(string id)
        {
            foreach (var item in Items) {
                if (id == item.Id) {
                    return item.Prefab;
                }
            }
            return null;
        }
    }
}