using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingConditionStartAction : IndexBindingToToggle
{
    [SerializeField] private BaseBaseActionScript[] onTrue;
    [SerializeField] private BaseBaseActionScript[] onFalse;
    [SerializeField] private BoolBool once;
    protected override void OnValueChanged(bool val)
    {
        base.OnValueChanged(val);
        if (_triggered && once.value)
            return;
        if (val == once.value2)
            _triggered = true;
        (val ? onTrue : onFalse).StartAction();
    }

    private bool _triggered;
    private void OnDisable()
    {
        _triggered = false;
    }
}
