using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptPos : ToggleScript
{
    [SerializeField] private Vector3 onAnchorPos;
    [SerializeField] private Vector3 offAnchorPos;

    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        transform.localPosition = val ? onAnchorPos : offAnchorPos;
    }

    protected override void LoadOn()
    {
        base.LoadOn();
        onAnchorPos = transform.localPosition;
    }

    protected override void LoadOff()
    {
        base.LoadOff();
        offAnchorPos = transform.localPosition;
    }
}