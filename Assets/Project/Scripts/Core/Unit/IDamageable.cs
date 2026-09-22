using System;

namespace Aegis.Core
{
    public interface IDamageable
    {
        public bool IsAlive { get; }
        public BodyHealth BodyHealth { get; }
        void TakeDamage(BodyPartId partId, float amount);
        event Action Died;
    }
}