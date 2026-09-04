namespace Aegis.Core
{
    public interface ICameraMode
    {
        CameraMode Mode { get; }

        void Enter(CameraRig rig);
        void Exit(CameraRig rig);
        void Tick(CameraRig rig, in CameraTickInput input, float deltaTime);
    }
}
