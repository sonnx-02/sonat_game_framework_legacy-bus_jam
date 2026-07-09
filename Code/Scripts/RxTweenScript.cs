using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;


public class RxTweenScript : BaseTweenScript
{
    [SerializeField] private Vector3 to1;
    [SerializeField] private Vector3 to2;
    [SerializeField] private float fromT = .5f;
    [SerializeField] private float time = 1;
    [SerializeField] private float delay = 1;
    [SerializeField] private bool reverse;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private Type type;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private bool canStopTween;

    private IDisposable ieTween;

    private enum Type
    {
        LocalPosition,
        LocalEuler,
        LocalScale,
        AnchoredPosition,
    }

    public override void Tween()
    {
        base.Tween();
        ApplyT(reverse ? 1 - fromT : fromT);
        ieTween = Observable.FromCoroutine(IeTween).TakeUntilDisable(this).Subscribe();
    }

    public void DisposeTween()
    {
        if (ieTween != null && canStopTween)
        {
            ieTween.Dispose();
        }
    }

    private IEnumerator IeTween()
    {
        float timeMove = fromT * time;
        bool flipMove = reverse;

        yield return new WaitForSeconds(delay);
        
        while (true)
        {
            if (timeMove > time)
            {
                flipMove = !flipMove;
                timeMove = timeMove % time;
            }

            var t = flipMove
                ? (1 - timeMove / time)
                : timeMove / time;

            t = moveCurve.Evaluate(t);
            ApplyT(t);
            yield return null;
            timeMove += Time.deltaTime;
        }
    }

    [MyButtonFloatSliderAttribute(nameof(ApplyT))] [SerializeField]
    private float testT;

    protected float lastT;

    protected void ApplyT(float t)
    {
        lastT = t;
        switch (type)
        {
            case Type.LocalPosition:
                transform.localPosition = Vector3.Lerp(to1, to2, t);
                break;
            case Type.LocalEuler:
                transform.localEulerAngles = Vector3.Lerp(to1, to2, t);
                break;
            case Type.LocalScale:
                transform.localScale = Vector3.Lerp(to1, to2, t);
                break;
            case Type.AnchoredPosition:
                rectTransform.anchoredPosition = Vector3.Lerp(to1, to2, t);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}