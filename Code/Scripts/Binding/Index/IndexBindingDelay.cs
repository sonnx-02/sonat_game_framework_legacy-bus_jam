using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingDelay : IndexBindingScript
{
    [SerializeField] private float delay;
    [SerializeField] private IndexBindingScript otherScripts;
    [SerializeField] private bool onlyWhenIncrease;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if(gameObject.activeInHierarchy && (!onlyWhenIncrease || (onlyWhenIncrease && IsIncreased)))
            delay.Timer(() =>
            {
                otherScripts.OnChanged(computedValue);
            });
        else
            otherScripts.OnChanged(computedValue);
    }
}
