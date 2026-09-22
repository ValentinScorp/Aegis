using System;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;

namespace Aegis.Core
{
    public class BodyHealth
    {
        private readonly Dictionary<BodyPartId, Health> _parts = new();

        public event Action<BodyPartId, float, float> PartChanged; // part, current, max
        public event Action Depleted; // якщо вважаєш смерть за сумою / критичними частинами

        public BodyHealth(float headMax, float torsoMax, float armMax, float legMax)
        {
            _parts[BodyPartId.Head] = Create(headMax);
            _parts[BodyPartId.Torso] = Create(torsoMax);
            _parts[BodyPartId.ArmLeft] = Create(armMax);
            _parts[BodyPartId.ArmRight] = Create(armMax);
            _parts[BodyPartId.LegLeft] = Create(legMax);
            _parts[BodyPartId.LegRight] = Create(legMax);
        }

        private Health Create(float max)
        {
            var h = new Health(max);
            // Підписка буде нижче через лямбду з part
            return h;
        }

        public void InitEvents()
        {
            foreach (var kv in _parts) {
                var part = kv.Key;
                kv.Value.Changed += (cur, max) => {
                    Debug.Log($"[BodyHealth] {part}: {cur}/{max}");
                    PartChanged?.Invoke(part, cur, max);
                };
            }
        }

        public Health Get(BodyPartId part) => _parts[part];

        public void TakeDamage(BodyPartId part, float amount)
        {
            if (!IsAlive) return;

            if (_parts.TryGetValue(part, out var h))
                h.TakeDamage(amount);

            if (!IsAlive)
                Depleted?.Invoke();
        }
        public void Heal(BodyPartId partId, float amount)
        {
            _parts[partId]?.Heal(amount);
        }
        public bool IsAlive => _parts[BodyPartId.Head].IsAlive && _parts[BodyPartId.Torso].IsAlive;

        public float GetCurrent(BodyPartId part) => _parts[part].Current;
        public float GetMax(BodyPartId part) => _parts[part].Max;
        public float GetCurrentAllParts() => _parts.Values.Sum(h => h.Current);
        public float GetMaxAllParts() => _parts.Values.Sum(h => h.Max);
    }
}