using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseOnEvent : BaseFrameworkView
{
    protected IDisposable Disposable;
    [SerializeField] protected bool listenOnlyWhenEnable;
    protected abstract IDisposable RegisterEvent();

    protected override void Register()
    {
        base.Register();
        if (!listenOnlyWhenEnable)
        {
            Disposable = RegisterEvent();
        }
    }

    protected void OnDisable()
    {
        if (listenOnlyWhenEnable)
        {
            Disposable?.Dispose();
        }
    }
    
    protected void OnEnable()
    {
        if (listenOnlyWhenEnable)
        {
            Disposable?.Dispose();
            Disposable = RegisterEvent();
        }
    }
}
