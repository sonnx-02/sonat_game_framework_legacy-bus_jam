using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenGemController : MonoBehaviour
{
    [SerializeField] private BaseRxAnimator[] tweens;
    [SerializeField] private RectTransform target;

    [SerializeField] [MyButton(nameof(Test))]
    private int test123;

    public void Test()
    {
        StartTween();
    }

    private void Start()
    {
//        foreach (var baseRxAnimator in tweens)
//        {
//            baseRxAnimator
//        }
    }

    public void SetTarget(RectTransform to)
    {
        target = to;
    }

    [SerializeField] private bool overrideTarget;
    [SerializeField] private bool overrideLastPoint;

    public void StartTween()
    {
        Vector3 local = GetComponent<RectTransform>().InverseTransformPoint(target.position);
        foreach (var baseRxAnimator in tweens)
        {
            if (baseRxAnimator is RxBezierMove)
            {
                RxBezierMove temp = (RxBezierMove) baseRxAnimator;
                if (overrideTarget)
                    temp.SetTarget(target, 0);
                if (overrideLastPoint)
                    temp.SetLastPoint(local);
                temp.Init();
            }
        }

        foreach (var baseRxAnimator in tweens)
            baseRxAnimator.Animator_InThenOut();
    }
}