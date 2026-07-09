using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingUpdatePos : IndexBindingScript
{
    [SerializeField] private bool x;
    [SerializeField] private bool y;
    [SerializeField] private bool z;
    [SerializeField] private Vector3 multi;
    [SerializeField] private bool local = true;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var transform1 = transform;
        var pos = local ? transform1.localPosition : transform1.position;
        if (x)
            pos.x = multi.x * computedValue;
        if (y)
            pos.y = multi.y * computedValue;
        if (z)
            pos.z = multi.z * computedValue;

        if (local)
            transform1.localPosition = pos;
        else
            transform1.position = pos;
    }
}
