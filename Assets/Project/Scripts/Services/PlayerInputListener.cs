using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aegis.Services
{
    public class PlayerInputListener : MonoBehaviour
    {
        private PlayerInputActions _inputActions;
        private HotkeyListener _hotkeyListener;
        public event Action<Vector2> TapPerformed;
        public event Action AttackPerformed;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _hotkeyListener = new HotkeyListener();
            _hotkeyListener.AttackPressed += () => AttackPerformed?.Invoke();
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
        private void Update()
        {
            _hotkeyListener.Update();
        }
        private void OnTapPerformed(InputAction.CallbackContext context)
        {
            Vector2 screenPosition = _inputActions.Gameplay.Point.ReadValue<Vector2>();
            TapPerformed?.Invoke(screenPosition);
            // Debug.Log("Tap at: " + screenPosition);
        }
    }

    public class HotkeyListener
    {
        public event Action AttackPressed;
        public void Update()
        {
            if (Keyboard.current != null && Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                AttackPressed?.Invoke();
                Debug.Log("Key Q pressed");
            }
        }
    }

}
