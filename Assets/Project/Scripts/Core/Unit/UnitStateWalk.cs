using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateWalk : IUnitState
    {
        public UnitState State => UnitState.Walk;
        private Unit _self;
        public Vector3 Destination { get; set; }

        public UnitStateWalk(Unit owner)
        {
            _self = owner;
        }
        public void Enter()
        {
            // Debug.Log("Enter Walk");            

            _self.AttackTarget = null;
            _self.ChaseTarget = null;
        }
       
        public void Exit()
        {
            // Debug.Log("Exit Walk");
            
        }

        public void OnActionsUpdate(float deltTime)
        {
            
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            
        }

        
    }
}