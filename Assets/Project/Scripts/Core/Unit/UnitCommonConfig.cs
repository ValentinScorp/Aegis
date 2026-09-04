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

        [Header("Perception")]
        [Tooltip("Висота 'очей' юніта над Position — звідки й куди тягнеться промінь перевірки видимості.")]
        public float EyeHeight = 1.6f;
        [Tooltip("Шари, що блокують видимість (стіни, рельєф тощо). Юнітів у ці шари включати не треба.")]
        public LayerMask ObstacleMask;
    }
}