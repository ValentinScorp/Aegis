using System;
using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    [CreateAssetMenu(menuName = "Aegis/Unit Config Registry")]
    public class UnitConfigCatalog : ScriptableObject
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

