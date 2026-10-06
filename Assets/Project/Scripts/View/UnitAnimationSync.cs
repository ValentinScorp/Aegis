using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;

namespace Aegis.View
{
    public class UnitAnimationSync : MonoBehaviour
    {
        private UnitAnimator _animator;
        private UnitAgentMovement _movement;
        private UnitDirectMovement _directMovement;
        private Unit _unit;

        private void Awake()
        {
            _animator = ComponentResolver.Require(this, GetComponent<UnitAnimator>());
            _movement = ComponentResolver.Require(this, GetComponent<UnitAgentMovement>());
            _directMovement = GetComponent<UnitDirectMovement>();
        }

        public void Bind(Unit unit) => _unit = unit;
        public void Unbind() => _unit = null;

        private void Update()
        {
            if (_unit?.Config == null) return;
            if (!_unit.IsAlive) return;
            
            if (_unit.ControlMode == UnitControlMode.Direct) {
                UpdateDirectMode();
            } else  {
                if (_animator.IsWalking) {
                    _animator.SetWalkSpeed(_movement.NormalizedSpeed * _unit.WalkAnimationSpeedMultiplier);
                }
                _animator.UpdateAimAnimation(_unit.IsAiming, _unit.AimCancelled, Time.deltaTime);
            }
            // Debug.Log($"[{name}] ControlMode={_unit.ControlMode}, IsMoving={_directMovement?.IsMoving}, Speed={_directMovement?.CurrentSpeedNormalized}");

        }
        private void UpdateDirectMode()
        {
            if (_directMovement != null) {
                if (_directMovement.IsMoving)
                    _animator.PlayWalk(_directMovement.CurrentSpeedNormalized * _unit.WalkAnimationSpeedMultiplier);
                else
                    _animator.PlayIdle();
            }
            _animator.UpdateAimAnimation(_unit.IsAiming, _unit.AimCancelled, Time.deltaTime);

            // Не цілимось і шар лука вимкнувся: ховаємо зброю (стоїмо чи йдемо, не важливо)
            if (!_unit.IsAiming && !_animator.IsAimAnimationActive)
                _unit.Weaponry.Holster();
        }
    }
}