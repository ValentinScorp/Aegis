using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateTransition
    {
        private readonly Unit _unit;
        private readonly UnitStateMachine _sm;

        public UnitStateTransition(Unit unit, UnitStateMachine stateMachine)
        {
            _unit = unit;
            _sm = stateMachine;
        }

        public void Evaluate(WorldEntity closestTarget)
        {
            // 1. Найвищий пріоритет — смерть
            if (!_unit.Health.IsAlive)
            {
                _sm.SetState(UnitState.Dead);
                return;
            }

            // 2. Наказ гравця (Walk) не перебиваємо,
            //    поки юніт сам не завершить рух через MovementComplete
            if (_sm.CurrentType == UnitState.Walk)
                return;

            // 3. Combat
            if (closestTarget != null && IsValidEnemy(closestTarget))
            {
                if (_unit.CanAttack(closestTarget))
                {
                    _unit.AttackTarget = closestTarget;
                    _sm.SetState(UnitState.Attack);
                    return;
                }

                if (ShouldChase(closestTarget))
                {
                    _unit.ChaseTarget = closestTarget;
                    _sm.SetState(UnitState.Chase);
                    return;
                }
            }

            // 4. Повернення на FixedPosition, якщо відійшли занадто далеко
            if (ShouldReturnHome())
            {
                var walk = _sm.GetState<UnitStateWalk>();
                if (walk != null)
                    walk.Destination = _unit.FixedPosition;

                _sm.SetState(UnitState.Walk);
                return;
            }

            // 5. Нічого робити — Idle
            _sm.SetState(UnitState.Idle);
        }

        private bool IsValidEnemy(WorldEntity entity)
        {
            if (entity is not Unit other)
                return false;

            if (other.FactionId == _unit.FactionId)
                return false;

            if (!other.Health.IsAlive)
                return false;

            return true;
        }

        private bool ShouldChase(WorldEntity target)
        {
            float distToTargetSqr = (target.Position - _unit.Position).sqrMagnitude;
            float chaseRadiusSqr = _unit.ChaseRadius * _unit.ChaseRadius;

            // Не біжимо за ціллю, якщо самі вже далеко від дому
            float distToHomeSqr = (_unit.Position - _unit.FixedPosition).sqrMagnitude;
            if (distToHomeSqr > chaseRadiusSqr)
                return false;

            return distToTargetSqr <= chaseRadiusSqr;
        }

        private bool ShouldReturnHome()
        {
            float distSqr = (_unit.Position - _unit.FixedPosition).sqrMagnitude;
            // Невеликий hysteresis, щоб не смикатися на межі
            float threshold = _unit.ChaseRadius * _unit.ChaseRadius * 0.85f;
            return distSqr > threshold;
        }
    }
}