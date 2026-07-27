using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;

namespace Aegis.View
{
    public class UnitAnimationSync : MonoBehaviour
    {
        private EntityAnimator _animator;
        private EntityMovement _movement;
        private Unit _unit;

        private void Awake()
        {
            _animator = ComponentResolver.Require(this, GetComponent<EntityAnimator>());
            _movement = ComponentResolver.Require(this, GetComponent<EntityMovement>());
        }

        public void Bind(Unit unit) => _unit = unit;
        public void Unbind() => _unit = null;

        private void Update()
        {
            if (!_animator.IsWalking || _unit?.Config == null) return;
            _animator.SetWalkSpeed(_movement.NormalizedSpeed * _unit.WalkAnimationSpeedMultiplier);
        }
    }
}