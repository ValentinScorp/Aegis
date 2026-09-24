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

        public bool IsMoving { get; private set; }
        public float CurrentSpeedNormalized { get; private set; }

        private void Awake()
        {
            _controller = ComponentResolver.Require(this, GetComponent<CharacterController>());
        }

        private void Update()
        {
            if (_unit == null || !_controller.enabled) return;

            IsMoving = _hasPendingDirection;
            // CurrentSpeedNormalized = _pendingDirection;

            Vector3 direction = _hasPendingDirection ? _pendingDirection : Vector3.zero;
            _hasPendingDirection = false;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -1f;
            _verticalVelocity += _gravity * Time.deltaTime;

            Vector3 motion = direction * _unit.MoveSpeed + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);

            // TIMCHASOVYI DEBUG — прибрати після діагностики
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 5f))
            {
                float capsuleBottom = _controller.bounds.min.y;
                float floorY = hit.point.y;
                // Debug.Log($"[DirectMove] transform.y={transform.position.y:F4} " +
                //           $"capsule.bottom={capsuleBottom:F4} " +
                //           $"floor(raycast)={floorY:F4} " +
                //           $"gap={capsuleBottom - floorY:F4} " +
                //           $"isGrounded={_controller.isGrounded} " +
                //           $"vVel={_verticalVelocity:F3} " +
                //           $"center={_controller.center} height={_controller.height} " +
                //           $"skin={_controller.skinWidth} radius={_controller.radius}");
            }
            else
            {
                // Debug.Log($"[DirectMove] transform.y={transform.position.y:F4} " +
                //           $"capsule.bottom={_controller.bounds.min.y:F4} " +
                //           $"floor(raycast)=НЕ ЗНАЙДЕНО (немає колайдера під ногами!) " +
                //           $"isGrounded={_controller.isGrounded}");
            }

            if (direction.sqrMagnitude > 0.0001f)
            {
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

        public void SetActive(bool value)
        {
            _controller.enabled = value;
            enabled = value;
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