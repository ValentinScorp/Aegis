using UnityEngine;

namespace Aegis.Core
{
    /// <summary>
    /// Діабло-стиль: фіксований ізометричний кут, камера плавно
    /// слідкує за rig.Target. Керування рухом юніта лишається
    /// клік-по-землі (SelectionController) — цей режим лише позиціонує камеру.
    /// </summary>
    public class FollowCameraMode : ICameraMode
    {
        public CameraMode Mode => CameraMode.Follow;

        private const float Distance = 14f;
        private const float FixedYaw = 45f;
        private const float FixedPitch = 45f;
        private const float SmoothTime = 0.15f;

        public void Enter(CameraRig rig)
        {
            rig.ApplyRotation(FixedYaw, FixedPitch);
        }
        public void Exit(CameraRig rig) { }

        public void Tick(CameraRig rig, in CameraTickInput input, float deltaTime)
        {
            if (rig.Target == null) return;

            Quaternion rot = Quaternion.Euler(FixedPitch, FixedYaw, 0f);
            Vector3 desired = rig.Target.Position - rot * Vector3.forward * Distance;

            float t = SmoothTime <= 0f ? 1f : 1f - Mathf.Exp(-deltaTime / SmoothTime);
            rig.ApplyPosition(Vector3.Lerp(rig.Position, desired, t));
        }
    }
}
