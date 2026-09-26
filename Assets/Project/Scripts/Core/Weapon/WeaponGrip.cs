using UnityEngine;

namespace Aegis.Core
{
    public static class WeaponGrip
    {
        public static bool IsTwoHanded(WeaponTypeId type) => type == WeaponTypeId.Bow;

        public static HandSocketId HandSlot(WeaponTypeId weaponType)
        {
            switch (weaponType) {
                case WeaponTypeId.Bow: return HandSocketId.Left;
                case WeaponTypeId.TwoHandSword: return HandSocketId.Right;
                default:
                    Debug.LogWarning($"[WeaponGrip] Unknown weaponId: {weaponType}");
                    return HandSocketId.Right;
            }
        }
    }
}