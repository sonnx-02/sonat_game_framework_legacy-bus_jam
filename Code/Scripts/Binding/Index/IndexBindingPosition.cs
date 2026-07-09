using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingPosition : IndexBindingScript
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3[] positions;
    [SerializeField] private Vector3[] scales;
    [SerializeField] private Vector3 positionDefault;
    [SerializeField] private BaseBaseActionScript[] actionOnChanged;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        actionOnChanged.StartAction();
        if (computedValue >= 0 && computedValue < positions.Length)
            target.localPosition = positions[computedValue];
        else
            target.localPosition = positionDefault;
        if (computedValue >= 0  && scales.Length > 0 && computedValue < scales.Length)
            target.localScale = scales[computedValue];
    }

  

    protected override void LoadComponent()
    {
        positionDefault = target.localPosition;
    }
}
