using System.Collections.Generic;

namespace Aegis.Core
{
    public static class WeaponAttackTimes
    {
        public static readonly Dictionary<WeaponTypeId, float> Times = new() {
        { WeaponTypeId.OneHandSword,  1.2f },
        { WeaponTypeId.OneHandDagger, 0.7f },
        { WeaponTypeId.OneHandSpear,  1.2f },
        { WeaponTypeId.Bow,           1.5f },
        { WeaponTypeId.Shield,        1.0f },
    };

        public static float Get(WeaponTypeId weaponType)
        {
            return Times.TryGetValue(weaponType, out float time) ? time : 0.5f;
        }
    }
}