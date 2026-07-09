using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BooleanValueRetrieverBase : BaseFrameworkView
{
    [SerializeField] private StringBool remoteLevelValue;
    [SerializeField] private int defaultLevelValid = -1;
    [SerializeField] private BooleanValueRetrieverBase[] others;
    protected abstract bool Value { get; set; }

    public int LevelCompareValue
    {
        get
        {
            if (remoteLevelValue.value2)
                return (int)RemoteConfigHelper.GetValueInt(remoteLevelValue.value,defaultLevelValid);
            return defaultLevelValid;
        }
    }
    public bool Valid => Value && ValueOthers && PlayerData.CurrentLevel >= LevelCompareValue;

    private bool ValueOthers
    {
        get
        {
            foreach (var booleanValueRetrieverBase in others)
                if (!booleanValueRetrieverBase.Value)
                    return false;
            return true;
        }
    }
}


public abstract class BooleanValueRetrieverMathCompare : BooleanValueRetrieverBase
{
    protected abstract int CompareValue1 { get; }
    protected abstract int CompareValue2 { get; }
    [SerializeField] private float percent100 = 100f;
    [SerializeField] private MathEquality mathEquality; // x % 2 == 0 , == is math compare

    protected override bool Value
    {
        get
        {
            switch (mathEquality)
            {
                case MathEquality.Equal:
                    return CompareValue1 == CompareValue2;
                case MathEquality.Diff:
                    return CompareValue1 != CompareValue2;
                case MathEquality.Smaller:
                    return CompareValue1 < CompareValue2;
                case MathEquality.SmallerOrEqual:
                    return CompareValue1 <= CompareValue2;
                case MathEquality.Greater:
                    return CompareValue1 > CompareValue2;
                case MathEquality.GreaterOrEqual:
                    return CompareValue1 >= CompareValue2;
                case MathEquality.Percent:
                    return ((int) (CompareValue1 * 1f / CompareValue2 * 100)) >= percent100;
                case MathEquality.HasBit:
                    return (CompareValue1 & CompareValue2) != 0;
                    break;
                default:
                    return false;
            }
        }
        set
        {
            
        }
    }
}