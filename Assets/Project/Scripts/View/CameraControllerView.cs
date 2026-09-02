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

        private CameraController _controller;

        private void Awake()
        {
            _inputListener = Utilities.ComponentResolver.ResolveOrFind(this, _inputListener);
            _controller = new CameraController(transform.position, _moveSpeed, _verticalSpeed);
        }

        private void OnEnable()
        {
            _controller.PositionChanged += OnPositionChanged;
        }

        private void OnDisable()
        {
            _controller.PositionChanged -= OnPositionChanged;
        }

        private void Update()
        {
            if (_inputListener == null) return;

            Vector2 moveInput = _inputListener.CameraMoveInput;
            float verticalInput = _inputListener.CameraVerticalInput;

            _controller.Tick(moveInput, verticalInput, Time.deltaTime);
        }

        private void OnPositionChanged(Vector3 position)
        {
            transform.position = position;
        }
    }
}