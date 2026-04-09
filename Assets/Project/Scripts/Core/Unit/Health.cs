using System;
using UnityEngine;

namespace Aegis.Core
{
    public class Health
    {
        public float Max { get; private set; }
        public float Current { get; private set; }
        public bool IsAlive => Current > 0f;

        public event Action<float, float> Changed; // current, max
        public event Action Depleted;

        public Health(float max)
        {
            Max = max;
            Current = max;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f) return;

            Current = Mathf.Max(0f, Current - amount);
            Changed?.Invoke(Current, Max);

            if (Current == 0f)
                Depleted?.Invoke();
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;

            Current = Mathf.Min(Max, Current + amount);
            Changed?.Invoke(Current, Max);
        }

        public void Reset() 
        {
            Current = Max;
            Changed?.Invoke(Current, Max);
        }

        public float Percentage => Current / Max;
    }
}