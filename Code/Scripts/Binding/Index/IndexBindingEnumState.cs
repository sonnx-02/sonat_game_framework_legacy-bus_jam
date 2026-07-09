using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class IndexBindingEnumState : IndexBindingScript
{
    protected abstract int EnumState { get; }
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private  bool setActive = true;
    [SerializeField] private  bool reverse;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);

        var valid = computedValue == EnumState;
        if (reverse)
            valid = !valid;
        if(setActive)
            gameObject.SetActive(valid);
        toggles.OnChanged(valid);
    }
}