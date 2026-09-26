// Core/Unit/UnitConfig.cs
using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "UnitConfig", menuName = "Aegis/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public UnitType UnitType;

        [Header("Base Stats")]
        public float BaseStrength;
        public float BaseSpeed;
        public float BaseSpirit;

        [Header("Weaponry")]
        public WeaponConfig MainWeaponPrimary;
        public WeaponConfig OffWeaponPrimary;
        public WeaponConfig MainWeaponSecondary;
        public WeaponConfig OffWeaponSecondary;
    }
}