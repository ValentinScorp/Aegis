using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitWeaponry
    {
        public readonly WeaponSet Primary;
        public readonly WeaponSet Secondary;
        public WeaponSet Active { get; private set; }

        public UnitWeaponry(WeaponSet primary, WeaponSet secondary)
        {
            Primary = primary;
            Secondary = secondary;
            Active = primary;
        }

        public void SetActive(WeaponSet set) => Active = set;

        public bool HasAnyRanged => Primary.IsRanged || Secondary.IsRanged;
        public WeaponSet GetClosestWeaponSet(float distance)
        {
            float primary = Primary.GetLongestMeleeAttackRange();
            float secondary = Secondary.GetLongestMeleeAttackRange();
            if (primary > secondary) {
                return Secondary;
            }
            return Primary;
        }
        public float GetLongestAttackDistance()
        {
            float primary = Primary.GetLongestMeleeAttackRange();
            float secondary = Secondary.GetLongestMeleeAttackRange();
            if (primary > secondary) {
                return primary;
            }
            return secondary;
        }
    }
}