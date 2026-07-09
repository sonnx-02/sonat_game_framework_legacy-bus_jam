using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CustomDailyPropertyEnabler : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.CustomDailyDataProperty)] [SerializeField]
    protected int key;

    [SerializeField] private bool activeThis = true;
    [SerializeField] private ToggleScript[] onActives;
    [SerializeField] private bool subscribe = true;

    protected IntReactiveProperty Property => key.GetDailyDataCustomProperty();
    // Start is called before the first frame update
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (subscribe)
            Property.Subscribe(data => OnChange());
        else
            OnChange();
    }

    protected void OnChange()
    {
        bool active = !Property.BoolValue;
        if (activeThis)
            gameObject.SetActive(active);
        onActives.OnChanged(active);
    }
}