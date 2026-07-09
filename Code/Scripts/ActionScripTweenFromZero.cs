using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScripTweenFromZero : BaseActionScriptInput
{
    [SerializeField] private BaseBaseActionScript[] actionAfterTween;
    [SerializeField] private TweenDisplayScript tweenDisplayScript;
    [SerializeField] private ToggleScript[] ifValid;

    //start, end, from, to
    [SerializeField]
    private int tweenTo;

    public override void SetArrayInt(int[] arr)
    {
        base.SetArrayInt(arr);
        tweenTo = arr[0];
        Set();
    }
    

    private void Set()
    {
        #if UNITY_EDITOR
        if (tweenTo == 0)
        {
 //           Debug.LogError("divide by zero");
//            Debug.Break();
        }
        #endif
        tweenDisplayScript.SetTarget(tweenTo == 0 ? 1: tweenTo);
        tweenDisplayScript.SetTo(0); 
    }

    protected override void DoAction()
    {
        base.DoAction();
        bool valid = tweenTo > 0;
        if (valid)
        {
            Set();
            tweenDisplayScript.TweenTo(tweenTo, OnCompleteTween);
        }
        ifValid.OnChanged(valid);
    }

    private void OnCompleteTween()
    {
        foreach (var baseActionScript in actionAfterTween)
            baseActionScript.StartAction(0);
    }

   
}
