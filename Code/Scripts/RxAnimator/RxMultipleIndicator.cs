using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MultipleArrow
{
    public float multiple;
    public float from;
    public float to;
}

public class RxMultipleIndicator : RxTweenScript_MultipleIndicator
{
    [SerializeField] private RxIdleAnimator idle;

    public override void StopTween()
    {
        base.StopTween();
        StopArrow();
    }

    private void StopArrow()
    {
        idle.Animator_Out();
    }

    protected override float CompareValue => idle.transform.localPosition.x;
}
