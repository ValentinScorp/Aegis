using UnityEngine;
using Aegis.Core;
using Aegis.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Aegis.View
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] FactionPalette _factionPalette;
        [SerializeField] private GameObject _bowPrefab;
        [SerializeField] private HealthView _healthView;
        [SerializeField] private ProjectileCatalog _projectileCatalog;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private GameObject _swordPrefab;
        [SerializeField] private BodyPartId _aimBodyPart = BodyPartId.Torso;
        [SerializeField] private LayerMask _aimRaycastMask;
        private Renderer _renderer;
        private UnitAgentMovement _unitAgentMovement;
        private UnitDirectMovement _unitDirectMovement;
        private UnitAimTwist _unitAimTwist;
        private UnitAnimator _entityAnimator;
        private UnitAnimationSync _unitAnimationSync;
        private WorldEntity _entity;
        private UnitWeaponryView _weaponry;
        private static readonly Dictionary<WorldEntity, UnitView> _views = new();
        private readonly Dictionary<BodyPartId, HitZoneView> _hitZones = new();
        private const bool PLAYER_AIM_DIRECT = true;

        public WorldEntity Entity => _entity;
        public Unit GetUnit() => _entity as Unit;
        private MaterialPropertyBlock _mpb;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _views.Clear();

        private void Awake()
        {
            _healthView = ComponentResolver.Require(this, GetComponentInChildren<HealthView>());
            _unitAgentMovement = GetComponent<UnitAgentMovement>();
            // Не всі юніти мають CharacterController/пряме керування — компонент опційний.
            _unitDirectMovement = GetComponent<UnitDirectMovement>();
            _unitAimTwist = GetComponent<UnitAimTwist>();
            _entityAnimator = GetComponentInChildren<UnitAnimator>();
            _unitAnimationSync = ComponentResolver.Require(this, GetComponent<UnitAnimationSync>());
            if (_projectileCatalog == null) Debug.LogWarning("No <ProjectileCatalog> on Humanoid prefab!");
            if (_projectileSpawnPoint == null) Debug.LogWarning("No projectile spawn point on Humanoid prefab!");

            _weaponry = ComponentResolver.Require(this, GetComponent<UnitWeaponryView>());

            if ((_renderer = GetComponentInChildren<Renderer>()) == null)
                Debug.LogWarning($"No <Renderer> found in prefab: {name}!", this);

            _unitAgentMovement.MovementCompleted += OnMovementComplete;
        }
        private void OnDestroy()
        {
            _unitAgentMovement.MovementCompleted -= OnMovementComplete;

            Unbind();
        }
        public void Initialize(FactionId factionIdid)
        {
            _mpb = new MaterialPropertyBlock();
            SetFactionColor(_factionPalette.GetColor(factionIdid));
        }
        public void Bind(WorldEntity entity)
        {
            if (entity is null) return;

            _entity = entity;
            transform.position = entity.Position;
            _views[entity] = this;   // одразу після _entity = entity;

            if (entity is Unit unit) {
                _weaponry.Bind(unit.Weaponry);

                _unitAgentMovement.Bind(unit);
                _unitDirectMovement?.Bind(unit);
                _unitDirectMovement?.SetActive(unit.ControlMode == UnitControlMode.Direct);
                _unitAimTwist?.Bind(unit);
                _entityAnimator.Bind(unit);
                _unitAnimationSync.Bind(unit);

                unit.ControlModeChanged += OnControlModeChanged;
                unit.WasSelectedByPlayer += OnPlayerSelection;
                unit.ChaseTo += OnChaseToAction;
                unit.WalkTo += OnWalkAction;
                unit.ExecutedStopMovement += _unitAgentMovement.Stop;
                unit.ActionPerformed += OnActionPerformed;
                unit.Died += OnDied;
                unit.ProjectileLaunched += OnProjectileLaunched;
                unit.ShotReleased += OnShotReleased;

                unit.HealthChanged += _healthView.OnHealthChanged;
                unit.Died += _healthView.OnHealthDepleted;

                foreach (var zone in GetComponentsInChildren<HitZoneView>(true)) {
                    zone.Bind(unit);
                    _hitZones[zone.BodyPart] = zone;
                }
            }
        }
        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null) {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }

        public void Unbind()
        {
            if (_entity == null) return;
            _views.Remove(_entity);
            _hitZones.Clear();

            if (_entity is Unit unit) {
                _weaponry.Unbind();
                _unitAgentMovement.Unbind();
                _unitDirectMovement?.Unbind();
                _unitAimTwist?.Unbind();
                _entityAnimator.Unbind();
                _unitAnimationSync.Unbind();

                unit.ControlModeChanged -= OnControlModeChanged;
                unit.WasSelectedByPlayer -= OnPlayerSelection;
                unit.ChaseTo -= OnChaseToAction;
                unit.WalkTo -= OnWalkAction;
                unit.ExecutedStopMovement -= _unitAgentMovement.Stop;
                unit.Died -= OnDied;
                unit.ProjectileLaunched -= OnProjectileLaunched;
                unit.ShotReleased -= OnShotReleased;

                unit.HealthChanged -= _healthView.OnHealthChanged;
                unit.Died -= _healthView.OnHealthDepleted;

                foreach (var zone in GetComponentsInChildren<HitZoneView>(true))
                    zone.Unbind();
            }
            _entity = null;
        }
        private void OnControlModeChanged(UnitControlMode mode)
        {
            bool direct = mode == UnitControlMode.Direct;

            if (direct) {
                _unitAgentMovement.DisableAgent();
                _unitDirectMovement?.SetActive(true);
            } else {
                _unitDirectMovement?.SetActive(false);
                _unitAgentMovement.EnableAgent();
            }

            if (direct && _unitDirectMovement == null)
                Debug.LogWarning($"[EntityView] Unit переведено в Direct-режим, але на префабі '{name}' немає EntityDirectMovement/CharacterController.", this);
        }
        private void OnActionPerformed(UnitActionEvent actionEvent)
        {
            var unit = GetUnit();
            if (unit == null) return;

            switch (actionEvent.Action) {
                case UnitAction.Attack:
                    _unitAgentMovement.LookAt(actionEvent.TargetPosition);
                    var weaponAnim = unit.Weaponry.ActiveAnimation;
                    var ainmSpeed = _entityAnimator.PlayAttack(weaponAnim, unit.AttackTime);
                    _weaponry.GetActiveHandWeapon()?.PlayShootAnimation(ainmSpeed);
                    break;
                case UnitAction.Idle:
                    _entityAnimator.PlayIdle();
                    break;
            }
        }
        private void OnChaseToAction(Vector3 target)
        {
            _unitAgentMovement.MoveTo(target);
            _entityAnimator.PlayWalk(_unitAgentMovement.AgentSpeed);
        }
        private void OnWalkAction(Vector3 destination)
        {
            _unitAgentMovement.MoveTo(destination);
            _entityAnimator.PlayWalk(_unitAgentMovement.AgentSpeed);
        }
        private void OnProjectileLaunched(Vector3 targetPosition)
        {
            if (_entity is Unit unit) {
                var target = unit.AttackTarget;
                var arrowPrefab = _projectileCatalog.GetPrefab(unit.Weaponry.ActiveProjectileId);
                if (target == null || arrowPrefab == null || _projectileSpawnPoint == null) return;

                var arrow = Instantiate(arrowPrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
                var aimPoint = GetAimPointOn(target);
                Debug.DrawLine(_projectileSpawnPoint.position, aimPoint, Color.green, 3f);
                arrow.Launch(unit, aimPoint, target.Velocity);
            }
        }
        private Vector3 GetAimPointOn(WorldEntity target)
        {
            return _views.TryGetValue(target, out var view)
                ? view.GetAimPoint(_aimBodyPart)
                : target.Position + Vector3.up;
        }
        public Vector3 GetAimPoint(BodyPartId part)
        {
            if (TryGetCenter(part, out var center)) {
                return center;
            }
            if (TryGetCenter(BodyPartId.Torso, out center)) {
                return center;
            }

            return transform.position + Vector3.up;   // запасний варіант, якщо зон нема
        }
        private void OnShotReleased()
        {
            if (_entity is not Unit unit) return;

            var arrowPrefab = _projectileCatalog.GetPrefab(unit.Weaponry.ActiveProjectileId);
            if (arrowPrefab == null || _projectileSpawnPoint == null) return;

            if (PLAYER_AIM_DIRECT) {
                Vector3 dir = Camera.main != null ? Camera.main.transform.forward : _projectileSpawnPoint.forward;
                var arrow = Instantiate(arrowPrefab, _projectileSpawnPoint.position, Quaternion.LookRotation(dir));
                arrow.LaunchDirection(unit, dir);
            } else {
                Vector3 aimPoint = GetScreenCenterAimPoint();
                var arrow = Instantiate(arrowPrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);
                arrow.Launch(unit, aimPoint, Vector3.zero);
            }
        }
        private Vector3 GetScreenCenterAimPoint()
        {
            var cam = Camera.main;
            if (cam == null)
                return _projectileSpawnPoint.position + _projectileSpawnPoint.forward * 30f;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _aimRaycastMask, QueryTriggerInteraction.Collide))
                return hit.point;

            return ray.origin + ray.direction * 60f;
        }

        private bool TryGetCenter(BodyPartId part, out Vector3 center)
        {
            if (_hitZones.TryGetValue(part, out var zone) && zone.TryGetComponent(out CapsuleCollider cap)) {
                center = zone.transform.TransformPoint(cap.center);
                return true;
            }
            center = Vector3.zero;
            return false;
        }

        // Точка, в яку цей юніт цілиться по ворогу

        private void OnMovementComplete(Vector3 pos)
        {
            _entityAnimator.PlayIdle();
        }
        private void OnLookAt(WorldEntity target)
        {
            if (target == null) return;

            _unitAgentMovement.LookAt(target.Position);
        }
        private void OnDied()
        {
            _unitAgentMovement.Stop();
            _unitAgentMovement.DisableAgent();

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
