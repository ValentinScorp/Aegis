using UnityEngine;

namespace Aegis.Core.AI
{
    public class IdleAction : BTAction
    {
        protected override void OnEnter(BTContext c)
        {
            c.Unit.StopMovement();
            c.Unit.StopAttackAction();     // тут запускаємо Idle-анімацію, а не в OnExit атаки
        }
        protected override BTStatus OnTick(BTContext c) => BTStatus.Running;
    }
}