using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxProgressIdle : BaseRxAnimator
{
    [SerializeField] protected int times = -1;
    [SerializeField] protected float duration = 0.2f;
    [SerializeField] protected Vector2 delayRepeatRandom;
    [SerializeField] protected ProgressScript[] progressScripts;
    [SerializeField] protected bool unScaledTime;

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void LogCustom()
    {
        base.LogCustom();
        DOTween.To(() => 0, x =>
        {
            foreach (var progressScript in progressScripts)
                progressScript.OnChanged(x);
        }, 1f, duration);
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        foreach (var progressScript in progressScripts)
            progressScript.OnChanged(0);
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        state = RxAnimatorState.MovingIn;

        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        ResetAnimation();

        int count = 0;
        float t = 0;

        if (duration > 0)
        {
            if (!unScaledTime)
                yield return new WaitForSeconds(delayRepeatRandom.GetRandom());
            else
                yield return CoroutineUtilities.WaitForRealTime(delayRepeatRandom.GetRandom());

            while (count != times)
            {
                while (t < duration)
                {
                    if (!unScaledTime)
                        t += Time.deltaTime;
                    else
                        t += Time.unscaledDeltaTime;
                    var d = t / duration;
                    foreach (var progressScript in progressScripts)
                        progressScript.OnChanged(d);
                    OnProgress(d);
                    yield return null;
                }

                if (!unScaledTime)
                    yield return new WaitForSeconds(delayRepeatRandom.GetRandom());
                else
                    yield return CoroutineUtilities.WaitForRealTime(delayRepeatRandom.GetRandom());

                if (t > duration)
                {
                    foreach (var progressScript in progressScripts)
                        progressScript.OnComplete();
                    OnCompleteLapse();
                    count += (int) (t / duration);
                    t = t % duration;
                }
            }
        }

        OnDone();
        foreach (var progressScript in progressScripts)
            progressScript.OnChanged(0);
        TriggerAfterInParticle(settings.DelayBeforeIn - settings.particleTimeAdjust);
        InvokeCallBack();
        state = RxAnimatorState.Idle;
    }

    protected virtual void OnProgress(float t)
    {
    }

    protected virtual void OnCompleteLapse()
    {
    }

    public virtual void OnDone()
    {
    }


    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}