using UnityEngine;

namespace Aegis.View
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private Color selectedColor = Color.yellow;

        private Renderer _renderer;

        private MaterialPropertyBlock _mpb;

        private bool _selected;
        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            _renderer = GetComponentInChildren<Renderer>();
            if (_renderer == null) Debug.LogWarning($"Didn't get <Renderer> in Prefab: {name}!", this);
        }
        public void Select(bool selected)
        {
            _selected = selected;
            UpdateVisual();
        }
        private void UpdateVisual()
        {
            if (_renderer == null) return;

            if (_mpb == null) _mpb = new MaterialPropertyBlock();

            _renderer.GetPropertyBlock(_mpb);

            if (_selected) {
                _mpb.SetColor("_EmissionColor", selectedColor);
            } else {
                _mpb.SetColor("_EmissionColor", Color.black);
            }

            _renderer.SetPropertyBlock(_mpb);
        }
    }
}
