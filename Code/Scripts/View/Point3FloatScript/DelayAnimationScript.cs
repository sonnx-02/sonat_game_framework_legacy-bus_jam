using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DelayAnimationScript : Point3FloatScript
{
    public Animator[] animators;
    public string startTrigger = "idle";
    public string afterDelayTrigger = "run";

    public void SetDelay(float delay)
    {
        foreach (var animator in animators)
        {
            animator.SetTrigger(startTrigger);
        }
        Observable.Timer(TimeSpan.FromSeconds(delay))
            .Subscribe(data =>
            {
                foreach (var animator in animators)
                {
                    animator.SetTrigger(afterDelayTrigger);
                }
            });
    }

    public override void SetParam(Point3Float para)
    {
        SetDelay(para.Value1);
    }
}
