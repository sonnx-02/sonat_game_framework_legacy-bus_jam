using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxAnimatorTrigger : BaseRxAnimator
{
    [SerializeField] private Animator animator;
    [SerializeField] private string keyIn = "moveIn";
    [SerializeField] private string keyOut = "moveOut";
    [SerializeField] private float timeOut = 0.5f;
    [SerializeField] private float timeIn = 0.5f;
    
    [SerializeField] private float delayIn = 0f;
    [SerializeField] private float delayOut = 0f;
    
    public override bool HasOut()
    {
        return string.IsNullOrEmpty(keyOut);
    }

    public override bool HasIn()
    {
        return string.IsNullOrEmpty(keyIn);
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return new WaitForSeconds(delayIn);

        animator.SetTrigger(keyIn);
        yield return new WaitForSeconds(timeIn);
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return new WaitForSeconds(delayOut);

        animator.SetTrigger(keyOut);
        yield return new WaitForSeconds(timeOut);
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return new WaitForSeconds(delayIn);
        animator.SetTrigger(keyIn);
        yield return new WaitForSeconds(timeIn);
        
        yield return new WaitForSeconds(delayOut);
        animator.SetTrigger(keyOut);
        yield return new WaitForSeconds(timeOut);
    }
}
