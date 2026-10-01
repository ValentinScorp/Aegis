using Aegis.Core;
using UnityEngine;
using Aegis.Utilities;

namespace Aegis.View
{
    [RequireComponent(typeof(CharacterController))]
    public class UnitDirectMovement : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeedDegPerSec = 720f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _maxTwistAngle = 100f;

        private CharacterController _controller;
        private Unit _unit;
        private Vector3 _pendingDirection;
        private bool _hasPendingDirection;
        private float _verticalVelocity;

        public bool IsMoving { get; private set; }
        public float CurrentSpeedNormalized { get; private set; }

        private void Awake()
        {
            _controller = ComponentResolver.Require(this, GetComponent<CharacterController>());
        }

        private void Update()
        {
            if (_unit == null || !_controller.enabled) return;

            IsMoving = _hasPendingDirection;

            Vector3 direction = _hasPendingDirection ? _pendingDirection : Vector3.zero;
            _hasPendingDirection = false;

            CurrentSpeedNormalized = direction.magnitude;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -1f;
            _verticalVelocity += _gravity * Time.deltaTime;

            Vector3 motion = direction * _unit.MoveSpeed + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);

            Quaternion? targetRot = null;

            if (_unit.IsAiming) {
                Vector3 aim = _unit.AimDirection;
                aim.y = 0f;

                if (direction.sqrMagnitude > 0.0001f && aim.sqrMagnitude > 0.0001f) {
                    // Ноги дивляться в бік руху. Якщо рух майже назад відносно прицілу,
                    // розвертаємо ноги на 180°, щоб торс не скручувався більше за ліміт.
                    Vector3 legs = direction;
                    if (Vector3.Angle(legs, aim) > _maxTwistAngle) legs = -legs;
                    targetRot = Quaternion.LookRotation(legs, Vector3.up);
                } else if (aim.sqrMagnitude > 0.0001f) {
                    // Стоїмо на місці: тіло повністю повертається на ціль, як зараз.
                    targetRot = Quaternion.LookRotation(aim, Vector3.up);
                }
            } else if (direction.sqrMagnitude > 0.0001f)
                targetRot = Quaternion.LookRotation(direction, Vector3.up);

            if (targetRot.HasValue)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot.Value, _rotationSpeedDegPerSec * Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (_unit == null) return;
            _unit.Position = transform.position;
        }

        public void Bind(Unit unit)
        {
            if (unit is null) return;
            _unit = unit;
            _unit.DirectMoveRequested += OnDirectMoveRequested;
        }

        public void Unbind()
        {
            if (_unit == null) return;
            _unit.DirectMoveRequested -= OnDirectMoveRequested;
            _unit = null;
        }

        public void SetActive(bool value)
        {
            _controller.enabled = value;
            enabled = value;
            _hasPendingDirection = false;
            _verticalVelocity = 0f;
        }

        private void OnDirectMoveRequested(Vector3 worldDirection)
        {
            // Debug.Log($"[DirectMovement] event received: {worldDirection}");
            worldDirection.y = 0f;
            _pendingDirection = worldDirection.sqrMagnitude > 1f ? worldDirection.normalized : worldDirection;
            // _hasPendingDirection = true;
            _hasPendingDirection = _pendingDirection.sqrMagnitude > 0.0001f;
        }
    }
}