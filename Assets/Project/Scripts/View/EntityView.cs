using System;
using UnityEngine;
using UnityEngine.AI;
using Aegis.Core;
using System.Collections;

namespace Aegis.View
{
    public class EntityView : MonoBehaviour
    {
        private EntityMovement _entityMovement;
        private EntityAnimator _entityAnimator;
        private WorldEntity _entity;
        public WorldEntity Entity => _entity;

        private void Awake()
        {
            _entityMovement = GetComponent<EntityMovement>();
            _entityAnimator = GetComponent<EntityAnimator>();
        }
        public void Bind(WorldEntity entity)
        {
            if (entity is null) return;

            _entity = entity;
            transform.position = entity.Position;

            if (entity is Unit unit) {
                _entityMovement.Bind(unit);

                unit.WasSelectedByPlayer += OnPlayerSelection;
                unit.MovedTo += _entityMovement.MoveTo;
                unit.PerformedAttack += _entityAnimator.PlayAttack;
                unit.LookedAt += OnLookAt;
            }
        }

        public void Unbind()
        {
            if (_entity == null) return;

            if (_entity is Unit unit) {
                _entityMovement.Unbind();

                unit.WasSelectedByPlayer -= OnPlayerSelection;
                unit.MovedTo -= _entityMovement.MoveTo;
                unit.PerformedAttack -= _entityAnimator.PlayAttack;
                unit.LookedAt -= OnLookAt;
            }
            _entity = null;
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

        private void OnDestroy() => Unbind();
    }
}
