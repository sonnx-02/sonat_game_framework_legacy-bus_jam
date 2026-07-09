using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISubscribe
{
    void OnSubscribe();
}

public abstract class SubscribeScriptBase : BaseFrameworkView
{
    [SerializeField] private MonoBehaviour[] subscribeScrips;
    private List<ISubscribe> _list;
    [SerializeField]
    private SubscribeWhen subscribeWhen;
    protected abstract void RegisterSubscribe();

    private enum SubscribeWhen
    {
        Register,
        KernelLoaded,
        Enable
    }

    public void SubscribeTo(ISubscribe add)
    {
        CheckRegister();
        _list.Add(add);
    }

    public void Remove(ISubscribe rm)
    {
        CheckRegister();
        _list.Remove(rm);
    }

    protected override void Register()
    {
        base.Register();
        _list = Get().ToList();
        if(subscribeWhen == SubscribeWhen.Register)
            RegisterSubscribe();
    }

    protected virtual void OnEnable()
    {
        if(subscribeWhen == SubscribeWhen.Enable)
            RegisterSubscribe();
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(subscribeWhen == SubscribeWhen.KernelLoaded)
            RegisterSubscribe();
    }

    IEnumerable<ISubscribe> Get()
    {
        foreach (var monoBehaviour in subscribeScrips)
            if (monoBehaviour is ISubscribe)
                yield return monoBehaviour as ISubscribe;
    }

    private Action _registerAction;

    public void RegisterAction(Action act)
    {
        _registerAction += act;
    }
    protected virtual void Invoke()
    {
        CheckRegister();
        foreach (var item in _list)
            item.OnSubscribe();
        _registerAction?.Invoke();
    }
}