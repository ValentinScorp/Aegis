using UnityEngine;

namespace Aegis.Core
{
    public static class WeaponGrip
    {
        public static bool IsTwoHanded(WeaponTypeId type) => type == WeaponTypeId.Bow;

        public static HandSocketId MainWeaponHandSlot(WeaponTypeId weaponType)
        {
            switch (weaponType) {
                case WeaponTypeId.Bow: return HandSocketId.Left;
                case WeaponTypeId.OneHandSword: return HandSocketId.Right;
                case WeaponTypeId.TwoHandSword: return HandSocketId.Right;
                default:
                    Debug.LogWarning($"[WeaponGrip] Unknown weaponId: {weaponType}");
                    return HandSocketId.Right;
            }
        }
    }
}