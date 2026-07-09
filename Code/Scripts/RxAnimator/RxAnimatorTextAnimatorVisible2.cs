using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if use_febucci
using Febucci.UI;
#endif
using TMPro;
using UnityEngine;

public class RxAnimatorTextAnimatorVisible2 : BaseRxAnimator
{
    [SerializeField] private float intervalIn = 0.1f;
    [SerializeField] private float delayAfterIn;
    [SerializeField] private BaseBaseActionScript[] actionAfterIn;
    [SerializeField] private string defaultText = "00:00";
    [SerializeField] private IndexBindingTextChange text;
#if use_febucci
    [SerializeField] private TextAnimator_TMP textAnimator;
#endif
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }


#if use_febucci
    public override void Prepare()
    {
        base.Prepare();
        textAnimator.SetText(defaultText);
    }


    protected override IEnumerator ThisDoAnimate_In()
    {
        textAnimator.SetText(defaultText);
        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);

        for (var i = 0; i < text.LastText.Length; i++)
        {
            textAnimator.SetVisibilityChar(i,false);
            yield return new WaitForSeconds(intervalIn);
        }
        if (delayAfterIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(delayAfterIn);
        textAnimator.SetText(text.LastText,true);
        for (var i = 0; i < text.LastText.Length; i++)
        {
            textAnimator.SetVisibilityChar(i,true);
            yield return new WaitForSeconds(intervalIn);
        }
        if (delayAfterIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(delayAfterIn);
        textAnimator.SetText(text.LastText);
        actionAfterIn.StartAction();
        
        if (settings.delayAfterOut > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
    }

    private string StringBuilder(string str1,string str2,int changeTo)
    {
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < str2.Length; i++)
            output.Append(i < changeTo ? str1[i] : str2[i]);
        return output.ToString();
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();

    protected override IEnumerator ThisDoAnimate_InThenOut()=> ThisDoAnimate_In();
#else
    protected override IEnumerator ThisDoAnimate_In()
    {
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
}