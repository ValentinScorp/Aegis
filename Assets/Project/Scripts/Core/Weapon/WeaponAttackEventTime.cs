using System.Collections.Generic;

namespace Aegis.Core
{
    public static class WeaponAttackEventTimes
    {
        public static readonly Dictionary<WeaponTypeId, float> Times = new() {
        { WeaponTypeId.OneHandSword,  0.3f },
        { WeaponTypeId.OneHandDagger, 0.3f },
        { WeaponTypeId.OneHandSpear,  0.3f },
        { WeaponTypeId.Bow,           0.7f },
        { WeaponTypeId.Shield,        1.0f },
    };

        public static float Get(WeaponTypeId weaponType)
        {
            return Times.TryGetValue(weaponType, out float time) ? time : 0.5f;
        }
    }
}