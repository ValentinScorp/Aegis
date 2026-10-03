using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitWeaponry
    {
        public Dictionary<WeaponSetId, WeaponSet> WeaponSets { get; private set; }
        public WeaponSetId ActiveSet { get; private set; }
        public bool IsHolstered { get; private set; }
        public event Action<UnitWeaponry> Changed;

        public UnitWeaponry()
        {
            WeaponSets = new Dictionary<WeaponSetId, WeaponSet>();
            ActiveSet = WeaponSetId.Primary;
            Holster();
        }
        public UnitWeaponry(WeaponConfig primMain, WeaponConfig primOff,
                            WeaponConfig secMain, WeaponConfig secOff)
        {
            WeaponSets = new Dictionary<WeaponSetId, WeaponSet>
            {
                { WeaponSetId.Primary,   new WeaponSet(primMain, primOff) },
                { WeaponSetId.Secondary, new WeaponSet(secMain, secOff) }
            };
            ActiveSet = WeaponSetId.Primary;
            Holster();
        }

        public float AttackTime => WeaponSets[ActiveSet].AttackTime;
        public float AttackEventTime => WeaponSets[ActiveSet].AttackEventTime;
        public float Damage => WeaponSets[ActiveSet].GetDamage;
        public bool HasBow => WeaponSets[WeaponSetId.Primary].IsBow() || WeaponSets[WeaponSetId.Secondary].IsBow();
        public bool BowActive => WeaponSets[ActiveSet].IsBow();
        public string ActiveProjectileId => WeaponSets[ActiveSet].ProjectileId;
        public string ActiveAnimation => WeaponSets[ActiveSet].Animation;
        public float GetAttackRange()
        {
            return WeaponSets[ActiveSet].GetAttackRange();
        }

        private void SetActive(WeaponSetId setId)
        {
            ActiveSet = setId;
            Changed?.Invoke(this);
        }
        public void ToggleHolster()
        {
            if (IsHolstered) Unholster();
            else Holster();
        }
        public void Holster()
        {
            if (IsHolstered) return;
            IsHolstered = true;
            Changed?.Invoke(this);
        }

        public void Unholster()
        {
            if (!IsHolstered) return;
            IsHolstered = false;
            Changed?.Invoke(this);
        }

        internal WeaponTypeId GetWeaponType(WeaponSetId setId, WeaponRoleId roleId)
        {
            if (!WeaponSets.TryGetValue(setId, out var set))
                return WeaponTypeId.None;

            return roleId switch {
                WeaponRoleId.Main => set.MainWeapon?.WeaponType ?? WeaponTypeId.None,
                WeaponRoleId.Off => set.OffWeapon?.WeaponType ?? WeaponTypeId.None,
                _ => WeaponTypeId.None
            };
        }
    }
}