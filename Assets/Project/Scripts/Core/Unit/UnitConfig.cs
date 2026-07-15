using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "UnitConfig", 
        menuName = "Aegis/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public EntityType UnitType;
        [Header("Combat")]
        public float AttackTime = 1.5f;
        public float AttackDamage = 12f;
        public float AttackRadius = 2f;
        public bool CanShoot = false;
        public float ShootTime = 1.5f;
        public float ShootRadius = 8f;
        public float AttackEventTime = 0.5f;
        public float ShootEventTime = 0.5f;

        [Header("Movement")]
        public float SearchRadius = 6f;
        public float ChaseRadius = 7f;

        [Header("Health")]
        public float MaxHealth = 100f;
    }
}