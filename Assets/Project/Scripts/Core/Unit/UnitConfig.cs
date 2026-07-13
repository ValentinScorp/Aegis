using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "UnitConfig", 
        menuName = "Aegis/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        [Header("Combat")]
        public float AttackTime = 1.5f;
        public float AttackDamage = 12f;
        public float AttackRadius = 2f;
        public bool CanShoot = false;
        public float ShootRadius = 8f;
        public float AttackEventTime = 0.627f;

        [Header("Movement")]
        public float SearchRadius = 6f;
        public float ChaseRadius = 7f;

        [Header("Health")]
        public float MaxHealth = 100f;
    }
}