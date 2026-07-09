using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseChecker : BaseBaseActionScript
{
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private bool setActiveThis;
    [SerializeField] private bool reverse;
    public virtual void Check()
    {
        var getCheck = GetCheck();
        if (reverse)
            getCheck = !getCheck;
        toggles.OnChanged(getCheck);
        if(setActiveThis)
            gameObject.SetActive(getCheck);
        OnCheck(getCheck);
    }

    protected override void DoAction()
    {
        base.DoAction();
        Check();
    }

    protected virtual void OnCheck(bool computeCheck)
    {
        
    }

    protected abstract bool GetCheck();

}
