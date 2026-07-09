using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingPreset : IndexBindingScript
{
    [SerializeField] private PresetSettingView[] presetSettingViews;
    [SerializeField] private int addValue;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var computed = computedValue + addValue;
        if (computed < 0)
            computed = 0;
        foreach (var presetSettingView in presetSettingViews)
        {
            presetSettingView.ApplyPreset(computed);
        }
    }
}
