using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingCustomFontChange : IndexBindingScript
{
    [SerializeField] private MyCanvasCustomText font;

    protected override bool IsNoComponent()
    {
        return font == null;
    }

    protected override void LoadComponent()
    {
        font = GetComponent<MyCanvasCustomText>();
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        font.SetFont(computedValue);
    }
}
