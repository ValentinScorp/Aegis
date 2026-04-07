
using System.Collections.Generic;
using UnityEngine;
using Aegis.Core;
using Aegis.View;
using System;

namespace Aegis.Services
{
    public class SelectionController : MonoBehaviour
    {
        [SerializeField] private PlayerInputListener _playerInputListener;
        [SerializeField] private RaycastHitDetector _raycastHitDetector;
        [SerializeField] private LayerMask _groundMask;

        private Unit _selectedUnit;

        private void Awake()
        {
            _playerInputListener = Utilities.ComponentResolver.ResolveOrFind(this, _playerInputListener);
            _raycastHitDetector  = Utilities.ComponentResolver.ResolveOrFind(this, _raycastHitDetector);
        }

        private void Start()
        {
            _playerInputListener.AttackPerformed += OnPlayerAttackPerformed;
            _raycastHitDetector.HitsDetected += OnRaycastHitsDetected;
        }
        private void OnDestroy()
        {
            _playerInputListener.AttackPerformed -= OnPlayerAttackPerformed;
            _raycastHitDetector.HitsDetected -= OnRaycastHitsDetected;
        }
        private void OnPlayerAttackPerformed()
        {
            // _selectedEntity?.PerformAttack();
        }
        private void OnRaycastHitsDetected(List<RaycastHit> list)
        {
            if (TryGetWorldEntity(list, out var entity)) {
                if (entity is Unit unit) SelectUnit(unit);
                return;
            }

            if (TryGetGroundPoint(list, out var groundPoint)) {
                if (_selectedUnit is Unit unit)
                    unit?.PerformWalk(groundPoint);
            }
        }
        private bool TryGetWorldEntity(List<RaycastHit> hits, out WorldEntity worldEntity)
        {
            if (TryGetEntityView(hits, out var view)) {
                worldEntity = view.Entity;
                return true;
            }
            worldEntity = null;
            return false;
        }
        private bool TryGetEntityView(List<RaycastHit> hits, out EntityView view)
        {
            foreach (var hit in hits) {
                view = hit.collider.GetComponentInParent<EntityView>();
                if (view != null) return true;
            }

            view = null;
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
        private void SelectUnit(Unit unit)
        {
            if (_selectedUnit is not null) {
                _selectedUnit.Select(false);                
            }
            _selectedUnit = unit;
            _selectedUnit.Select(true);
        }
    }
}
