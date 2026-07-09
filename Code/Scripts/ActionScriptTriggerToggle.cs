using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptTriggerToggle : BaseBaseActionScript
{
    [SerializeField] private ToggleScript[] toggleScripts;
    public override void StartAction(bool value)
    {
        base.StartAction(value);
        toggleScripts.OnChanged(value);
    }

    public override void LogCustom()
    {
        base.LogCustom();
        toggleScripts = GetComponents<ToggleScript>();
    }
}
