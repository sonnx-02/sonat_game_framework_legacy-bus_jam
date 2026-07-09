using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourScriptBase : BaseFrameworkView
{
    public virtual void StartAction()
    {
        Observable.FromCoroutine(IeDoingAction).TakeUntilDisable(this).Subscribe();
    }

    protected abstract IEnumerator IeDoingAction();
}