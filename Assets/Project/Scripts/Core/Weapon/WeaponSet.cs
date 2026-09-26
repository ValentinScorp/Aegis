using UnityEngine;

namespace Aegis.Core
{
    public class WeaponSet
    {
        public WeaponConfig MainWeapon { get; private set; }
        public WeaponConfig OffWeapon { get; private set; }

        public WeaponSet(WeaponConfig mainHand, WeaponConfig offHand)
        {
            MainWeapon = mainHand;
            if (mainHand != null && WeaponGrip.IsTwoHanded(mainHand.WeaponType) && offHand != null) {
                Debug.LogWarning($"{mainHand.Id} — two hand weapon, OffHand ({offHand.Id}) will be ignored.");
                return;
            }
            OffWeapon = offHand;
        }
        public bool IsInHolster { get; private set; }
        public bool IsRanged => MainWeapon != null && MainWeapon.IsRanged;
        public bool IsEmpty => MainWeapon == null && OffWeapon == null;
        public float AttackTime => MainWeapon != null ? WeaponAttackTimes.Get(MainWeapon.WeaponType) : 0.0f;
        public float AttackEventTime => MainWeapon != null ? WeaponAttackEventTimes.Get(MainWeapon.WeaponType) : 0.0f;
        public string ProjectileId => MainWeapon != null ? MainWeapon.ProjectileId : "";
        public float GetDamage => MainWeapon != null ? MainWeapon.Damage : 0.0f;
        public bool IsBow()
        {
            if (MainWeapon is null) {
                Debug.LogWarning("<MainHand> not set in <WeaponSet>!");
                return false;
            }

            if (MainWeapon.WeaponType == WeaponTypeId.Bow) {
                return true;
            }
            return false;
        }
        public WeaponTypeId WeaponType => MainWeapon != null ? MainWeapon.WeaponType : WeaponTypeId.None;
        public string Animation => MainWeapon != null ? MainWeapon.Animation : null;
        public float GetAttackRange()
        {
            if (MainWeapon != null) return MainWeapon.AttackRange;
            return 0.5f;
        }
        public bool CanReach(float targetDistance)
        {
            if (MainWeapon != null && MainWeapon.AttackRange >= targetDistance) return true;
            if (OffWeapon != null && OffWeapon.AttackRange >= targetDistance) return true;
            return false;
        }
    }
}