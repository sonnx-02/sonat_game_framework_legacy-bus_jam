using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptProgressScript : ToggleScript
{
    [SerializeField] private ProgressScript[] progressScripts;
    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        foreach (var progressScript in progressScripts)
            progressScript.OnChanged(val ? 1 : 0);
    }
}
