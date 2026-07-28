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
        public float GetAttackRange()
        {
            return Active.GetAttackRange();
        }
        public WeaponSet GetClosestWeaponSet()
        {
            float primary = Primary.GetAttackRange();
            float secondary = Secondary.GetAttackRange();
            if (primary > secondary) {
                return Secondary;
            }
            return Primary;
        }
        public float GetLongestAttackRange()
        {
            float primary = Primary.GetAttackRange();
            float secondary = Secondary.GetAttackRange();
            if (primary > secondary) {
                return primary;
            }
            return secondary;
        }
    }
}