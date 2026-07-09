using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script to turn on or off rx animators
/// </summary>
public class IndexBindingTurnOffAnimator : IndexBindingScript
{
    [SerializeField] private TurnOnAnimationCondition[] conditions;
    
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        foreach (var condition in conditions)
        {
            bool valid =  condition.condition > computedValue;
            foreach (var baseRxAnimator in condition.animators)
                baseRxAnimator.SetTurnOff(valid);
        }
    }

    protected override void LoadComponent()
    {
    }

    [Serializable]
    public struct TurnOnAnimationCondition
    {
        public int condition;// larger
        public BaseRxAnimator[] animators;
    }
}
