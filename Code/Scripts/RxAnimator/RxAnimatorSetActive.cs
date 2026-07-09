using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxAnimatorSetActive : BaseRxAnimator
{
    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        gameObject.SetActive(false);
        transform.DOKill();

        settings.others.ForEach(x =>
        {
            x.ResetAnimation();
        });
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return new WaitForSecondsRealtime(settings.DelayBeforeIn);
        gameObject.SetActive(true);
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return new WaitForSecondsRealtime(settings.delayAfterOut);
        gameObject.SetActive(false);
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        gameObject.SetActive(false);
        yield return settings.DelayBeforeIn;
        gameObject.SetActive(true);
        yield return settings.delayAfterOut;
        gameObject.SetActive(false);
    }
}
