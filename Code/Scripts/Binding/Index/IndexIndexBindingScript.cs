using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable Unity.InefficientPropertyAccess
public abstract class IndexIndexBindingScriptBase : BaseBindingAction
{
    public abstract void OnChanged(int value,int value2);

    
    protected override void OnShow(bool overwritten)
    {
    }

    protected abstract void LoadComponent();

    protected virtual void Reset()
    {
        LoadComponent();
    }

}