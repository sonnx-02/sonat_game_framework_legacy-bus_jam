using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable Unity.InefficientPropertyAccess
public abstract class IndexBindingScriptBase : BaseBindingAction
{
    public abstract void OnChanged(int value);

    
    protected override void OnShow(bool overwritten)
    {
    }

    public virtual void  OnChanged2(int  index)
    {
    }

    public virtual void OnChanged2(float value)
    {
    }
}