using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptSetActive : ToggleScript
{
    [SerializeField] private bool disableThis;
    [SerializeField] private GameObject[] thisActive;
    [SerializeField] private GameObject[] otherReverses;
    [SerializeField] private float delayDeactivate;
    [SerializeField] private float delayActivate;
    protected override void LoadComponent()
    {
        
    }

    [DisplayAsDisable] [SerializeField] private bool lastValue;
    private IDisposable _disposable;
    protected override void Trigger(bool val)
    {
        lastValue = val;
        if (!disableThis)
        {
            if (!val)
            {
                if (delayDeactivate <= 0)
                    gameObject.SetActive(false);
                else
                {
                    _disposable?.Dispose();
                    _disposable = delayDeactivate.Timer(() => gameObject.SetActive(lastValue));
                }
            }
            else
            {
                if (delayActivate <= 0)
                    gameObject.SetActive(true);
                else
                {
                    _disposable?.Dispose();
                    _disposable = delayActivate.Timer(() => gameObject.SetActive(lastValue));
                }
            }
        }
        foreach (var otherReverse in otherReverses)
            otherReverse.gameObject.SetActive(!val);
        foreach (var otherReverse in thisActive)
            otherReverse.gameObject.SetActive(val);
    }

    public void ToggleLast()
    {
        Trigger(!lastValue);
    }
}
