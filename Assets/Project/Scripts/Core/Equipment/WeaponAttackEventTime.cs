using System.Collections.Generic;

namespace Aegis.Core
{
    public static class WeaponAttackEventTimes
    {
        public static readonly Dictionary<WeaponType, float> Times = new() {
        { WeaponType.OneHandSword,  0.3f },
        { WeaponType.OneHandDagger, 0.3f },
        { WeaponType.OneHandSpear,  0.3f },
        { WeaponType.Bow,           0.7f },
        { WeaponType.Shield,        1.0f },
    };

        public static float Get(WeaponType type)
        {
            return Times.TryGetValue(type, out float time) ? time : 0.5f;
        }
    }
}