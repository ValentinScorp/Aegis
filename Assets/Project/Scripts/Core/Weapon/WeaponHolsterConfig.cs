using System;
using Aegis.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponHolsterConfig", menuName = "Weapon/Weapon Holster Config")]
public class WeaponHolsterConfig : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public WeaponTypeId WeaponId;
        public HolsterSocketId PrimaryMainWeaponHolster;
        public HolsterSocketId PrimaryOffWeaponHolster;
        public HolsterSocketId SecondaryMainWeaponHolster;
        public HolsterSocketId SecondaryOffWeaponHolster;
    }

    public Entry[] Items;
    public HolsterSocketId GetHolster(WeaponTypeId weaponId, WeaponSetId setId, WeaponRoleId roleId)
    {
        if (weaponId == WeaponTypeId.None) return HolsterSocketId.None;
        
        foreach (var item in Items) {
            if (item.WeaponId != weaponId) continue;

            if (setId == WeaponSetId.Primary) {
                if (roleId == WeaponRoleId.Main)
                    return item.PrimaryMainWeaponHolster;
                if (roleId == WeaponRoleId.Off) {
                    return item.PrimaryOffWeaponHolster;
                }
            }
            if (setId == WeaponSetId.Secondary) {
                if (roleId == WeaponRoleId.Main)
                    return item.SecondaryMainWeaponHolster;
                if (roleId == WeaponRoleId.Off) {
                    return item.SecondaryOffWeaponHolster;
                }
            }
        }
        Debug.LogWarning($"[{GetType().Name}] Can't find weapon holster config!", this);
        return HolsterSocketId.None;
    }
    // public bool TryGetHolsters(WeaponTypeId weaponId, WeaponRoleId roleId, out HolsterSocketId primary, out HolsterSocketId secondary)
    // {
    //     foreach (var item in Items) {
    //         if (item.WeaponId != weaponId) continue;
    //         if (roleId == WeaponRoleId.Main) {
    //             primary = item.PrimaryMainWeaponHolster;
    //             secondary = item.SecondaryMainWeaponHolster;
    //             return true;
    //         } else if (roleId == WeaponRoleId.Off) {
    //             primary = item.PrimaryOffWeaponHolster;
    //             secondary = item.SecondaryOffWeaponHolster;
    //             return true;
    //         }
    //     }
    //     Debug.LogWarning($"[{GetType().Name}] Can't find weapon holster config!", this);
    //     primary = HolsterSocketId.ShoulderLeft;
    //     secondary = HolsterSocketId.ShoulderRight;
    //     return false;
    // }
}
