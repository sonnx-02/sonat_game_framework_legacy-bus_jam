using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UniRx;

public class RxResizeAnimator : BaseRxAnimator
{
    [Header("Resize In")]
    public Vector2 ResizeInSize;
    public float ResizeInTime = 0.5f;
    public float ResizeInDelay;
    public float ResizeInDelayAfter;
    public Ease ResizeInEase = Ease.Linear;

    [Header("Resize Out")]
    public Vector2 ResizeOutSize;
    public float ResizeOutTime = 0.5f;
    public float ResizeOutDelay;
    public float ResizeOutDelayAfter;
    public Ease ResizeOutEase = Ease.Linear;



    private RectTransform _rectTransform;
    private Vector2 _originSize;
    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override void Register()
    {
        base.Register();
        _rectTransform = GetComponent<RectTransform>();
        _originSize = _rectTransform.sizeDelta;
    }

    public override void ResetAnimation(bool setAcive = false)
    {
        base.ResetAnimation(setAcive);
        _rectTransform.sizeDelta = ResizeInSize;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        busy = true;
        state = RxAnimatorState.MovingIn;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        yield return new WaitForSeconds(settings.DelayBeforeIn);
        yield return new WaitForSeconds(ResizeInDelay);

        float t = 0;
        while (t < ResizeInTime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            _rectTransform.sizeDelta = Vector2.Lerp(ResizeInSize, _originSize, t / ResizeInTime);
        }
        _rectTransform.sizeDelta = Vector2.Lerp(ResizeInSize, _originSize, t / ResizeInTime);

        yield return new WaitForSeconds(ResizeInDelayAfter);


        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        CheckRegister();
        state = RxAnimatorState.MovingOut;
        busy = true;

        yield return new WaitForSeconds(settings.delayAfterOut);
        yield return new WaitForSeconds(ResizeOutDelay);

        float t = 0;
        while (t < ResizeOutTime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            _rectTransform.sizeDelta = Vector2.Lerp(ResizeOutSize, _originSize, t / ResizeOutTime);
        }
        _rectTransform.sizeDelta = Vector2.Lerp(ResizeOutSize, _originSize, t / ResizeOutTime);

        yield return new WaitForSeconds(ResizeOutDelayAfter);

        state = RxAnimatorState.Reset;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        busy = false;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        CheckRegister();
        busy = true;
        state = RxAnimatorState.MovingIn;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        yield return new WaitForSeconds(settings.DelayBeforeIn);
        yield return new WaitForSeconds(ResizeInDelay);

        float t = 0;
        while (t < ResizeInTime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            _rectTransform.sizeDelta = Vector2.Lerp(ResizeInSize, _originSize, t / ResizeInTime);
        }
        _rectTransform.sizeDelta = Vector2.Lerp(ResizeInSize, _originSize, t / ResizeInTime);

        yield return new WaitForSeconds(ResizeInDelayAfter);
        yield return new WaitForSeconds(settings.delayAfterOut);
        yield return new WaitForSeconds(ResizeOutDelay);

        t = 0;
        while (t < ResizeOutTime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            _rectTransform.sizeDelta = Vector2.Lerp(ResizeOutSize, _originSize, t / ResizeOutTime);
        }
        _rectTransform.sizeDelta = Vector2.Lerp(ResizeOutSize, _originSize, t / ResizeOutTime);

        yield return new WaitForSeconds(ResizeOutDelayAfter);

        state = RxAnimatorState.Reset;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        busy = false;
    }
}
