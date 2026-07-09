using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingScriptRevokeAnim : IndexBindingScript
{
    [SerializeField] private BaseRxAnimator[] animators;
    [SerializeField] private int compareValue;
    [SerializeField] private Condition condition;
    [SerializeField] private float delay;
    public enum Condition
    {
        No,
        ChangeToDifferentFromValue, // mean if it switch from 0(compareValue) to another value => revoke
    }
    private int _last = -1;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        bool valid = condition == Condition.No;
        if (condition == Condition.ChangeToDifferentFromValue)
            if (_last == compareValue && compareValue != computedValue)
                valid = true;
        if (valid)
            delay.Timer(() =>
            {
                foreach (var baseRxAnimator in animators)
                {
                    if (baseRxAnimator.gameObject.activeInHierarchy)
                        baseRxAnimator.Animator_In();
                } 
            });
        _last = computedValue;
    }
}
