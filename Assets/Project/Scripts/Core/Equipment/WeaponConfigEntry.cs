// Core/Item/WeaponConfigEntry.cs
using System;

namespace Aegis.Core
{
    [Serializable]
    public class WeaponConfigEntry
    {
        public string id;
        public string display_name;
        public string animation;
        public string weapon_type;
        public float damage;
        public float attack_range;
        public float weight;
        public float armor_value;
        public float deflect_chance;
        public string[] effects;
    }

    [Serializable]
    public class WeaponConfigCollection
    {
        public WeaponConfigEntry[] items;
    }
}