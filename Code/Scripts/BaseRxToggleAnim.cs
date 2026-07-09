using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class BaseRxToggleAnim : ToggleScript
{
    protected bool IsTweening { get; set; }
    protected IDisposable Disposable;
    [SerializeField] private ProgressScript[] progressScripts;
    protected override void Trigger(bool val)
    {
        if (!IsTweening)
        {
            IsTweening = true;
            Observable.FromCoroutine(() => ToggleAnim(val))
                .Subscribe();
        }
    }

    public virtual void ResetAnim()
    {
        IsTweening = false;
        Disposable?.Dispose();
        UpdateValue(false, 0);
    }

    protected virtual void UpdateValue(bool reverse, float progress)
    {
        foreach (var progressScript in progressScripts)
        {
            progressScript.TriggerToggle(progress,reverse);
        }
    }

    protected abstract IEnumerator ToggleAnim(bool reverse);
}
