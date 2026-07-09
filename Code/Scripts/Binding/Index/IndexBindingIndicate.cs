using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MathOperator
{
    None,
    Add,
    Multiple,
    Divide,
    Mode,
    Ceil,
    Floor
}

public enum MathEquality
{
    Equal,
    Diff,
    Smaller,
    SmallerOrEqual,
    Greater,
    GreaterOrEqual,
    Percent,
    HasBit
}

public class IndexBindingIndicate : IndexBindingScript
{
    public MathOperator math = MathOperator.None;
    public int mathValue;
    public int compareValue;

    public Image target;
    public Sprite onEqual;
    public Sprite onSmaller;
    public Sprite onGreater;


    protected override void LoadComponent()
    {
        target = GetComponent<Image>();
    }

    protected override bool IsNoComponent()
    {
        return target == null;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);

        var computeValue = GetValue(computedValue);

        if (computeValue == compareValue)
            target.sprite = onEqual;
        else   if (computeValue < compareValue)
            target.sprite = onSmaller;
        else   if (computeValue > compareValue)
            target.sprite = onGreater;
    }

    private int GetValue(int input)
    {
        switch (math)
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
                return -1;
            case MathOperator.Floor:
                return -1;
            case MathOperator.None:
                return -1;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

