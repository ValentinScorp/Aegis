
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

        private WorldEntity _selectedEntity;

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
                SelectNewEntity(entity);
                return;
            }

            if (TryGetGroundPoint(list, out var groundPoint)) {
                _selectedEntity?.PerformMovementTo(groundPoint);
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
        private void SelectNewEntity(WorldEntity entity)
        {
            if (_selectedEntity is not null) {
                _selectedEntity.Select(false);                
            }
            _selectedEntity = entity;
            _selectedEntity.Select(true);
        }
    }
}
