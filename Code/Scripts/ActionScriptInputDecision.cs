using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MathCompareDataAction 
{
    public BaseBaseActionScript[] actions;

    public MathEquality mathEquality; // x % 2 == 0 , == is math compare
    public IntValueRetrieverOrDefault retriever; // if has retriever use retriever not value

    public bool GetResult(int computeValue)
    {
        int compareValue = retriever.Value;
        switch (mathEquality)
        {
            case MathEquality.Equal:
                return computeValue == compareValue;
            case MathEquality.Diff:
                return computeValue != compareValue;
            case MathEquality.Smaller:
                return computeValue < compareValue;
            case MathEquality.SmallerOrEqual:
                return computeValue <= compareValue;
            case MathEquality.Greater:
                return computeValue > compareValue;
            case MathEquality.GreaterOrEqual:
                return computeValue >= compareValue;
            case MathEquality.Percent:
                return ((int) (computeValue * 1f / compareValue * 100f)) >= compareValue;
            case MathEquality.HasBit:
                return (computeValue & compareValue) != 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool CheckStartAction(int compareValue)
    {
        if(GetResult(compareValue))
        {
            actions.StartAction();
            return true;
        }

        return false;
    }
}

public class ActionScriptInputDecision : BaseActionScriptInput
{
    [SerializeField] private Button button;
//    public BaseBaseActionScript[] defaultActions;
    protected override void Register()
    {
        base.Register();
        if (button != null)
            button.onClick.AddListener(DoAction);
    }

    [SerializeField] private MathCompareDataAction[] actions;
    [SerializeField] private ForRunType runType = ForRunType.ForEachAll;

   
    
    protected override void DoAction()
    {
        base.DoAction();
        foreach (var mathCompareDataAction in actions)
        {
            var check = mathCompareDataAction.CheckStartAction(input.Value);
            if (runType == ForRunType.StopWhenFalse && !check)
                break;
            if (runType == ForRunType.StopWhenTrue && check)
                break;
        }
    }

    public override void StartAction(int index)
    {
        base.StartAction(index);
        foreach (var mathCompareDataAction in actions)
        {
            var check = mathCompareDataAction.CheckStartAction(index);
            if (runType == ForRunType.StopWhenFalse && !check)
                break;
            if (runType == ForRunType.StopWhenTrue && check)
                break;
        }
    }
}
