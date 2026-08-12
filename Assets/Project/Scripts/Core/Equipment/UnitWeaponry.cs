using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitWeaponry
    {
        private readonly WeaponSet _primary;
        private readonly WeaponSet _secondary;
        private WeaponSet _active { get; set; }

        public UnitWeaponry(WeaponSet primary, WeaponSet secondary)
        {
            _primary = primary;
            _secondary = secondary;
            _active = primary;
        }
        public float AttackTime => _active.AttackTime;
        public float AttackEventTime => _active.AttackEventTime;
        public void SetActive(WeaponSet set) => _active = set;

        public float Damage => _active.GetDamage;
        public bool HasBow => _primary.IsBow() || _secondary.IsBow();
        public bool BowActive => _active.IsBow();
        public bool HasAnyRanged => _primary.IsRanged || _secondary.IsRanged;
        public string ActiveProjectileId => _active.ProjectileId;
        public WeaponType ActiveWeaponType => _active.WeaponType;
        public float GetAttackRange()
        {
            return _active.GetAttackRange();
        }
        public WeaponSet GetClosestWeaponSet()
        {
            float primary = _primary.GetAttackRange();
            float secondary = _secondary.GetAttackRange();
            if (primary > secondary) {
                return _secondary;
            }
            return _primary;
        }
        public float GetLongestAttackRange()
        {
            float primary = _primary.GetAttackRange();
            float secondary = _secondary.GetAttackRange();
            if (primary > secondary) {
                return primary;
            }
            return secondary;
        }
    }
}