using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptCheckSetActive : BaseActionScriptBoolean
{
    [SerializeField] private GameObject[] actives;
    [SerializeField] private GameObject[] activesReverse;
    [SerializeField] private ToggleScript[] toggles;

    public override void StartAction()
    {
        base.StartAction();
        var valid = Value;
        actives.SetActiveAll(valid);
        activesReverse.SetActiveAll(!valid);
        toggles.OnChanged(valid);
    }
}
