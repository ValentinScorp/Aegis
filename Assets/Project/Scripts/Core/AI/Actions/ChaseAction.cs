using UnityEngine;

namespace Aegis.Core.AI
{
    public class ChaseAction : BTAction
    {
        private WorldEntity _target; 
        private Vector3 _last;

        protected override void OnEnter(BTContext c)
        {
            var u = c.Unit;
            _target = u.CurrentTarget;
            u.ChaseTarget = _target;
            u.IsChasing = true;
            _last = _target.Position;
            u.PerformChase(_target);
        }
        protected override BTStatus OnTick(BTContext c)
        {
            if (_target == null || c.Unit.CurrentTarget != _target) return BTStatus.Failure;
            if ((_target.Position - _last).sqrMagnitude > 0.25f)   // перепрокладаємо шлях, якщо ціль зрушила >0.5 м
            {
                _last = _target.Position;
                c.Unit.PerformChase(_target);
            }
            return BTStatus.Running;
        }
        protected override void OnExit(BTContext c) => c.Unit.IsChasing = false;
    }
}