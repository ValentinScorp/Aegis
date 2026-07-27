namespace Aegis.Core
{
    public class WeaponSet
    {
        public readonly WeaponConfig MainHand;
        public readonly WeaponConfig OffHand;

        public WeaponSet(WeaponConfig mainHand, WeaponConfig offHand)
        {
            MainHand = mainHand;
            OffHand = offHand;
        }

        public bool IsRanged => MainHand != null && MainHand.IsRanged;
        public bool IsEmpty => MainHand == null && OffHand == null;
        public float AttackTime => MainHand != null ? WeaponAttackTimes.Get(MainHand.WeaponType) : 5.0f;
        public float AttackEventTime => MainHand != null ? WeaponAttackEventTimes.Get(MainHand.WeaponType) : 5.0f;
        public float GetLongestMeleeAttackRange() {
            if (MainHand != null && !MainHand.IsRanged && (OffHand == null || (OffHand != null && OffHand.IsRanged))) {
                return MainHand.AttackRange;
            }
            if ((MainHand == null || (MainHand != null && MainHand.IsRanged)) && OffHand != null && !OffHand.IsRanged) {
                return OffHand.AttackRange;
            }
            if (MainHand != null && !MainHand.IsRanged && OffHand != null && !OffHand.IsRanged) {
                return (MainHand.AttackRange >= OffHand.AttackRange) ? MainHand.AttackRange : OffHand.AttackRange;
            }
            return 1.0f;
        }
        public bool CanReach(float targetDistance)
        {
            if (MainHand != null && MainHand.AttackRange >= targetDistance) return true;
            if (OffHand != null && OffHand.AttackRange >= targetDistance) return true;
            return false;
        }
    }
}