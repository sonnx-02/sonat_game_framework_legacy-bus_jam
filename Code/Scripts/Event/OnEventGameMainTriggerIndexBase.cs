using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniRx;


public abstract class OnEventGameMainTriggerIndexBase : BaseOnEvent
{
    protected abstract int Value { get; }
    [SerializeField] private GameMainEvent[] onEvents;
    [SerializeField] private IndexBindingScriptBase[] bindings;
    [SerializeField] private bool onlyWhenActiveInHierarchy;
    protected override void Register()
    {
        base.Register();
    }

    protected override IDisposable RegisterEvent()
    {
        return SubjectController.OnGameMainEvent.Subscribe(OnSubcribe);
    }

    protected virtual void OnSubcribe(GameMainEvent data)
    {
        if (!onlyWhenActiveInHierarchy || gameObject.activeInHierarchy)
            foreach (var ev in onEvents)
            {
                if (data == ev)
                    bindings.OnChanged(Value);
                break;
            }
    }
}