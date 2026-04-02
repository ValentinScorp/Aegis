using UnityEngine;

namespace Aegis.Core
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material selectedMaterial;

        private bool isSelected;

        private void Reset()
        {
            objectRenderer = GetComponentInChildren<Renderer>();
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
            if (objectRenderer == null) return;

            objectRenderer.material = isSelected ? selectedMaterial : defaultMaterial;
        }
    }
}
