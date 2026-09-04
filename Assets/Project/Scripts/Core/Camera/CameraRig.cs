using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    /// <summary>
    /// Заміна старого CameraController. Тримає позицію/орієнтацію камери
    /// і делегує обчислення конкретному ICameraMode — той самий підхід,
    /// що й UnitStateMachine для юнітів (enum-keyed реєстр, декларативний
    /// вибір поведінки замість розгалужень if/else).
    /// </summary>
    public class CameraRig
    {
        public event Action<Vector3> PositionChanged;
        public event Action<float, float> RotationChanged; // yaw, pitch

        public Vector3 Position { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public CameraMode CurrentMode { get; private set; }
        public WorldEntity Target { get; private set; }

        public readonly float MoveSpeed;
        public readonly float VerticalSpeed;

        private readonly Dictionary<CameraMode, ICameraMode> _modes = new();
        private ICameraMode _current;

        public CameraRig(Vector3 initialPosition, float moveSpeed, float verticalSpeed,
                          float initialYaw = 45f, float initialPitch = 45f)
        {
            Position = initialPosition;
            MoveSpeed = moveSpeed;
            VerticalSpeed = verticalSpeed;
            Yaw = initialYaw;
            Pitch = initialPitch;

            Register(new FreeCameraMode());
            Register(new FollowCameraMode());
            Register(new ThirdPersonCameraMode());

            SetMode(CameraMode.Free);
        }

        private void Register(ICameraMode mode) => _modes[mode.Mode] = mode;

        public void SetMode(CameraMode mode, WorldEntity target = null)
        {
            Target = target;

            if (CurrentMode == mode && _current != null) return;

            if (!_modes.TryGetValue(mode, out var next)) {
                Debug.LogWarning($"[CameraRig] Режим {mode} не зареєстрований");
                return;
            }

            _current?.Exit(this);
            _current = next;
            CurrentMode = mode;
            _current.Enter(this);
        }

        public void Tick(in CameraTickInput input, float deltaTime)
        {
            _current?.Tick(this, input, deltaTime);
        }

        public void SetTarget(WorldEntity target) => Target = target;

        internal void ApplyPosition(Vector3 position)
        {
            Position = position;
            PositionChanged?.Invoke(position);
        }

        internal void ApplyRotation(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
            RotationChanged?.Invoke(yaw, pitch);
        }
    }
}
