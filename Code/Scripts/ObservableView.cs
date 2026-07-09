using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ObservableView : BaseActionScript
{
    [SerializeField] protected BaseRxAnimator[] anims;
    [SerializeField] protected IndexBindingScriptBase[] paramBinds; // 0
    public abstract IEnumerator Ie(int[] parameter);

    

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        Observable(new[] {value}).Subscribe();
    }

    public virtual void ResetView()
    {
        gameObject.SetActive(false);
    }

    public virtual IObservable<Unit> Observable(int[] value)
    {
        gameObject.SetActive(true);
        AnimIn();
        if(value != null)
            paramBinds.OnChanged(value[0]);
        return UniRx.Observable.FromCoroutine(() => Ie(value));
    }

    public void AnimIn()
    {
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_In();
    }
    
    public void AnimOut()
    {
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_In();
    }
}
