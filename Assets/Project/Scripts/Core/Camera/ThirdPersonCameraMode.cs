using UnityEngine;

namespace Aegis.Core
{
    /// <summary>
    /// AC Odyssey-стиль: камера за спиною rig.Target, yaw/pitch керуються
    /// мишею (LookDelta). Напрямок камери також використовується View-шаром
    /// для трансформації WASD-вводу в світовий напрямок прямого руху юніта.
    /// </summary>
    public class ThirdPersonCameraMode : ICameraMode
    {
        public CameraMode Mode => CameraMode.ThirdPerson;

        private const float Distance = 4.5f;
        private const float Height = 2f;
        private const float LookSensitivity = 0.15f;
        private const float MinPitch = -10f; // side
        private const float MaxPitch = 75f; // top
        private const float SmoothTime = 0.08f;

        private Vector3 _pivot;

        public void Enter(CameraRig rig)
        {
            if (rig.Target != null) _pivot = rig.Target.Position;
        }
        public void Exit(CameraRig rig) { }

        public void Tick(CameraRig rig, in CameraTickInput input, float deltaTime)
        {
            if (rig.Target == null) return;

            float yaw = rig.Yaw + input.LookDelta.x * LookSensitivity;
            float pitch = Mathf.Clamp(rig.Pitch - input.LookDelta.y * LookSensitivity, MinPitch, MaxPitch);
            rig.ApplyRotation(yaw, pitch);

            float t = SmoothTime <= 0f ? 1f : 1f - Mathf.Exp(-deltaTime / SmoothTime);
            _pivot = Vector3.Lerp(_pivot, rig.Target.Position, t);

            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            rig.ApplyPosition(_pivot - rot * Vector3.forward * Distance + Vector3.up * Height);
        }
    }
}
