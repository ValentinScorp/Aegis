using System.Collections.Generic;
using UnityEngine;

namespace Aegis.View
{
    public readonly struct WeaponAnimationInfo
    {
        public readonly int StateHash;
        public readonly string ClipName;

        public WeaponAnimationInfo(int stateHash, string clipName)
        {
            StateHash = stateHash;
            ClipName = clipName;
        }
    }

    /// <summary>
    /// Мапить категорію анімації зброї (WeaponConfig.Animation, напр. "sword"/"bow")
    /// на конкретний стан Animator-контролера й ім'я кліпа для розрахунку AttackSpeed.
    /// Категорію задає контент (JSON), сам мапінг на Animator-стани — структура коду.
    /// </summary>
    public static class WeaponAnimationCatalog
    {
        private static readonly int SwordAttackHash = Animator.StringToHash("SwordAttack");
        private static readonly int BowShootHash = Animator.StringToHash("BowShoot");

        private static readonly Dictionary<string, WeaponAnimationInfo> _map = new() {
            { "sword", new WeaponAnimationInfo(SwordAttackHash, "Human_HitSword") },
            { "bow",   new WeaponAnimationInfo(BowShootHash,    "Human_ShootBow") },
            // TODO: "dagger" / "spear" / "shield" — коли з'являться свої кліпи
        };

        private static readonly WeaponAnimationInfo _fallback = _map["sword"];

        public static WeaponAnimationInfo Get(string animationName)
        {
            if (!string.IsNullOrEmpty(animationName) && _map.TryGetValue(animationName, out var info))
                return info;

            Debug.LogWarning($"[WeaponAnimationCatalog] No animation mapped for <{animationName}>, using fallback.");
            return _fallback;
        }
    }
}