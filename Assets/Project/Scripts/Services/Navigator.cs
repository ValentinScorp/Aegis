
using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;
using Aegis.View;

namespace Aegis.Services
{
    public class Navigator : MonoBehaviour
    {
        [SerializeField] private RaycastDetector _hitDetector;
        [SerializeField] private LayerMask _groundMask;

        private Selectable _selected;
        private EntityView _selectedView;

        private void Start()
        {
            _hitDetector.HitsDetected += OnHitsDetected;
        }

        private void OnDestroy()
        {
            _hitDetector.HitsDetected -= OnHitsDetected;
        }

        private void OnHitsDetected(List<RaycastHit> list)
        {
            if (TryGetSelectable(list, out var selectable)) {
                Select(selectable);
                return;
            }

            if (_selectedView == null) return;

            if (TryGetGroundPoint(list, out var groundPoint)) {
                _selectedView.MoveTo(groundPoint);
            }
        }

        private bool TryGetSelectable(List<RaycastHit> hits, out Selectable selectable)
        {
            foreach (var hit in hits) {
                selectable = hit.collider.GetComponentInParent<Selectable>();
                if (selectable != null) return true;
            }

            selectable = null;
            return false;
        }

        private bool TryGetGroundPoint(List<RaycastHit> hits, out Vector3 point)
        {
            foreach (var hit in hits) {
                var layerMask = 1 << hit.collider.gameObject.layer;
                if ((layerMask & _groundMask.value) != 0) {
                    point = hit.point;
                    return true;
                }
            }

            point = default;
            return false;
        }

        private void Select(Selectable selectable)
        {
            if (_selected == selectable) return;

            _selected?.Deselect();
            _selected = selectable;
            _selected.Select();
            _selectedView = _selected.GetComponentInParent<EntityView>();
        }
    }
}
