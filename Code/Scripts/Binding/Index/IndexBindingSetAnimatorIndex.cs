using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingSetAnimatorIndex : IndexBindingScript
{

    [SerializeField] private BaseRxAnimator[] anims;

    

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.SetIndex(computedValue);
    }
}
