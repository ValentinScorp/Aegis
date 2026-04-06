using System.Collections.Generic;

namespace Aegis.Core
{
    public class UnitStateWalk : IUnitState
    {
        private Unit _owner;

        public UnitStateWalk(Unit owner)
        {
            _owner = owner;
        }
        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void OnActionsUpdate(float deltTime)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            throw new System.NotImplementedException();
        }
    }
}