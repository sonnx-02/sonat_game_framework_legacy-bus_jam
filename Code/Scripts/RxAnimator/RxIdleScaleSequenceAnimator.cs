using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

[Serializable]
public struct ScaleSequenceData
{
    public Vector3 value;
    public float time;
    public AnimationCurve curve;
}

public class RxIdleScaleSequenceAnimator : BaseRxAnimator
{
    [SerializeField] private float waitToOut;

    // Idle
    [SerializeField] private ScaleSequenceData[] sequences;
    
    
    private bool _stop;
    
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        DOTween.Kill(this);
        if (sequences.Length > 1)
            transform.localScale = sequences[0].value;
    }

    void OnDisable()
    {
        DOTween.Kill(this); 
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        ResetAnimation();
        Observable.FromCoroutine(DoAnimate_Loop).TakeUntilDisable(this).Subscribe();
        yield return null;
    }

    public IEnumerator DoAnimate_Loop()
    {
        CheckRegister();
        busy = true;
        state = RxAnimatorState.MovingIn;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        yield return new WaitForSeconds(settings.DelayBeforeIn);

        int sequenceIndex = 1;

        if (sequences.Length > 1)
        {
            while (!_stop)
            {
                yield return transform.DOScale(sequences[sequenceIndex].value, sequences[sequenceIndex].time)
                    .SetEase(sequences[sequenceIndex].curve).WaitForCompletion();
                sequenceIndex++;
                if (sequenceIndex == sequences.Length)
                    sequenceIndex = 0;
            }
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return waitToOut;
        _stop = true;
        ResetAnimation();
        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        ResetAnimation();
        Observable.FromCoroutine(DoAnimate_Loop).TakeUntilDisable(this).Subscribe();
        yield return null;
    }
}