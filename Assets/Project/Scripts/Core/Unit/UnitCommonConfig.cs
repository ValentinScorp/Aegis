// Core/Unit/UnitCommonConfig.cs
using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "UnitCommonConfig", menuName = "Aegis/Unit Common Config")]
    public class UnitCommonConfig : ScriptableObject
    {
        [Header("Unarmed")]
        public float UnarmedDamage;
        public float UnarmedCooldown;

        [Header("Health")]
        public float BaseHealth;
        public float HealthPerStrength;

        [Header("Movement")]
        public float SearchRadius;
        public float ChaseRadius;
        public float MoveSpeed;
        public float WalkAnimationSpeedMultiplier;
    }
}