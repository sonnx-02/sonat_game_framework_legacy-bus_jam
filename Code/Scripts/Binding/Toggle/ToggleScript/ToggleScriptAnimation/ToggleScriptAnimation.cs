using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ToggleScriptAnimation : ToggleScript
{
    [MyButtonInt(nameof(CopyFrom), nameof(CopyTo))]
    [SerializeField] private  int abc123;

    public abstract void CopyFrom();
    public abstract void CopyTo();
    protected abstract IEnumerator ThisDoAnimate_In();
    protected abstract IEnumerator ThisDoAnimate_Out();

    private IDisposable _disposable;
    protected override void Trigger(bool val)
    {
        _disposable?.Dispose();
        if (val)
            _disposable = Observable.FromCoroutine(ThisDoAnimate_In).Subscribe();
        else
            _disposable = Observable.FromCoroutine(ThisDoAnimate_Out).Subscribe();
    }
}