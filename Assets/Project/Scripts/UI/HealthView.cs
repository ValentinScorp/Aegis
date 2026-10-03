using Aegis.Core;
using UnityEngine;

namespace Aegis.UI
{
    public abstract class HealthView : MonoBehaviour
    {
        public virtual void OnHealthChanged(float value, float max) { }
        public virtual void OnPartChanged(BodyPartId part, float current, float max) { }

        public virtual void Refresh(BodyHealth body) { }
        public virtual void OnHealthDepleted() => gameObject.SetActive(false);
    }
}