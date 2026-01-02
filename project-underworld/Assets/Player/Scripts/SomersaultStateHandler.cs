using UnityEngine;

public class SomersaultStateHandler : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Moves>().FinishSomersault();
    }
}
