using UnityEngine;
using UnityEngine.UI;

namespace Aegis.View
{
    public class BodyPartView : MonoBehaviour
    {
        [SerializeField] private BodyPartId _part;
        [SerializeField] private Image _image;
        [SerializeField, Range(0f, 1f)] private float _saturation = 0.85f;
        [SerializeField, Range(0f, 1f)] private float _value = 0.95f;

        public BodyPartId Part => _part;

        private void Reset() => _image = GetComponent<Image>();

        public void Apply(float current, float max)
        {
            float ratio = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            _image.color = Color.HSVToRGB(Mathf.Lerp(0f, 0.33f, ratio), _saturation, _value);
        }
    }
}