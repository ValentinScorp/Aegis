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

            _entityMovement.MovementCompleted += OnMovementComplete;
        }
        private void OnDestroy()
        {
            _entityMovement.MovementCompleted -= OnMovementComplete; 
            Unbind();         
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
                unit.AttackBegin += OnAttack;
                unit.AttackEnd += _entityAnimator.StopAttack;
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
                unit.AttackBegin -= OnAttack;
                unit.AttackEnd -= _entityAnimator.StopAttack;
            }
            _entity = null;
        }

        private void OnChaseTo(Vector3 target)
        {
            _entityMovement.MoveTo(target);
            _entityAnimator.StopAttack();
            _entityAnimator.StopWalk();
            _entityAnimator.PlayChase();
        }
        private void OnWalk(Vector3 destination)
        {
            _entityMovement.MoveTo(destination);
            _entityAnimator.StopAttack();
            _entityAnimator.StopChase();
            _entityAnimator.PlayWalk();
        }
        private void OnAttack(Vector3 targetPosition)
        {
            _entityAnimator.StopChase();
            _entityAnimator.StopWalk();
            _entityMovement.LookAt(targetPosition);            
            _entityAnimator.PlayAttack();
        }
        private void OnMovementComplete(Vector3 pos)
        {
            _entityAnimator.StopChase();
            _entityAnimator.StopWalk();
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

    }
}
