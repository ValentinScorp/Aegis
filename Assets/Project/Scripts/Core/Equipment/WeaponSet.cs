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
        public float AttackRange => MainHand != null ? MainHand.AttackRange : 1.0f;
    }
}