using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ProgressScript : BaseFrameworkView
{
    [SerializeField] private bool disable;
    public ProgressScript[] others;
    [SerializeField] private BoolFloat tweenTimeDelay;
    
    public virtual void OnChanged(float val)
    {
        if (useSpeed.value)
            OnChangedTween(val, 1);
        else
        {
            Trigger(val);
            foreach (var toggleScript in others)
                toggleScript.OnChanged(val);
        }
    }

    public virtual void Init()
    {
        
    }

    private IDisposable _tween;
    [SerializeField] protected BoolFloat useSpeed = new BoolFloat(false,1); // 100 value is 1 seconds
    [SerializeField] protected float timeClamp = .5f;
    [SerializeField] private bool tweenBackward;
    public virtual void OnChangedTween(float val,float time)
    {
        if (disable)
            return;
        
        var from = _last;
        _last = val;
        _tween?.Dispose();
        if (val > from || tweenBackward)
        {
            if (useSpeed.value)
            {
                time = (val - from) / useSpeed.value2;
                if (time > timeClamp)
                    time = timeClamp;
                time = Mathf.Abs(time);
            }

            if (tweenTimeDelay.value)
            {
                _tween = Observable.FromCoroutine(() => DuongExtensions.IEDoValue(from, val, time, t => 
                {
                    Trigger(t);
                    foreach (var toggleScript in others)
                        toggleScript.OnChanged(t);
                },tweenTimeDelay.value2)).Subscribe();
            }
            else
            {
                _tween = Observable.FromCoroutine(() => DuongExtensions.IEDoValue(from, val, time, t => 
                {
                    Trigger(t);
                    foreach (var toggleScript in others)
                        toggleScript.OnChanged(t);
                })).Subscribe();
            }
        }
        else
        {
            Trigger(val);
            foreach (var toggleScript in others)
                toggleScript.OnChanged(val);
        }
    }

    [MyButtonInt(nameof(OnProgress100))] public int abc;

    public void OnProgress100(int a)
    {
        OnChanged(a / 100f);
    }

    private float _last;
    protected virtual void Trigger(float val)
    {
        _last = val;
    }

    public virtual void TriggerToggle(float val, bool reverse)
    {
        foreach (var toggleScript in others)
            toggleScript.TriggerToggle(val,reverse);
    }

    public virtual void OnComplete()
    {
        
    }
    
}