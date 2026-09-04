using Aegis.Core;
using UnityEngine;
using Aegis.Utilities;

namespace Aegis.View
{
    /// <summary>
    /// Прямий рух юніта під керуванням гравця (Odyssey-режим).
    /// Працює через CharacterController — на відміну від EntityMovement
    /// (NavMeshAgent), тут немає пошуку шляху: напрямок задається щокадрово
    /// ззовні (UnitMovementModeCoordinator/ThirdPersonCameraControllerView)
    /// через Unit.PerformDirectMove(), а Unit транслює це в DirectMoveRequested.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class EntityDirectMovement : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeedDegPerSec = 720f;
        [SerializeField] private float _gravity = -20f;

        private CharacterController _controller;
        private Unit _unit;
        private Vector3 _pendingDirection;
        private bool _hasPendingDirection;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = ComponentResolver.Require(this, GetComponent<CharacterController>());
        }

        private void Update()
        {
            if (_unit == null || !_controller.enabled) return;

            Vector3 direction = _hasPendingDirection ? _pendingDirection : Vector3.zero;
            _hasPendingDirection = false;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -1f;
            _verticalVelocity += _gravity * Time.deltaTime;

            Vector3 motion = direction * _unit.MoveSpeed + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);

            if (direction.sqrMagnitude > 0.0001f) {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot,
                    _rotationSpeedDegPerSec * Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_unit == null) return;
            _unit.Position = transform.position;
        }

        public void Bind(Unit unit)
        {
            if (unit is null) return;
            _unit = unit;
            _unit.DirectMoveRequested += OnDirectMoveRequested;
        }

        public void Unbind()
        {
            if (_unit == null) return;
            _unit.DirectMoveRequested -= OnDirectMoveRequested;
            _unit = null;
        }

        public void SetActive(bool active)
        {
            _controller.enabled = active;
            enabled = active;
            _hasPendingDirection = false;
            _verticalVelocity = 0f;
        }

        private void OnDirectMoveRequested(Vector3 worldDirection)
        {
            worldDirection.y = 0f;
            _pendingDirection = worldDirection.sqrMagnitude > 1f ? worldDirection.normalized : worldDirection;
            _hasPendingDirection = true;
        }
    }
}
