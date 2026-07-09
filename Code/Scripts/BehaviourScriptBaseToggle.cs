using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourScriptBaseToggle : BaseFrameworkView
{
    public virtual void StartAction(bool isOn)
    {
        Observable.FromCoroutine(() => IeDoingAction(isOn)).TakeUntilDisable(this).Subscribe();
    }

    protected abstract IEnumerator IeDoingAction(bool isOn);
}