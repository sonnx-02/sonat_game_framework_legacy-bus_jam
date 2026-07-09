using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class DirectionHitScriptBase : BaseFrameworkView
{
    [MyButtonRange(0, 3, nameof(Hit), new string[] {"top", "right", "bottom", "left"})] [SerializeField] private int copy2;

    private IDisposable _disposable;
    protected bool Busy;

    public virtual void Hit(int direction)
    {
        if (!Busy)
        {
            _disposable?.Dispose();
            _disposable = null;
            _disposable = Observable.FromCoroutine(() => OnHit(direction)).Subscribe();
        }
    }

    protected abstract IEnumerator OnHit(int direction);
}
