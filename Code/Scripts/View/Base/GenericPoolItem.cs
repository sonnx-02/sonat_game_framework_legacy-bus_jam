using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPoolItem : PoolItem
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.PoolItemEnum)]
    private int index;

    [SerializeField] private BaseRxStepAnimator stepAnimator;
    public override int Index => index;

    [SerializeField] private Vector2Int animatorStep;
    [SerializeField] private IndexBindingScriptBase[] bindings;
    [SerializeField] private ListActionScript[] listActions;
    [SerializeField] private ToggleScript[] toggles;

    public void StartStepAnimation()
    {
        stepAnimator.StartAnim(animatorStep.x, animatorStep.y);
    }

    public void StartStepAnimation(int from, int to)
    {
        animatorStep = new Vector2Int(from, to);
        StartStepAnimation();
    }

    public override void SetParameter(PointParameter param)
    {
        base.SetParameter(param);
        bindings.OnChanged(param.value);
    }

    public void OnChange(int change)
    {
        bindings.OnChanged(change);
    }
    
    public void OnChange2(int change)
    {
        foreach (var indexBindingScriptBase in bindings)
            indexBindingScriptBase.OnChanged2(change);
    }


    public void StartActions(int key)
    {
        listActions[key].scripts.StartAction();
    }

    public void StartActions(int key, int i)
    {
        listActions[key].scripts.StartAction(i);
    }

    public void SetToggle(bool toggle)
    {
        toggles.OnChanged(toggle);
    }
}