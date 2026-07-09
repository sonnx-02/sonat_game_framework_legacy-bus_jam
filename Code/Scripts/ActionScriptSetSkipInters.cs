using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetSkipInters : BaseBaseActionScript
{
    [SerializeField] private bool defaultVal = true;
    [SerializeField] private bool reverse;
    protected override void DoAction()
    {
        base.DoAction();
        SessionData.skipIntersOnce = defaultVal;
    }

    public override void StartAction(bool value)
    {
        SessionData.skipIntersOnce = reverse ? !value : value;
    }
}
