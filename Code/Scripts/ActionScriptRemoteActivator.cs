using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptRemoteActivator : BaseBaseActionScript
{

    [SerializeField] private GameObject[] actives;
    [SerializeField] private GameObject[] deactives;

    [SerializeField] private ValidScriptMath math;
    
    public override void StartAction()
    {
        base.StartAction();
        var check = math.IsValid();
        actives.SetActiveAll(check);
        deactives.SetActiveAll(!check);
    }
}
