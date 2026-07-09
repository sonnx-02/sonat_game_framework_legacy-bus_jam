using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ProgressScriptInt : BaseFrameworkView
{
    [FormerlySerializedAs("others")] 
    [SerializeField] private ProgressScriptInt[] intProgresses;
    [FormerlySerializedAs("others2")] 
    [SerializeField] private ProgressScript[] progresses;
    
    private int _target;
    public virtual int Target => _target;

    public virtual void SetTarget(int value)
    {
        _target = value;
    }

    public virtual bool IsFinish()
    {
        return _last >= _target;
    }

    private int _last;
    public virtual void OnChanged(int val)
    {
        if (useSpeed.value)
            OnChangedTween(val, 1);
        else
        {
            Trigger(val);
            var progressFloat = val * 1f / Target;
            foreach (var progressScript in progresses)
                progressScript.OnChanged(progressFloat);
        }
        foreach (var toggleScript in intProgresses)
            toggleScript.OnChanged(val);
    }
    
    private IDisposable _tween;
    [SerializeField] protected BoolFloat useSpeed = new BoolFloat(false,1); // 100 value is 1 seconds
    [SerializeField] protected float timeClamp = .5f;
    public void OnChangedTween(int val,float time)
    {
        var from = _last;
        _last = val;
        _tween?.Dispose();
        if (val > from)
        {
            if (useSpeed.value)
            {
                time = (val - from) / useSpeed.value2;
                if (time > timeClamp)
                    time = timeClamp;
            }
            _tween = Observable.FromCoroutine(() => DuongExtensions.IEDoValue(from, val, time, t =>
            {
                var d = (int) t;
                Trigger(d);
                var progressFloat = t / Target;
                foreach (var progressScript in progresses)
                    progressScript.OnChanged(progressFloat);
            })).Subscribe();
        }
        else
        {
            Trigger(val);
            var progressFloat = val * 1f / Target;
            foreach (var progressScript in progresses)
                progressScript.OnChanged(progressFloat);
        }
    }
    
    protected virtual void Trigger(int val)
    {
        _last = val;
    }
}