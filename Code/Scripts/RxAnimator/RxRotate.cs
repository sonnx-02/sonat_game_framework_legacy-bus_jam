using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxRotate : BaseRxAnimator
{
    [SerializeField] private bool activeIn;

    [ConditionalField(nameof(activeIn), new object[] {true}, false, true)] [SerializeField]
    private AnimationData eulerIn = new AnimationData()
    {
        y = 90,
        time = 1,
        ease =  Ease.Linear
    };
    [SerializeField] private bool activeOut;
    [ConditionalField(nameof(activeOut), new object[] {true}, false, true)]
    [SerializeField] private AnimationData eulerOut= new AnimationData()
    {
        time = 1,
        ease =  Ease.Linear
    };
    public override bool HasOut()
    {
        return activeOut;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        transform.localEulerAngles = Vector3.zero;
    }

    public override bool HasIn()
    {
        return activeIn;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return transform.DOLocalRotate(eulerIn.vec,eulerIn.time).SetEase(eulerIn.ease).WaitForCompletion();
        InvokeAfterInAction();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return transform.DOLocalRotate(eulerOut.vec,eulerOut.time).SetEase(eulerOut.ease).WaitForCompletion();
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return transform.DOLocalRotate(eulerIn.vec, eulerIn.time).SetEase(eulerIn.ease).WaitForCompletion();
        InvokeAfterInAction();
        yield return transform.DOLocalRotate(eulerOut.vec,eulerOut.time).SetEase(eulerOut.ease).WaitForCompletion();
    }
}
