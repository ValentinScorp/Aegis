using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;

namespace Aegis.View
{
    public class RangedCombatView : MonoBehaviour, ICombatView
    {
        [SerializeField] private Transform _arrowSpawnPoint;
        [SerializeField] private Arrow _arrowPrefab;
        [SerializeField] private AnimationClip _shootClip;

        private EntityAnimator _entityAnimator;
        private EntityMovement _entityMovement;
        private EquipmentSlotView _weaponSlot;
        private Unit _unit;
        private float _clipLength;

        private void Awake()
        {
            if ((_entityAnimator = GetComponentInChildren<EntityAnimator>()) == null)
                Debug.LogWarning($"No <EntityAnimator> found in component <RangedCombatView> of prefab: {name}!", this);

            if ((_entityMovement = GetComponent<EntityMovement>()) == null)
                Debug.LogWarning($"No <EntityMovement> found in component <RangedCombatView> of prefab: {name}!", this);

            //_weaponSlot = System.Array.Find(GetComponents<EquipmentSlotView>(), s => s.Type == EquipmentSlotType.HandLeft);
             _weaponSlot = ComponentResolver.Require(this, GetComponent<EquipmentSlotView>());

            if (_shootClip == null)
                Debug.LogWarning($"No <AnimationClip> set in component <RangedCombatView> of prefab: {name}!", this);

            _clipLength = _shootClip != null ? _shootClip.length : 1f;
        }

        public void Bind(Unit unit)
        {
            _unit = unit;
            unit.ShootBegin += OnShoot;
            unit.ProjectileLaunched += OnProjectileLaunched;
        }
        public void Unbind()
        {
            if (_unit == null) return;
            _unit.ShootBegin -= OnShoot;
            _unit.ProjectileLaunched -= OnProjectileLaunched;

            _unit = null;
        }

        private void OnShoot(Vector3 targetPosition)
        {
            _entityMovement.LookAt(targetPosition);
            if (_clipLength > 0 && _unit.AttackTime > 0) {
                float animSpeed = _clipLength / _unit.AttackRange;
                _entityAnimator.PlayShoot(animSpeed);
                _weaponSlot?.PlayBowShoot(animSpeed);
            }
        }
        private void OnProjectileLaunched(Vector3 targetPosition)
        {
            if (_unit?.AttackTarget == null || _arrowPrefab == null) return;
Debug.Log("Launch");
            Transform spawn = _arrowSpawnPoint != null ? _arrowSpawnPoint : transform;
            var arrow = Instantiate(_arrowPrefab, spawn.position, spawn.rotation);
            arrow.Launch(_unit, _unit.AttackTarget);
        }
    }
}