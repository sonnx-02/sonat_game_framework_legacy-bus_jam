using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToggleScriptTextChange : ToggleScript
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private BaseMyCustomFontText customText;
    [SerializeField] private string textOn;
    [SerializeField] private string textOff;
    protected override void LoadComponent()
    {
    }

    private void Reset()
    {
        textMesh = GetComponent<TMP_Text>();
        customText = GetComponent<BaseMyCustomFontText>();
    }

    protected override void Trigger(bool val)
    {
        if (textMesh != null)
            textMesh.text = val ? textOn : textOff;
        if (customText != null)
            customText.SetText(val ? textOn : textOff);
    }
}
