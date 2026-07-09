using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IndexPropertyChangeScript_Toggle  : IndexPropertyChangeScriptBase
{
    protected override int Value { get; set; }

    protected override void OnClick()
    {
        source.BoolValue = !source.BoolValue;
    }

    protected override  void OnChanged()
    {
        toggles.OnChanged(source.BoolValue);
    }

    protected override string GetName()
    {
        return "toggle_".ToLower();
    }

    
}
