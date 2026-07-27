// Core/Unit/UnitConfig.cs
using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "UnitConfig", menuName = "Aegis/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public EntityType UnitType;

        [Header("Base Stats")]
        public float BaseStrength;
        public float BaseSpeed;
        public float BaseSpirit;

        [Header("Weaponry")]
        public WeaponConfig MainHandPrimary;
        public WeaponConfig OffHandPrimary;
        public WeaponConfig MainHandSecondary;
        public WeaponConfig OffHandSecondary;
    }
}