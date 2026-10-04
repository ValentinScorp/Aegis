using System;

namespace Aegis.Core
{
    [Serializable]
    public class EffectConfigEntry
    {
        public string id;
        public string action;
        public string target;
        public string description;
        public string nature;
        public float value;
        public float duration;
    }

    [Serializable]
    public class EffectConfigCollection
    {
        public EffectConfigEntry[] effects;
    }
}