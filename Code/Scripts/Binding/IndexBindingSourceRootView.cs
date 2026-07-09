using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[System.Obsolete("this is obsolete use IndexPropertySourceRetriever")]
public class IndexBindingSourceRootView : BindingSourceRootView
{
    [SerializeField] private IndexBindingScriptBase[] scripts;
    [SerializeField] private IndexPropertyChangeScriptBase[] changeScripts;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        var p = GetProperty();
        if(scripts.Length > 0)
            p.Subscribe(scripts.OnChanged).DisposeWith(this);
        foreach (var sc in changeScripts)
            sc.SetProperty(p);
    }

    IntReactiveProperty GetProperty()
    {
        var p = (IntReactiveProperty) GetPropertyByName();
        if (p == null)
            Debug.LogError("not found" + propertyName, gameObject);

        return p;
    }
}