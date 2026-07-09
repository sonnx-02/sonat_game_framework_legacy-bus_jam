using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class BaseIndexBindingObservable : IndexBindingScript
{
    protected IDisposable Disposable;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        Disposable?.Dispose();
        if (gameObject.activeInHierarchy)
            Disposable = ObservableAction(computedValue)
                .TakeUntilDisable(this).Subscribe();
    }

    public virtual IObservable<Unit> ObservableAction(int value)
    {
        return UniRx.Observable.FromCoroutine(() => IeOnChanged(value));
    }

    public virtual IEnumerator IeOnChanged(int value)
    {
        if (value < 0)
            yield return null;
    }
}