using UnityEngine;

public class AttackStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Movement>().FinishAttack();
    }
}
