using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptStopInputFor : BaseBaseActionScript
{
    [SerializeField] private float delay = 1;
    protected override void DoAction()
    {
        base.DoAction();
        ScreenRoot.eventSystem.enabled = false;
        delay.TimerUnscale(() => ScreenRoot.eventSystem.enabled = true);
    }
}
