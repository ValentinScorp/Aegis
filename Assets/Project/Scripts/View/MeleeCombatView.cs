using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class MeleeCombatView : MonoBehaviour, ICombatView
    {
        [SerializeField] private AnimationClip _aminationClip;
        private EntityAnimator _entityAnimator;
        private EntityMovement _entityMovement;
        private Unit _unit;
        private float _clipLength;

        private void Awake()
        {
            if ((_entityAnimator = GetComponentInChildren<EntityAnimator>()) == null)
                Debug.LogWarning($"No <EntityAnimator> found in component <MeleeCombatView> of prefab: {name}!", this);

            if ((_entityMovement = GetComponent<EntityMovement>()) == null)
                Debug.LogWarning($"No <EntityMovement> found in component <MeleeCombatView> of prefab: {name}!", this);

            if (_aminationClip == null)
                Debug.LogWarning($"No <AnimationClip> set in component <MeleeCombatView> of prefab: {name}!", this);

            _clipLength = _aminationClip != null ? _aminationClip.length : 1f;
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
            if (_clipLength > 0 && _unit.AttackTime > 0) {
                float animSpeed = _clipLength / _unit.AttackTime;
                _entityAnimator.PlayAttack(animSpeed);
            }
        }
    }
}