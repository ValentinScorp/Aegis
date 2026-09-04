using UnityEngine;
using Aegis.Core;
using Aegis.View;

namespace Aegis.Services
{
    /// <summary>
    /// Оркеструє перемикання режимів камери гарячими клавішами (F1/F2/F3
    /// в PlayerInputListener.HotkeyListener) і синхронізує їх з
    /// UnitControlMode вибраного юніта. Сама позиція/орієнтація камери
    /// рахується в Core (CameraRig + ICameraMode) — цей клас лише командує.
    /// </summary>
    public class CameraModeController : MonoBehaviour
    {
        [SerializeField] private CameraControllerView _cameraView;
        [SerializeField] private PlayerInputListener _inputListener;
        [SerializeField] private SelectionController _selectionController;

        private Unit _thirdPersonUnit;

        private void Awake()
        {
            _cameraView = Utilities.ComponentResolver.ResolveOrFind(this, _cameraView);
            _inputListener = Utilities.ComponentResolver.ResolveOrFind(this, _inputListener);
            _selectionController = Utilities.ComponentResolver.ResolveOrFind(this, _selectionController);
        }

        private void OnEnable()
        {
            _inputListener.FreeCameraRequested += OnFreeRequested;
            _inputListener.FollowCameraRequested += OnFollowRequested;
            _inputListener.ThirdPersonCameraRequested += OnThirdPersonRequested;
        }

        private void OnDisable()
        {
            _inputListener.FreeCameraRequested -= OnFreeRequested;
            _inputListener.FollowCameraRequested -= OnFollowRequested;
            _inputListener.ThirdPersonCameraRequested -= OnThirdPersonRequested;
        }

        private void Update()
        {
            if (_thirdPersonUnit == null) return;
            if (_cameraView.Rig.CurrentMode != CameraMode.ThirdPerson) return;

            // WASD в Odyssey-режимі керує не панорамою камери, а юнітом —
            // напрямок рахуємо відносно поточного yaw камери.
            Vector2 move = _inputListener.CameraMoveInput;
            Quaternion yawRot = Quaternion.Euler(0f, _cameraView.Rig.Yaw, 0f);
            Vector3 worldDir = yawRot * new Vector3(move.x, 0f, move.y);

            _thirdPersonUnit.PerformDirectMove(worldDir);
        }

        private void OnFreeRequested()
        {
            ReleaseThirdPersonUnit();
            _cameraView.Rig.SetMode(CameraMode.Free);
        }

        private void OnFollowRequested()
        {
            var unit = _selectionController.SelectedUnit;
            if (unit == null) {
                Debug.LogWarning("[CameraModeController] Немає вибраного юніта для Follow-режиму.");
                return;
            }
            ReleaseThirdPersonUnit();
            _cameraView.Rig.SetMode(CameraMode.Follow, unit);
        }

        private void OnThirdPersonRequested()
        {
            var unit = _selectionController.SelectedUnit;
            if (unit == null) {
                Debug.LogWarning("[CameraModeController] Немає вибраного юніта для Odyssey-режиму.");
                return;
            }

            ReleaseThirdPersonUnit();
            _thirdPersonUnit = unit;
            _thirdPersonUnit.SetControlMode(UnitControlMode.Direct);
            _cameraView.Rig.SetMode(CameraMode.ThirdPerson, unit);
        }

        private void ReleaseThirdPersonUnit()
        {
            if (_thirdPersonUnit == null) return;

            _thirdPersonUnit.PerformDirectMove(Vector3.zero);
            _thirdPersonUnit.SetControlMode(UnitControlMode.Indirect);
            _thirdPersonUnit = null;
        }
    }
}
