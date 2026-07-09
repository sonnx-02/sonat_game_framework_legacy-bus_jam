using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptCheckStartAction : BaseActionScriptBoolean
{

    [SerializeField] private BaseBaseActionScript[] actions;

    protected override void DoAction()
    {
        base.DoAction();
        if(Value)
            actions.StartAction();
    }
}
