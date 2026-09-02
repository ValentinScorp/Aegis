using System;
using UnityEngine;

namespace Aegis.Core
{
    public class CameraController
    {
        public event Action<Vector3> PositionChanged;

        public Vector3 Position { get; private set; }

        private readonly float _moveSpeed;
        private readonly float _verticalSpeed;

        public CameraController(Vector3 initialPosition, float moveSpeed, float verticalSpeed)
        {
            Position = initialPosition;
            _moveSpeed = moveSpeed;
            _verticalSpeed = verticalSpeed;
        }

        public void Tick(Vector2 moveInput, float verticalInput, float deltaTime)
        {
            Vector3 horizontal = new Vector3(moveInput.x, 0f, moveInput.y);
            if (horizontal.sqrMagnitude > 1f) {
                horizontal.Normalize();
            }

            Vector3 offset = horizontal * (_moveSpeed * deltaTime)
                            + Vector3.up * (verticalInput * _verticalSpeed * deltaTime);

            if (offset == Vector3.zero) return;

            Position += offset;
            PositionChanged?.Invoke(Position);
        }
    }
}