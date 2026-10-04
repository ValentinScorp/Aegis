using UnityEngine;

namespace Aegis.Core
{
    [CreateAssetMenu(fileName = "EffectConfig", menuName = "Aegis/Effect Config")]
    public class EffectConfig : ScriptableObject
    {
        public string Id;
        public EffectAction Action;
        public EffectTarget Target;
        public EffectNature Nature;
        public string Description;
        public float Value;
        public float Duration;
    }
}