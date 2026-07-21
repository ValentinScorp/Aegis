using UnityEngine;
using Aegis.Core;

namespace Aegis.View
{
    public class EntityView : MonoBehaviour
    {
        [SerializeField] private HealthView _healthView;
        private ICombatView[] _combatViews;

        private Renderer _renderer;
        private EntityMovement _entityMovement;
        private EntityAnimator _entityAnimator;
        private WorldEntity _entity;
        public WorldEntity Entity => _entity;

        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _healthView = Utilities.ComponentResolver.ResolveOrFind(this, _healthView);
            _entityMovement = GetComponent<EntityMovement>();
            _entityAnimator = GetComponentInChildren<EntityAnimator>();
            _combatViews = GetComponents<ICombatView>();

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
                _entityMovement.Bind(unit);
                unit.WasSelectedByPlayer += OnPlayerSelection;
                unit.ChaseTo += OnChaseTo;
                unit.WalkTo += OnWalk;
                unit.ExecutedStopMovement += _entityMovement.Stop;
                unit.AttackEnd += _entityAnimator.PlayIdle;
                unit.ShootEnd += _entityAnimator.PlayIdle;

                unit.Health.Changed += _healthView.OnHealthChanged;
                unit.Health.Depleted += _healthView.OnHealthDepleted;

                foreach (var combat in _combatViews)
                    combat.Bind(unit);
            }
        }

        public void Unbind()
        {
            if (_entity == null) return;

            if (_entity is Unit unit) {
                _entityMovement.Unbind();

                unit.WasSelectedByPlayer -= OnPlayerSelection;
                unit.ChaseTo -= OnChaseTo;
                unit.WalkTo -= OnWalk;
                unit.ExecutedStopMovement -= _entityMovement.Stop;
                unit.AttackEnd -= _entityAnimator.PlayIdle;
                unit.ShootEnd -= _entityAnimator.PlayIdle;

                unit.Health.Changed -= _healthView.OnHealthChanged;
                unit.Health.Depleted -= _healthView.OnHealthDepleted;

                foreach (var combat in _combatViews)
                    combat.Unbind();
            }
            _entity = null;
        }

        private void OnChaseTo(Vector3 target)
        {
            _entityMovement.MoveTo(target);
            _entityAnimator.PlayWalk(_entityMovement.Velocity);
        }
        private void OnWalk(Vector3 destination)
        {
            _entityMovement.MoveTo(destination);
            _entityAnimator.PlayWalk(_entityMovement.Velocity);
        }
        private void OnAttack(Vector3 targetPosition)
        {
            _entityMovement.LookAt(targetPosition);
            float attackTime = 1f;
            if (_entity is Unit unit) {
                attackTime = unit.AttackTime;
            }
            _entityAnimator.PlayAttack(attackTime);
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
