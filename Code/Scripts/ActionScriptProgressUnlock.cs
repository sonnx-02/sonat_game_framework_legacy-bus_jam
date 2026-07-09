using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


public class ActionScriptProgressUnlock : BaseActionScriptInput
{
    [SerializeField] private BaseBaseActionScript[] actionIfFinishOnStart;
    [FormerlySerializedAs("actionIfFinish")] 
    [SerializeField] private BaseBaseActionScript[] actionIfFinishOnComplete;
    [SerializeField] private BaseBaseActionScript[] actionToggleIfFinish;
    [SerializeField] private BaseBaseActionScript[] actionBeforeTween;
    [SerializeField] private BaseBaseActionScript[] actionAfterTween;
    [SerializeField] private TweenDisplayScript tweenDisplayScript;
    [SerializeField] private ToggleScript[] ifValid;
    [SerializeField] private float delayTween;
    //start, end, from, to
    [SerializeField] private Int4 currentData;

    public override void SetArrayInt(int[] arr)
    {
        base.SetArrayInt(arr);
        currentData = new Int4(arr);
        Set();
    }

    protected override void DoAction()
    {
        base.DoAction();
        bool valid = currentData.IsProgressValid();
        if (valid)
        {
            Set();
            actionToggleIfFinish.StartAction(currentData.To >= currentData.End);
            if (currentData.End >= currentData.To)
                actionIfFinishOnStart.StartAction();
            actionBeforeTween.StartAction();
            if (delayTween > 0)
                Observable.Timer(TimeSpan.FromSeconds(delayTween), Scheduler.MainThreadIgnoreTimeScale)
                    .Subscribe(data =>
                    {
                        tweenDisplayScript.TweenTo(currentData.To - currentData.Start, OnCompleteTween);
                    });
            else
                tweenDisplayScript.TweenTo(currentData.To - currentData.Start, OnCompleteTween);
        }

        ifValid.OnChanged(valid);
    }
    
    private void Set()
    {
#if UNITY_EDITOR
        return;
        if(disableAction)
            return;
        if (currentData.End - currentData.From == 0)
        {
            Debug.LogError("divide by zero");
            Debug.Break();
        }
#endif
        tweenDisplayScript.SetTarget(currentData.End - currentData.Start);
        tweenDisplayScript.SetTo(currentData.From - currentData.Start); 
    }

    private void OnCompleteTween()
    {
        if (currentData.To >= currentData.End)
            foreach (var baseActionScript in actionIfFinishOnComplete)
                baseActionScript.StartAction(0);
        foreach (var baseActionScript in actionAfterTween)
            baseActionScript.StartAction(0);
    }
}
