using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValidScriptBase : BaseFrameworkView,ISubscribe
{
    [SerializeField] private ToggleScript[] togglesOnEnable;
    [SerializeField] private GameObject[] whenOn;
    [SerializeField] private GameObject[] whenOff;
    [SerializeField] private SubscribeScriptBase updateOn;
    [SerializeField] private ValidScriptBase[] otherOr;
    [SerializeField] private ValidScriptBase[] otherAnd;
    protected abstract bool IsThisValid();

    public bool IsValid()
    {
        bool valid = IsThisValid();
        if(!valid)
            foreach (var validScriptBase in otherOr)
                if (validScriptBase.IsValid())
                {
                    valid = true;
                    break;
                }

        if (!valid)
            return false;
        foreach (var validScriptBase in otherAnd)
            if (!validScriptBase.IsValid())
            {
                return false;
            }

        return true;
    }
   

    protected override void Register()
    {
        base.Register();
        if (updateOn != null)
            updateOn.RegisterAction(OnSubscribe);
    }

    protected virtual void OnEnable()
    {
        OnSubscribe();
    }

    public void OnSubscribe()
    {
        bool valid = IsValid();
        togglesOnEnable.OnChanged(valid);
        whenOn.SetActiveAll(valid);
        whenOff.SetActiveAll(!valid);
    }
}
