using System.Collections.Generic;

namespace Aegis.Core
{
    public static class WeaponAttackTimes
    {
        public static readonly Dictionary<WeaponType, float> Times = new() {
        { WeaponType.OneHandSword,  1.2f },
        { WeaponType.OneHandDagger, 0.7f },
        { WeaponType.OneHandSpear,  1.2f },
        { WeaponType.Bow,           1.5f },
        { WeaponType.Shield,        1.0f },
    };

        public static float Get(WeaponType type)
        {
            return Times.TryGetValue(type, out float time) ? time : 0.5f;
        }
    }
}