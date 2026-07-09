using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptToggleAddPos : BaseBaseActionScript
{
    [SerializeField] private Vector3 add;
    public override void StartAction(bool value)
    {
        base.StartAction(value);
        transform.localPosition += value ? add : -add;
    }

    public override void StartAction(int index)
    {
        base.StartAction(index);
        StartAction(index > 0);
    }
}
