using Aegis.Core;
using UnityEngine;

namespace Aegis.UI
{
    public class PlayerHealthHud : MonoBehaviour
    {
        [SerializeField] private HealthView _healthView;

        private Unit _unit;

        private void Awake() => _healthView.gameObject.SetActive(false);

        public void Bind(Unit unit)
        {
            Unbind();
            _unit = unit;
            if (_unit == null) return;

            _healthView.gameObject.SetActive(true);        // спершу активуємо, щоб виконався Awake ляльки
            _unit.HealthChanged += _healthView.OnHealthChanged;
            _unit.BodyPartHealthChanged += _healthView.OnPartChanged;
            _healthView.Refresh(_unit.BodyHealth);
        }

        public void Unbind()
        {
            if (_unit == null) return;
            _unit.HealthChanged -= _healthView.OnHealthChanged;
            _unit.BodyPartHealthChanged -= _healthView.OnPartChanged;
            _unit = null;
            _healthView.gameObject.SetActive(false);
        }

        private void OnDestroy() => Unbind();
    }
}