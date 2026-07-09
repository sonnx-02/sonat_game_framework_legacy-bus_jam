using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingSizeDelta : IndexBindingScript
{
    [SerializeField] private Vector2[] sizes;

    

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        sizes[value] = (transform as RectTransform).sizeDelta;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        (transform as RectTransform).sizeDelta = sizes[computedValue];
    }
}
