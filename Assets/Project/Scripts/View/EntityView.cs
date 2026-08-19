using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Aegis.View
{
    public class EntityView : MonoBehaviour
    {
        [SerializeField] private GameObject _bowPrefab;
        [SerializeField] private HealthView _healthView;
        [SerializeField] private ProjectileCatalog _projectileCatalog;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private GameObject _swordPrefab;
        private ICombatView[] _combatViews;
        private Dictionary<WeaponSlotType, WeaponSlotView> _equipmentSlots;

        private Renderer _renderer;
        private EntityMovement _entityMovement;
        private EntityAnimator _entityAnimator;
        private UnitAnimationSync _unitAnimationSync;
        private WorldEntity _entity;
        private UnitWeaponryView _weaponry;
        public WorldEntity Entity => _entity;
        public Unit GetUnit() => _entity as Unit;
        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _healthView = ComponentResolver.Require(this, GetComponentInChildren<HealthView>());
            _entityMovement = GetComponent<EntityMovement>();
            _entityAnimator = GetComponentInChildren<EntityAnimator>();
            _unitAnimationSync = ComponentResolver.Require(this, GetComponent<UnitAnimationSync>());
            _combatViews = GetComponents<ICombatView>();
            if (_projectileCatalog == null) Debug.LogWarning("No <ProjectileCatalog> on Humanoid prefab!");
            if (_projectileSpawnPoint == null) Debug.LogWarning("No projectile spawn point on Humanoid prefab!");

            _equipmentSlots = GetComponents<WeaponSlotView>().ToDictionary(s => s.SlotType);
            _weaponry = ComponentResolver.Require(this, GetComponent<UnitWeaponryView>());

            if ((_renderer = GetComponentInChildren<Renderer>()) == null)
                Debug.LogWarning($"No <Renderer> found in prefab: {name}!", this);

            _entityMovement.MovementCompleted += OnMovementComplete;
        }
        private void OnDestroy()
        {
            _entityMovement.MovementCompleted -= OnMovementComplete;

            Unbind();
        }
        public void Initialize(int factionId)
        {
            _mpb = new MaterialPropertyBlock();

            switch (factionId) {
                case 1: SetFactionColor(Color.red); break;
                case 2: SetFactionColor(Color.blue); break;
                case 3: SetFactionColor(Color.green); break;
                case 4: SetFactionColor(Color.yellow); break;
                default: break;
            }
        }
        public void Bind(WorldEntity entity)
        {
            if (entity is null) return;

            _entity = entity;
            transform.position = entity.Position;

            if (entity is Unit unit) {
                _weaponry.Bind(unit.Weaponry);

                _entityMovement.Bind(unit);
                _entityAnimator.Bind(unit);
                _unitAnimationSync.Bind(unit);

                unit.WasSelectedByPlayer += OnPlayerSelection;
                unit.ChaseTo += OnChaseToAction;
                unit.WalkTo += OnWalkAction;
                unit.ExecutedStopMovement += _entityMovement.Stop;
                unit.ActionPerformed += OnActionPerformed;
                unit.Died += OnDied;
                unit.ProjectileLaunched += OnProjectileLaunched;

                unit.HealthChanged += _healthView.OnHealthChanged;
                unit.Died += _healthView.OnHealthDepleted;


                foreach (var combat in _combatViews)
                    combat.Bind(unit);
            }
        }

        public void Unbind()
        {
            if (_entity == null) return;

            if (_entity is Unit unit) {
                _weaponry.Unbind();
                _entityMovement.Unbind();
                _entityAnimator.Unbind();
                _unitAnimationSync.Unbind();

                unit.WasSelectedByPlayer -= OnPlayerSelection;
                unit.ChaseTo -= OnChaseToAction;
                unit.WalkTo -= OnWalkAction;
                unit.ExecutedStopMovement -= _entityMovement.Stop;
                unit.Died -= OnDied;
                unit.ProjectileLaunched -= OnProjectileLaunched;

                unit.HealthChanged -= _healthView.OnHealthChanged;
                unit.Died -= _healthView.OnHealthDepleted;

                foreach (var combat in _combatViews)
                    combat.Unbind();
            }
            _entity = null;
        }
        private void OnActionPerformed(UnitActionEvent actionEvent)
        {
            var unit = GetUnit();
            if (unit == null) return;

            switch (actionEvent.Action) {
                case UnitAction.Attack:
                    _entityMovement.LookAt(actionEvent.TargetPosition);
                    var weaponAnim = unit.Weaponry.ActiveAnimation;
                    if (!unit.CanShoot
                        && _equipmentSlots.TryGetValue(WeaponSlotType.HandRight, out var mainHandSlot)
                        && _swordPrefab != null) {
                        mainHandSlot.EquipWeapon(_swordPrefab);
                    }
                    _entityAnimator.PlayAttack(weaponAnim, unit.AttackTime);
                    break;
                case UnitAction.Idle:
                    _entityAnimator.PlayIdle();
                    if (_equipmentSlots.TryGetValue(WeaponSlotType.HandRight, out var slot))
                        slot.UnequipWeapon();
                    break;
            }
        }


        private void OnChaseToAction(Vector3 target)
        {
            _entityMovement.MoveTo(target);
            _entityAnimator.PlayWalk(_entityMovement.Velocity);
        }
        private void OnWalkAction(Vector3 destination)
        {
            _entityMovement.MoveTo(destination);
            _entityAnimator.PlayWalk(_entityMovement.Velocity);
        }
        private void OnProjectileLaunched(Vector3 targetPosition)
        {
            if (_entity is Unit unit) {
                string projecitleId = unit.Weaponry.ActiveProjectileId;
                var arrowPrefab = _projectileCatalog.GetPrefab(projecitleId);
                if (unit?.AttackTarget == null || arrowPrefab == null || _projectileSpawnPoint == null) return;
                var arrow = Instantiate(arrowPrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
                arrow.Launch(unit, unit.AttackTarget);
            }
        }
        private void OnMovementComplete(Vector3 pos)
        {
            _entityAnimator.PlayIdle();
        }
        private void OnLookAt(WorldEntity target)
        {
            if (target == null) return;

            _entityMovement.LookAt(target.Position);
        }
        private void OnDied()
        {
            _entityMovement.Stop();
            _entityMovement.DisableAgent();

            var selectable = GetComponent<Selectable>();
            if (selectable) selectable.Select(false);

            _entityAnimator.PlayDeath();
        }

        private void OnPlayerSelection(bool selected)
        {
            var selectable = GetComponent<Selectable>();
            if (selectable)
                selectable.Select(selected);
            else
                Debug.LogWarning("<Selectable> not found on <EntityView>!");
        }
        private void SetFactionColor(Color color)
        {
            if (_renderer == null) return;

            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_FactionColor", color);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}
