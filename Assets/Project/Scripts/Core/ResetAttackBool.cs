using UnityEngine;

public class ResetAttackBool : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("PerformAttack");
    }
}