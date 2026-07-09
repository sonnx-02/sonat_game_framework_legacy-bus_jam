using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;



public  class IndexBindingScriptMode : IndexBindingScript
{
    [SerializeField] private int threshold = 1000000;
    [SerializeField] private IndexBindingScriptBase[] otherMod;
    [SerializeField] private IndexBindingScriptBase[] otherDivide;

    

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (computedValue >= threshold)
        {
            var value = computedValue % threshold ;
            otherDivide.OnChanged(value);
        }
        else
        {
            otherMod.OnChanged(computedValue);
        }
    }
}