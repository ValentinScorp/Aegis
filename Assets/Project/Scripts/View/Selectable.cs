using UnityEngine;

namespace Aegis.View
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private Renderer _objectRenderer;

        [SerializeField] private Color selectedColor = Color.yellow;

        private MaterialPropertyBlock _mpb;

        private bool isSelected;

        private void Reset()
        {
            _objectRenderer = GetComponentInChildren<Renderer>();
            if (_objectRenderer == null) Debug.LogWarning("Didn't get Renderer in Prefab!");
        }
        public void Select()
        {
            isSelected = true;
            UpdateVisual();
        }
        public void Deselect()
        {
            isSelected = false;
            UpdateVisual();
        }
        private void UpdateVisual()
        {
            if (_objectRenderer == null) return;

            if (_mpb == null) _mpb = new MaterialPropertyBlock();

            if (isSelected) {
                _mpb.SetColor("_EmissionColor", selectedColor);
            } else {
                _mpb.Clear();
            }

            _objectRenderer.SetPropertyBlock(_mpb);
        }
    }
}
