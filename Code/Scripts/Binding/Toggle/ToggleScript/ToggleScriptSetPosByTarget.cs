using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptSetPosByTarget : ToggleScript
{
    [SerializeField] private Transform toSet;
    [SerializeField] private Transform targetOn;
    [SerializeField] private Transform targetOff;
    protected override void LoadComponent()
    {
    }

    
    protected override void Trigger(bool val)
    {
        toSet.position = val ?targetOn.position : targetOff.position;
    }
}
