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
    public class CameraModeSwitcher : MonoBehaviour
    {
        [SerializeField] private CameraView _cameraView;
        [SerializeField] private PlayerInputListener _inputListener;
        [SerializeField] private SelectionController _selectionController;
        private CameraRig _camRig;
        private SelectionModel _selectionModel;
        private Unit _thirdPersonUnit;

        private void Awake()
        {
            _cameraView = Utilities.ComponentResolver.ResolveOrFind(this, _cameraView);
            _camRig = _cameraView?.Rig;
            _inputListener = Utilities.ComponentResolver.ResolveOrFind(this, _inputListener);
            _selectionController = Utilities.ComponentResolver.ResolveOrFind(this, _selectionController);
            _selectionModel = _selectionController?.Model;
        }

        private void OnEnable()
        {
            _inputListener.FreeCameraRequested += OnFreeRequested;
            _inputListener.FollowCameraRequested += OnFollowRequested;
            _inputListener.ThirdPersonCameraRequested += OnThirdPersonRequested;
            World.Instance.PlayerUnitAssigned += OnPlayerUnitAssigned;
        }

        private void OnDisable()
        {
            _inputListener.FreeCameraRequested -= OnFreeRequested;
            _inputListener.FollowCameraRequested -= OnFollowRequested;
            _inputListener.ThirdPersonCameraRequested -= OnThirdPersonRequested;
            World.Instance.PlayerUnitAssigned -= OnPlayerUnitAssigned;
        }

        private void Update()
        {
            if (_thirdPersonUnit == null) return;
            if (_camRig.CurrentMode != CameraMode.ThirdPerson) return;

            // WASD в Odyssey-режимі керує не панорамою камери, а юнітом —
            // напрямок рахуємо відносно поточного yaw камери.
            Vector2 move = _inputListener.CameraMoveInput;
            Quaternion yawRot = Quaternion.Euler(0f, _camRig.Yaw, 0f);
            Vector3 worldDir = yawRot * new Vector3(move.x, 0f, move.y);

            _thirdPersonUnit.PerformDirectMove(worldDir);
        }

        private void OnFreeRequested()
        {
            ReleaseThirdPersonUnit();
            _camRig.SetMode(CameraMode.Free);
        }

        private void OnFollowRequested()
        {
            var unit = _selectionModel.Selected;
            if (unit == null) {
                Debug.LogWarning("[CameraModeController] Немає вибраного юніта для Follow-режиму.");
                return;
            }
            ReleaseThirdPersonUnit();
            _camRig.SetMode(CameraMode.Follow, unit);
        }

        private void OnThirdPersonRequested()
        {
            var unit = _selectionModel.Selected;
            if (unit == null) {
                Debug.LogWarning("[CameraModeController] Немає вибраного юніта для Odyssey-режиму.");
                return;
            }

            EnterThirdPerson(unit);
        }
        public void EnterThirdPerson(Unit unit)
        {
            ReleaseThirdPersonUnit();
            _thirdPersonUnit = unit;
            unit.SetControlMode(UnitControlMode.Direct);
            _camRig.SetMode(CameraMode.ThirdPerson, unit);
        }

        private void ReleaseThirdPersonUnit()
        {
            if (_thirdPersonUnit == null) return;

            _thirdPersonUnit.PerformDirectMove(Vector3.zero);
            _thirdPersonUnit.SetControlMode(UnitControlMode.Indirect);
            _thirdPersonUnit = null;
        }
        private void OnPlayerUnitAssigned(Unit unit)
        {
            _selectionModel.Select(unit);
            EnterThirdPerson(unit);
        }
    }
}
