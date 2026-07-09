using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxProgressScriptHandler : BaseRxAnimator
{
    [SerializeField] private ProgressScript[] progresses;
    [SerializeField] private float time = 1f;

    [SerializeField] private AnimationCurve curve
        = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    
    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        busy = true;
        state = RxAnimatorState.MovingIn;
        
        if(settings.DelayBeforeIn> 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
  
        
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime; 
            float d = curve.Evaluate(t / time);
            foreach (var progressScript in progresses)
                progressScript.OnChanged(d);
            yield return null;
        }

        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        CheckRegister();
        busy = true;
        
        state = RxAnimatorState.MovingIn;
        
        if(settings.DelayBeforeIn> 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime; 
            float d = curve.Evaluate(t / time);
            foreach (var progressScript in progresses)
                progressScript.OnChanged(d);
            yield return null;
        }

        foreach (var progressScript in progresses)
            progressScript.OnChanged(1);

        if (settings.delayAfterOut> 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        
        state = RxAnimatorState.Reset;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        busy = false;
    }
}
