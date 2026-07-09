using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingSetIndex : IndexBindingScript
{
    [SerializeField] private BaseRxAnimator[] animators;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        foreach (var baseRxAnimator in animators)
            baseRxAnimator.SetIndex(computedValue);
    }
}
