using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TweenDisplayScriptProgress : TweenDisplayScript
{
//    [SerializeField] private Image imgFill;

    [SerializeField] protected ProgressScript[] progresses;
    [SerializeField] private ToggleScript[] toggleOnTweens;
    [SerializeField] private ToggleScript[] onTweenAnimComplete;
    [SerializeField] private float delay;
    [SerializeField] private float speed = 10;

    private IDisposable _disposable;

    private bool _finish;

    public override void Start()
    {
        base.Start();
        toggleOnTweens.OnChanged(false);
    }

    private void Progress(float t)
    {
        foreach (var script in progresses)
            script.OnChanged(t);
    }

    public override void SetDisplayValue(int value)
    {
        if (TargetValue <= 0)
        {
            Progress(1);
            ProgressInt(value);
        }
        else
        {
            Progress(value * 1f / TargetValue);
            ProgressInt(value);
        }
    }

    public override void SetTo(int value)
    {
        base.SetTo(value);
#if UNITY_EDITOR
        //if (TargetValue == 0)
        //    Debug.LogError($"Target not set " + gameObject, gameObject);
#endif
        float t = TargetValue == 0 ? 0f : value * 1f / TargetValue;
        Progress(t);
        ProgressInt(displayValue);
    }

    public override float DoAdd(int val)
    {
        CheckKernelLoaded();
        if (val < 0 && instantSub)
        {
            SetTo(trueValue + val);
            return 0;
        }

        _add += val;
        trueValue += val;
        if (!_busy)
        {
            Observable.FromMicroCoroutine(TweenEnumerator)
                .TakeUntilDisable(this)
                .Subscribe();
        }

        return 0;
    }

    private void OnDisable()
    {
        _busy = false;
    }


    public override void ResetValue(int value)
    {
        base.ResetValue(value);

        MyIEnumeratorExtensions.IoValue(0, value, speed, x => { Progress(x / TargetValue); })
            .TakeUntilDisable(this)
            .Subscribe(data => { });
    }

    public float time = 2f;

    public override void ResetValue2(int value)
    {
        base.ResetValue2(value);
        Observable.FromCoroutine(() => MoveCoroutine(value)).Subscribe();
    }

    public override void ResetValue3(int value)
    {
        base.ResetValue2(value);
        DoAdd(value);
    }


    private IEnumerator MoveCoroutine(float value)
    {
        float t = 0.0f;

        while (t < time)
        {
            t += Time.deltaTime;
            Progress(t / time * (value / TargetValue));
            yield return null;
        }
    }


    private bool _busy;
    private float _add;

    [SerializeField] private int intervalSound = 100;
    [SerializeField] private float velocity = 0f;
    private IEnumerator TweenEnumerator()
    {
        _busy = true;
        toggleOnTweens.OnChanged(true);
        onTweenAnimComplete.OnChanged(false);
        float t0 = 0;

        float countDownSound = intervalSound / 1000f;
        float tSound = 0f;
        while (t0 < delay)
        {
            t0 += Time.deltaTime;
            PlaySound();
            yield return null;
        }

        float display = displayValue;
        int lastDisplayInt = (int) display;
        _onValueUpdate?.Invoke(lastDisplayInt);
        float sp = speed;
        if (_add > 0)
            do
            {
                var t = Time.deltaTime * sp;
                if (velocity > 0.01f)
                    sp += velocity * Time.deltaTime;
                if (t > _add)
                    t = _add;
                _add -= t;
                display += t;

                Progress(display / TargetValue);
                displayValue = (int) display;
                ProgressInt(displayValue);
                if (lastDisplayInt != displayValue)
                {
                    _onValueUpdate?.Invoke(displayValue);
                    lastDisplayInt = displayValue;
                }

                yield return null;
                PlaySound();

            } while (_add > 0);
        else
        {
            while (_add < 0)
            {
                var t = Time.deltaTime * speed;
                _add += t;
                display -= t;
                Progress(display / TargetValue);
                displayValue = (int) display;
                ProgressInt(displayValue);
                yield return null;
                PlaySound();
            }
        }
        
        Progress(trueValue *1f / TargetValue);
        displayValue = trueValue;
        ProgressInt(trueValue);

        CheckFinishTarget();
        onTweenAnimComplete.OnChanged(true);
        toggleOnTweens.OnChanged(false);
        _add = 0;
        _busy = false;
        OnCompleteTween?.Invoke();

        void PlaySound()
        {
            tSound += Time.deltaTime;
            if (tSound > countDownSound)
            {
                tSound = 0;
                RootView.rootView.audioManager.Play(scoreSound); 
            }
        }
    }
}