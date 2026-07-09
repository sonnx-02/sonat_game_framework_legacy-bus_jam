using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class RxTweenScript_MultipleIndicator : RxTweenScript
{
    [SerializeField] protected MultipleArrow[] data;

    protected virtual float CompareValue => lastT;
    public virtual float GetMultiple(out int index)
    {
        index = 0;
        for (var i = 0; i < data.Length; i++)
            if (CompareValue >= data[i].@from && CompareValue <= data[i].to)
            {
                index = i;
                return data[i].multiple;
            }

        return 0;
    }
    
    public virtual float GetMultiple()
    {
        for (var i = 0; i < data.Length; i++)
            if (CompareValue >= data[i].@from && CompareValue <= data[i].to)
                return data[i].multiple;

        return 0;
    }
}