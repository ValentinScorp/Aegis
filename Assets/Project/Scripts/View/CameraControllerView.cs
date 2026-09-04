using UnityEngine;
using Aegis.Core;
using Aegis.Services;

namespace Aegis.View
{
    public class CameraControllerView : MonoBehaviour
    {
        [SerializeField] private PlayerInputListener _inputListener;
        [SerializeField] private float _moveSpeed = 20f;
        [SerializeField] private float _verticalSpeed = 15f;

        public CameraRig Rig { get; private set; }

        private void Awake()
        {
            _inputListener = Utilities.ComponentResolver.ResolveOrFind(this, _inputListener);
            Rig = new CameraRig(transform.position, _moveSpeed, _verticalSpeed);
        }

        private void OnEnable()
        {
            Rig.PositionChanged += OnPositionChanged;
            Rig.RotationChanged += OnRotationChanged;
        }

        private void OnDisable()
        {
            Rig.PositionChanged -= OnPositionChanged;
            Rig.RotationChanged -= OnRotationChanged;
        }

        private void Update()
        {
            if (_inputListener == null) return;

            var input = new CameraTickInput(
                _inputListener.CameraMoveInput,
                _inputListener.CameraVerticalInput,
                _inputListener.LookDelta,
                Rig.Target?.Position);

            Rig.Tick(input, Time.deltaTime);
        }

        private void OnPositionChanged(Vector3 position)
        {
            transform.position = position;
        }

        private void OnRotationChanged(float yaw, float pitch)
        {
            // Free-режим орієнтацію не використовує — застосовуємо лише
            // коли rig її дійсно виставляє (Follow/ThirdPerson).
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
