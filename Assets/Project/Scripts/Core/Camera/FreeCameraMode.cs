using UnityEngine;

namespace Aegis.Core
{
    /// <summary>
    /// Вільна стратегічна камера (стара поведінка CameraController.Tick).
    /// WASD пан по площині + окрема вертикаль. Орієнтація не змінюється.
    /// </summary>
    public class FreeCameraMode : ICameraMode
    {
        public CameraMode Mode => CameraMode.Free;

        public void Enter(CameraRig rig) { }
        public void Exit(CameraRig rig) { }

        public void Tick(CameraRig rig, in CameraTickInput input, float deltaTime)
        {
            Vector3 horizontal = new Vector3(input.MoveInput.x, 0f, input.MoveInput.y);
            if (horizontal.sqrMagnitude > 1f)
                horizontal.Normalize();

            Vector3 offset = horizontal * (rig.MoveSpeed * deltaTime)
                            + Vector3.up * (input.VerticalInput * rig.VerticalSpeed * deltaTime);

            if (offset == Vector3.zero) return;

            rig.ApplyPosition(rig.Position + offset);
        }
    }
}
