using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
public class MathCompareData
{
    public MathEquality mathEquality; // x % 2 == 0 , == is math compare
#if UNITY_EDITOR
    [LabelHeader("Re-select to reload ENUM", 0, 1, 1)] 
    [SerializeField]
    private int testHeader;
    [SerializeField] [MyButton(nameof(LogCompareValue))]
    private int testButton;
#endif
    [ConditionalField(nameof(retriever),new object[]{null})]
    public BuiltInEnumType builtInEnumType;
    
    [IndexByBuiltInEnum(nameof(builtInEnumType))] // for display
    public int value; // x % 2 == 0, 0 is compare value

    // compareByEnumValue is use value of enum as int to compare, or compare by value of that in current game setting or somewher elese 
    [ConditionalField(nameof(builtInEnumType), 
        new object[] {BuiltInEnumType.UnlockConditionEnum})]
    public bool compareByEnumValue = true;
    public IntValueRetrieverBase retriever; // if has retriever use retriever not value
    [ConditionalField(nameof(mathEquality),new object[]{MathEquality.Percent})]
    public int percent100;


    private void LogCompareValue()
    {
        Debug.Log(GetCompareValue());
    }
    
    public int GetCompareValue()
    {
        if (retriever != null)
            return retriever[value];
        if (builtInEnumType == BuiltInEnumType.UnlockConditionEnum && !compareByEnumValue)
            return RootView.rootView.gameController.BaseCurrentGameSetting.levelUnlocks[value];
        return value;
    }
    
    
    public bool GetResult(int computeValue)
    {
        int compareValue = GetCompareValue();
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
                return ((int) (computeValue * 1f / compareValue * 100f)) >= percent100;
            case MathEquality.HasBit:
                return (computeValue & compareValue) != 0;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public abstract class IndexBindingMathCompareBase : IndexBindingScript
{
    [FormerlySerializedAs("math")] [SerializeField]
    private MathOperator mathOperator = MathOperator.None; // math operator % / mod...
    [SerializeField] private int mathValue; // ex x % 2, => 2 is math value
    [SerializeField] private IndexBindingScriptBase[] compareValueBindings;
#if UNITY_EDITOR
    [LabelHeaderAttribute("CompareHere!:", 1, 1, 0)]
    public string test;
#endif
    [SerializeField] private MathCompareData defaultCompare;
    [SerializeField] private MathCompareData[] otherAndCompare;
    [SerializeField] private MathCompareData[] otherOrCompare;
    [SerializeField] private int[] excepts; //  x % 2 == 0, if x in excepts, return false
    [SerializeField] private bool reverse; // toggle set active this gameobject

    [SerializeField] private IndexBindingScriptBase[] othersWhenOn;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        compareValueBindings.OnChanged(defaultCompare.GetCompareValue());
    }

    protected abstract void OnValueChanged(bool toggle);

 
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var val = GetToggle(computedValue);
        if (val)
            othersWhenOn.OnChanged(computedValue);
        OnValueChanged(val);
    }

    protected override void PlaySoundOnChanged()
    {
    }

    private bool GetToggle(int computedValue)
    {
        foreach (var except in excepts)
            if (computedValue == except)
                return false;

        var operatedValue = GetOperatedValue(computedValue);
        bool result = defaultCompare.GetResult(operatedValue);
        if (otherAndCompare.Any(mathCompareData => !mathCompareData.GetResult(operatedValue)))
            result = false;
        if (otherOrCompare.Any(mathCompareData => mathCompareData.GetResult(computedValue)))
            result = true;
        if (reverse)
            return !result;
        return result;
    }


    private int GetOperatedValue(int input)
    {
        switch (mathOperator)
        {
            case MathOperator.Add:
                return input + mathValue;
            case MathOperator.Multiple:
                return input * mathValue;
            case MathOperator.Divide:
                return input / mathValue;
            case MathOperator.Mode:
                return input % mathValue;
            case MathOperator.Ceil:
                return Mathf.CeilToInt(input * 1f / mathValue);
            case MathOperator.Floor:
                return Mathf.FloorToInt(input * 1f / mathValue);
            case MathOperator.None:
                return input;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetCompareValue(int val)
    {
        defaultCompare.value = val;
    }
}