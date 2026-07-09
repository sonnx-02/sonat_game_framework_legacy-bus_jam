using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IndexScriptTrigger
{
    public IndexBindingScriptBase script;
    public int value;
}

public class ToggleScriptTriggerIndexScript : ToggleScript
{
    [SerializeField] private bool triggerTrueOnly;
    [SerializeField] private IndexScriptTrigger[] onTriggers;
    [SerializeField] private IndexScriptTrigger[] offTriggers;
    
    protected override void LoadComponent()
    {
        
    }

    protected override void Trigger(bool val)
    {
        if (val)
        {
            foreach (var indexScriptTrigger in onTriggers)
                indexScriptTrigger.script.OnChanged(indexScriptTrigger.value);
        }
        else if (!triggerTrueOnly)
        {
            foreach (var indexScriptTrigger in offTriggers)
                indexScriptTrigger.script.OnChanged(indexScriptTrigger.value);
        }
    }
}
