using Aegis.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Aegis.View
{
    [RequireComponent(typeof(Slider))]
    public class HealthBarView : HealthView
    {
        private Slider _healthBar;

        private void Awake() => _healthBar = GetComponent<Slider>();

        public override void OnHealthChanged(float value, float max)
        {
            _healthBar.maxValue = max;
            _healthBar.value = value;
        }
    }
}