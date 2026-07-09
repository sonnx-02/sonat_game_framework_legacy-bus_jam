using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBindingAction : BaseFrameworkView
{

    [SerializeField]
    protected bool reloadOnEnable;

    protected abstract void OnShow(bool overwritten);

    protected virtual void OnEnable()
    {
        if (reloadOnEnable)
            OnShow(false);
    }
}
