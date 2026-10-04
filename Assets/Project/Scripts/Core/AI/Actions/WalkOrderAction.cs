using UnityEngine;

namespace Aegis.Core.AI
{
    public class WalkOrderAction : BTAction   // наказ гравця
    {
        private Vector3 _dest;

        protected override void OnEnter(BTContext c)
        {
            var u = c.Unit;
            u.CurrentTarget = null;
            u.AttackTarget = null;
            u.ChaseTarget = null;
            Issue(u);
        }
        protected override BTStatus OnTick(BTContext c)
        {
            var u = c.Unit;
            if (u.PlayerOrder is not Vector3 d) 
                return BTStatus.Success;

            if (d != _dest) 
                Issue(u);                       // гравець клікнув ще раз
            
            if (u.MoveFinished) {
                u.PlayerOrder = null;
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }
        private void Issue(Unit u)
        {
            _dest = u.PlayerOrder.Value;
            u.MoveFinished = false;
            u.RaiseWalk(_dest);
        }
    }
}