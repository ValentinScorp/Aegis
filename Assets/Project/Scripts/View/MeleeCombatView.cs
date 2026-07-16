using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class MeleeCombatView : MonoBehaviour, ICombatView
    {
        private EntityAnimator _entityAnimator;
        private EntityMovement _entityMovement;
        private Unit _unit;

        private void Awake()
        {
            _entityAnimator = GetComponentInChildren<EntityAnimator>();
            _entityMovement = GetComponent<EntityMovement>();
        }

        public void Bind(Unit unit)
        {
            _unit = unit;
            unit.AttackBegin += OnAttack;
        }
        public void Unbind()
        {
            if (_unit == null) return;
            _unit.AttackBegin -= OnAttack;
            _unit = null;
        }

        private void OnAttack(Vector3 targetPosition)
        {
            _entityMovement.LookAt(targetPosition);
            _entityAnimator.PlayAttack(_unit.AttackTime);
        }
    }
}