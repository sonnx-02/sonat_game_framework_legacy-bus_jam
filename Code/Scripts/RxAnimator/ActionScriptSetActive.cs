using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetActive : BaseActionScript
{
    [SerializeField] private bool active;
    [SerializeField] private GameObject[] targets;
    [SerializeField] private Activator activator;
    protected override void DoAction()
    {
        base.DoAction();
        targets.SetActiveAll(active);
        activator.OnActivate();
    }
}
