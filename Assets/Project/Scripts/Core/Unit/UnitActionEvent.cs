using UnityEngine;

namespace Aegis.Core
{
    public readonly struct UnitActionEvent
    {
        public readonly UnitActionId Action;
        public readonly Vector3 TargetPosition;

        public UnitActionEvent(UnitActionId action, Vector3 targetPosition)
        {
            Action = action;
            TargetPosition = targetPosition;
        }
    }
}