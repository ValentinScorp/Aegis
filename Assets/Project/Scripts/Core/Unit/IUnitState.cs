using UnityEngine;

namespace Aegis.Core
{
    public interface IUnitState
    {
        void Enter(Vector3? destination = null);
        void Exit();
        void OnInteractionsUpdate(WorldEntity closestTarget);
        void OnActionsUpdate(float deltTime);
    }
}