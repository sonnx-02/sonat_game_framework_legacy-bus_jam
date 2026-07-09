using System.Collections;
using System.Collections.Generic;
#if use_febucci
using Febucci.UI;
#endif
using UnityEngine;

public class RxAnimatorTextAnimatorVisible : BaseRxAnimator
{
    [SerializeField] private bool[] visibleIn;
    [SerializeField] private bool[] visibleOut;
    [SerializeField] private float intervalIn = 0.1f;
    [SerializeField] private float intervalOut = 0.1f;
    [SerializeField] private float delayAfterIn;
    [SerializeField] private BaseBaseActionScript[] actionAfterIn;
#if use_febucci
    [SerializeField] private TextAnimator_TMP textAnimator;
#endif
    public override bool HasOut()
    {
        return visibleOut.Length > 0;
    }

    public override bool HasIn()
    {
        return visibleIn.Length > 0;
    }

#if !use_febucci
    protected override IEnumerator ThisDoAnimate_In()
    {
        Debug.LogError("use febucci please");
        yield break;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield break;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield break;
    }
#endif


#if use_febucci
    protected override IEnumerator ThisDoAnimate_In()
    {
        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        for (var i = 0; i < visibleIn.Length; i++)
        {
            textAnimator.SetVisibilityChar(i,visibleIn[i]);
            yield return new WaitForSeconds(intervalIn);
        }
        
        if (delayAfterIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(delayAfterIn);
        actionAfterIn.StartAction();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        for (var i = 0; i < visibleOut.Length; i++)
        {
            textAnimator.SetVisibilityChar(i,visibleOut[i]);
            yield return new WaitForSeconds(intervalOut);
        }
        
        if (settings.delayAfterOut > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        for (var i = 0; i < visibleIn.Length; i++)
        {
            textAnimator.SetVisibilityChar(i,visibleIn[i]);
            yield return new WaitForSeconds(intervalIn);
        }
        
        if (delayAfterIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(delayAfterIn);
        actionAfterIn.StartAction();

        for (var i = 0; i < visibleOut.Length; i++)
        {
            textAnimator.SetVisibilityChar(i,visibleOut[i]);
            yield return new WaitForSeconds(intervalOut);
        }
        if (settings.delayAfterOut > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
    }
#endif
}
