using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingDistribute : IndexBindingScript
{
    [SerializeField] private Transform[] targets;
    [SerializeField] private ListVector3[] settings;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var setting = settings[computedValue];
        for (var i = 0; i < setting.vecs.Length; i++)
        {
            targets[i].localPosition = setting.vecs[i];
        }
    }

    protected override void LoadComponent()
    {
    }
}
