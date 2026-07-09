using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InvokeRegisteredActionButton : BaseButton
{
    [IndexAsEnum(BuiltInEnumType.RegisterAction)]
    [SerializeField] private int action;
    
    protected override void Register()
    {
        base.Register();
    }

    protected override void OnClick()
    {
        base.OnClick();
        SubjectController.InvokeRegisteredAction(action);
    }

    protected override string GetName()
    {
        return base.GetName();
    }
}
