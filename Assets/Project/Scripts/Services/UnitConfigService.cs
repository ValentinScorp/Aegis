using Aegis.Core;
using TMPro;

namespace Aegis.View
{
    public class UnitConfigService
    {
        private readonly EntityViewRegistry _registry;

        public UnitConfigService(EntityViewRegistry registry)
        {
            _registry = registry;
        }

       public UnitConfig GetConfig(EntityType type)
        {
            return _registry.GetConfig(type);
        }
    }
}