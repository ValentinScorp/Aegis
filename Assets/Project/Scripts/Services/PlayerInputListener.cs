using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aegis.Services
{
    public class PlayerInputListener : MonoBehaviour
    {
        private PlayerInputActions _inputActions;
        public event Action<Vector2> TapPerformed;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            // ServiceLocator.Register(this);
        }
        private void OnDestroy()
        {
            // ServiceLocator.Unregister<PlayerInputListener>();
        }
        private void OnEnable()
        {
            _inputActions.Gameplay.Enable();
            _inputActions.Gameplay.Tap.performed += OnTapPerformed;
        }
        private void OnDisable()
        {
            _inputActions.Gameplay.Tap.performed -= OnTapPerformed;
            _inputActions.Gameplay.Disable();
        }
        private void OnTapPerformed(InputAction.CallbackContext context)
        {        
            Vector2 screenPosition = _inputActions.Gameplay.Point.ReadValue<Vector2>();
            TapPerformed?.Invoke(screenPosition);
            // Debug.Log("Tap at: " + screenPosition);
        }
    }
}
