using UnityEngine;

namespace Aegis.Core
{
    public readonly struct UnitActionEvent
    {
        public readonly UnitAction Action;
        public readonly Vector3 TargetPosition;
        public readonly float Speed;
        public readonly string WeaponAnimation;

        public UnitActionEvent(UnitAction action, Vector3 targetPosition, float speed, string weaponAnimation)
        {
            Action = action;
            TargetPosition = targetPosition;
            Speed = speed;
            WeaponAnimation = weaponAnimation;
        }
    }
}