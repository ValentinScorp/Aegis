using UnityEngine;

namespace Aegis.Core
{
    /// <summary>
    /// Уніфікований пакет вводу для будь-якого режиму камери за один тік.
    /// Не всі поля використовуються кожним режимом (Free ігнорує LookDelta,
    /// Follow/ThirdPerson ігнорують MoveInput тощо).
    /// </summary>
    public readonly struct CameraTickInput
    {
        public readonly Vector2 MoveInput;
        public readonly float VerticalInput;
        public readonly Vector2 LookDelta;
        public readonly Vector3? TargetPosition;

        public CameraTickInput(Vector2 moveInput, float verticalInput, Vector2 lookDelta, Vector3? targetPosition)
        {
            MoveInput = moveInput;
            VerticalInput = verticalInput;
            LookDelta = lookDelta;
            TargetPosition = targetPosition;
        }
    }
}
