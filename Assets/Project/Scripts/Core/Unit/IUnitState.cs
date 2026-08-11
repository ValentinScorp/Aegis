using UnityEngine;

namespace Aegis.Core
{
    public interface IUnitState
    {
        UnitState State { get; }

        void Enter();
        void Exit();
        void OnInteractionsUpdate(WorldEntity closestTarget);
        void OnActionsUpdate(float deltTime);
    }
}