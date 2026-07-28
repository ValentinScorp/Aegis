using UnityEngine;

namespace Aegis.Core
{
    public readonly struct UnitActionEvent
    {
        public readonly UnitAction Action;
        public readonly Vector3 TargetPosition;

        public UnitActionEvent(UnitAction action, Vector3 targetPosition)
        {
            Action = action;
            TargetPosition = targetPosition;
        }
    }
}