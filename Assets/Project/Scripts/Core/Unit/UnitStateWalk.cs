using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateWalk : IUnitState
    {
        private Unit _self;
        public Vector3 Destination { get; set; }

        public UnitStateWalk(Unit owner)
        {
            _self = owner;
        }
        public void Enter(Vector3? destination = null)
        {
            _self.AttackTarget = null;
            _self.ChaseTarget = null;
            _self.ClosestTarget = null;
        }

        public void Exit()
        {
            
        }

        public void OnActionsUpdate(float deltTime)
        {
            
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            
        }
    }
}