using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Aegis/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public string Animation;
        public WeaponType WeaponType;
        public bool IsRanged => WeaponType == WeaponType.Bow;

        [Header("Combat")]
        public float Damage;
        public float AttackRange;
        public float Weight;

        [Header("Shield only")]
        public float ArmorValue;
        public float DeflectChance;

        public string[] EffectIds;
    }
}