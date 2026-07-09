using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BindingCurrentEnum
{
    NotAvailable,
    Available,
    Current
}

public class IndexBindingCurrent : BaseBindingAction
{
    protected override void OnShow(bool overwritten)
    {
        
    }

    [SerializeField] private bool currentAsAvailable;
    public void OnChanged(BindingCurrentEnum state)
    {
        switch (state)
        {
            case BindingCurrentEnum.NotAvailable:
                foreach (var toggleScript in toggleNotAvailable)
                    toggleScript.OnChanged(true);
                foreach (var toggleScript in toggleAvailable)
                    toggleScript.OnChanged(false);
                foreach (var toggleScript in toggleCurrent)
                    toggleScript.OnChanged(false);
                break;
            case BindingCurrentEnum.Available:
                foreach (var toggleScript in toggleNotAvailable)
                    toggleScript.OnChanged(false);
                foreach (var toggleScript in toggleAvailable)
                    toggleScript.OnChanged(true);
                foreach (var toggleScript in toggleCurrent)
                    toggleScript.OnChanged(false);
                break;
            case BindingCurrentEnum.Current:
                foreach (var toggleScript in toggleNotAvailable)
                    toggleScript.OnChanged(false);
                foreach (var toggleScript in toggleAvailable)
                    toggleScript.OnChanged(currentAsAvailable);
                foreach (var toggleScript in toggleCurrent)
                    toggleScript.OnChanged(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public ToggleScript[] toggleNotAvailable;
    public ToggleScript[] toggleAvailable;
    public ToggleScript[] toggleCurrent;
}
