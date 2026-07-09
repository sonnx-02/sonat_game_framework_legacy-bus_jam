using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HpScriptBase : BaseFrameworkView
{
    protected int StartHp;
    protected int Current;

    [SerializeField] private IndexBindingScriptBase[] valueBindings;
    [SerializeField] private IndexBindingScriptBase[] losingBindings;

    public virtual void Clear()
    {
        
    }
    
    public virtual void SetStartValue(int startHp)
    {
        Clear();
        StartHp = startHp;
        OnChanged(StartHp);
    }

    public virtual void OnChanged(int obj)
    {
        Current = obj;
        valueBindings.OnChanged(Current);
        losingBindings.OnChanged(StartHp - Current);
    }
}
