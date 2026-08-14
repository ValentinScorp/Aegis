using System;
using Aegis.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponHolsterConfig", menuName = "Weapon/Weapon Holster Config")]
public class WeaponHolsterConfig : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public WeaponType WeaponType;
        public EquipmentSlotType PrimaryHolster;
        public EquipmentSlotType SecondaryHolster;
    }

    public Entry[] Items;

    public bool TryGetHolsters(WeaponType type, out EquipmentSlotType primary, out EquipmentSlotType secondary)
    {
        foreach (var e in Items)
        {
            if (e.WeaponType != type) continue;
            primary = e.PrimaryHolster;
            secondary = e.SecondaryHolster;
            return true;
        }

        primary = EquipmentSlotType.HipRight;
        secondary = EquipmentSlotType.HipLeft;
        return false;
    }
}
