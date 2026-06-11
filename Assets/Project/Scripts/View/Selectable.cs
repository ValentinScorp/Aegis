using UnityEngine;

namespace Aegis.View
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private Renderer _objectRenderer;

        [SerializeField] private Color selectedColor = Color.yellow;

        private MaterialPropertyBlock _mpb;

        private bool _selected;
        
        private void Reset()
        {
            _objectRenderer = GetComponentInChildren<Renderer>();
            if (_objectRenderer == null) Debug.LogWarning("Didn't get Renderer in Prefab!");
        }
        public void Select(bool selected)
        {
            _selected = selected;
            UpdateVisual();
        }
        private void UpdateVisual()
        {
            if (_objectRenderer == null) return;

            if (_mpb == null) _mpb = new MaterialPropertyBlock();

            _objectRenderer.GetPropertyBlock(_mpb);

            if (_selected) {
                _mpb.SetColor("_EmissionColor", selectedColor);
            } else {
                _mpb.SetColor("_EmissionColor", Color.black);
            }

            _objectRenderer.SetPropertyBlock(_mpb);
        }

    }
}
