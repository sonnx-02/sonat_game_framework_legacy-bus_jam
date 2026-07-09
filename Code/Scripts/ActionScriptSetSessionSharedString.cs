using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetSessionSharedString : BaseBaseActionScriptStartOnEnable
{
    [IndexAsEnum(BuiltInEnumType.SessionSharedString)] [SerializeField]
    private int key;

    [SerializeField] private string value;

    [SerializeField] private bool onDisable;
    [SerializeField] private string value2;

    protected override void DoAction()
    {
        base.DoAction();
        SessionData.sharedString[key] = value;
    }

    private void OnDisable()
    {
        SessionData.sharedString[key] = value2;
    }
}