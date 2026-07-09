using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConditionStart
{
    [SerializeField] private BaseBaseActionScript[] actions;
    [SerializeField] protected BooleanValueRetrieverBase input;
    [SerializeField] protected bool defaultValue;

    protected virtual bool Value
    {
        get
        {
            if (input == null)
                return defaultValue;
            return input.Valid;
        }
    }
    
    public bool Trigger()
    {
        if (Value)
        {
            actions.StartAction();
            return true;
        }

        return false;
    }
}
public class ActionScriptStartByCondition : BaseBaseActionScript
{
    [SerializeField] private ConditionStart[] conditions;
    [SerializeField] private ForRunType runType = ForRunType.ForEachAll;

    protected override void DoAction()
    {
        base.DoAction();
        foreach (var mathCompareDataAction in conditions)
        {
            var check = mathCompareDataAction.Trigger();
            if (runType == ForRunType.StopWhenFalse && !check)
                break;
            if (runType == ForRunType.StopWhenTrue && check)
                break;
        }
    }
}
