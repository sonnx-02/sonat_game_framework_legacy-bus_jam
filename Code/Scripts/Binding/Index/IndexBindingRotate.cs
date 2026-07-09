using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingRotate : IndexBindingScript
{
    [SerializeField] private Transform rotateTransform;
    [SerializeField] private Vector3 rotateVec = new Vector3(0,90,0);
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        (rotateTransform == null ? transform : rotateTransform).localEulerAngles = rotateVec * computedValue;
    }
}
