using UnityEngine;

namespace Aegis.Core.AI
{
    public class ReturnHomeAction : BTAction
    {
        protected override void OnEnter(BTContext c)
        {
            var u = c.Unit;
            u.CurrentTarget = null; 
            u.AttackTarget = null; 
            u.ChaseTarget = null;
            u.MoveFinished = false;
            u.PerformReturnHome();
        }
        protected override BTStatus OnTick(BTContext c) {
            return c.Unit.MoveFinished || c.Unit.IsInPerimeter(c.Unit.Position, 1.5f) ? BTStatus.Success : BTStatus.Running;
        }
    }
}