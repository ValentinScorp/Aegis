using UnityEngine;

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
        public float AttackTime => MainHand != null ? WeaponAttackTimes.Get(MainHand.WeaponType) : 0.0f;
        public float AttackEventTime => MainHand != null ? WeaponAttackEventTimes.Get(MainHand.WeaponType) : 0.0f;
        public string ProjectileId => MainHand.ProjectileId;
        public float GetDamage => MainHand != null ? MainHand.Damage : 0.0f;
        public bool IsBow() 
        {
            if (MainHand is null) {
                Debug.LogWarning("<MainHand> not set in <WeaponSet>!");
                return false;
            }

            if (MainHand.WeaponType == WeaponType.Bow) {
                return true;
            }
            return false;
        }
        public WeaponType WeaponType => MainHand.WeaponType;
        public float GetAttackRange()
        {
            if (MainHand != null) return MainHand.AttackRange;
            return 0.5f;
        }
        public bool CanReach(float targetDistance)
        {
            if (MainHand != null && MainHand.AttackRange >= targetDistance) return true;
            if (OffHand != null && OffHand.AttackRange >= targetDistance) return true;
            return false;
        }
    }
}