using System;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitWeaponry
    {
        public class WeaponSet
        {
            public WeaponConfig MainHand { get; private set; }
            public WeaponConfig OffHand  { get; private set; }

            public WeaponSet()
            { }
            public WeaponSet(WeaponConfig mainHand, WeaponConfig offHand)
            {
                MainHand = mainHand;
                OffHand = offHand;
            }
            public bool IsSheated { get; private set; }
            public bool IsRanged => MainHand != null && MainHand.IsRanged;
            public bool IsEmpty => MainHand == null && OffHand == null;
            public float AttackTime => MainHand != null ? WeaponAttackTimes.Get(MainHand.WeaponType) : 0.0f;
            public float AttackEventTime => MainHand != null ? WeaponAttackEventTimes.Get(MainHand.WeaponType) : 0.0f;
            public string ProjectileId => MainHand != null ? MainHand.ProjectileId : "";
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
            public WeaponType WeaponType => MainHand != null ? MainHand.WeaponType : WeaponType.None;
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
            internal void Set(WeaponConfig main, WeaponConfig off)
            {
                MainHand = main;
                OffHand  = off;
            }
        }

        public WeaponSet Primary { get; private set; }
        public WeaponSet Secondary { get; private set; }
        public WeaponSet Active { get; private set; }
        public bool IsSheathed { get; private set; }
        public event Action<UnitWeaponry> Changed;

        public UnitWeaponry()
        {
            Primary = new WeaponSet();
            Secondary = new WeaponSet();
            Active = Primary;
        }
        public UnitWeaponry(WeaponConfig primMain, WeaponConfig primOff, 
                            WeaponConfig secMain, WeaponConfig secOff)
        {
            Primary = new WeaponSet(primMain, primOff);
            Secondary = new WeaponSet(secMain, secOff);
            Active = Primary;
        }
        public float AttackTime => Active.AttackTime;
        public float AttackEventTime => Active.AttackEventTime;
        
        public float Damage => Active.GetDamage;
        public bool HasBow => Primary.IsBow() || Secondary.IsBow();
        public bool BowActive => Active.IsBow();
        public bool HasAnyRanged => Primary.IsRanged || Secondary.IsRanged;
        public string ActiveProjectileId => Active.ProjectileId;
        public WeaponType ActiveWeaponType => Active.WeaponType;

        public float GetAttackRange()
        {
            return Active.GetAttackRange();
        }
        public WeaponSet GetClosestWeaponSet()
        {
            float primaryRange = Primary.GetAttackRange();
            float secondaryRange = Secondary.GetAttackRange();
            return primaryRange > secondaryRange ? Secondary : Primary;
        }
        public float GetLongestAttackRange()
        {
            float primaryRange = Primary.GetAttackRange();
            float secondaryRange = Secondary.GetAttackRange();
            return primaryRange > secondaryRange ? primaryRange : secondaryRange;
        }
        public void Refresh()
        {
            Changed?.Invoke(this);
        }

        public void SwitchSet()
        {
            if (Primary.IsEmpty) return;
            if (Secondary.IsEmpty) return;

            SetActive(Active == Primary ? Secondary : Primary);
        }
        private void SetActive(WeaponSet set)
        {
            if (set == null) return;
            if (set != Primary && set != Secondary) return;
            if (set == Active) return;

            Active = set;
            Changed?.Invoke(this);
        }
        public void ToggleSheathe()
        {
            if (IsSheathed) Unsheathe();
            else Sheathe();
        }
        public void Sheathe()
        {
            if (IsSheathed) return;
            IsSheathed = true;
            Changed?.Invoke(this);
        }

        public void Unsheathe()
        {
            if (!IsSheathed) return;
            IsSheathed = false;
            Changed?.Invoke(this);
        }

        public void Equip(WeaponSet targetSet, WeaponConfig main, WeaponConfig off)
        {
            if (targetSet != Primary && targetSet != Secondary) return;

            targetSet.Set(main, off);
            
            if (targetSet == Active)
                IsSheathed = false;

            Changed?.Invoke(this);
        }

        public void EquipPrimary(WeaponConfig main, WeaponConfig off)   => Equip(Primary, main, off);
        public void EquipSecondary(WeaponConfig main, WeaponConfig off) => Equip(Secondary, main, off);
    }
}