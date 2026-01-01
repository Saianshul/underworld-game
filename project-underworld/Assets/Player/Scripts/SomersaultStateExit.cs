using UnityEngine;

public class SomersaultStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Movement>().FinishSomersault();
    }
}
