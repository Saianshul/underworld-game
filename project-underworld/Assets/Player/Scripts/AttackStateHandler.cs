using UnityEngine;

public class AttackStateHandler : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Cast"))
        {
            animator.GetComponent<Moves>().StartCast();
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Moves>().FinishAttack();
    }
}
