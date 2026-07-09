using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionScriptPlayProgress : BaseActionScript
{
    [SerializeField] private float time = 1;
    [SerializeField] private Vector2 fromTo = new Vector2(0, 1);
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private ProgressScript[] progressScripts;

    protected override void DoAction()
    {
        base.DoAction();
        Observable.FromMicroCoroutine(Do).TakeUntilDisable(this)
            .Subscribe();
    }

    private IEnumerator Do()
    {
        float t = 0f;
        Change(fromTo.x);
        while (t < time)
        {
            t += Time.deltaTime;
            if(useCurve)
                Change(curve.Evaluate(t/time));
            else
                Change(t/time);
            yield return null;
        }
        Change(fromTo.y);
    }

    private void Change(float val)
    {
        foreach (var progressScript in progressScripts)
        {
            progressScript.OnChanged(val);
        }
    }

    
}