using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCustomByIndex : BasePopup
{
    [SerializeField] private IndexBindingScriptBase[] bindings;
    public override void SetParameter(CustomParameter parameter)
    {
        base.SetParameter(parameter);
        bindings.OnChanged(parameter.intValue);
    }
}
