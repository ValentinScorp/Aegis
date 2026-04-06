using System.Collections.Generic;

namespace Aegis.Core
{
    public interface IUnitState
    {
        void Enter();
        void Exit();
        void OnInteractionsUpdate(WorldEntity closestTarget);
        void OnActionsUpdate(float deltTime);
    }
}