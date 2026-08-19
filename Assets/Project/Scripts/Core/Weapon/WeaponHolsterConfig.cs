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
        public WeaponSlotType PrimaryHolster;
        public WeaponSlotType SecondaryHolster;
    }

    public Entry[] Items;

    public bool TryGetHolsters(WeaponType type, out WeaponSlotType primary, out WeaponSlotType secondary)
    {
        foreach (var e in Items)
        {
            if (e.WeaponType != type) continue;
            primary = e.PrimaryHolster;
            secondary = e.SecondaryHolster;
            return true;
        }

        primary = WeaponSlotType.HipRight;
        secondary = WeaponSlotType.HipLeft;
        return false;
    }
}
