using System;

namespace Aegis.Core
{
    public interface IDamageable
    {
        public Health Health { get; }
        void TakeDamage(float amount);
        event Action Died;
    }
}