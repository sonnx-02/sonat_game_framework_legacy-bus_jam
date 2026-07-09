using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressScriptBezier : ProgressScript
{
    [SerializeField] private Transform from;
    [SerializeField] private Transform to;
    [SerializeField] private Transform bezier;
    public override void Init()
    {
        base.Init();
    }

    public override void OnChanged(float val)
    {
        base.OnChanged(val);
        transform.position = DuongExtensions.Bezier2(@from.position, bezier.position, to.position, val);
    }
}
