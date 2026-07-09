using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressScriptIntText : ProgressScriptInt
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private BaseMyCustomFontText customText;
    [SerializeField] private string displayFormat = "{0}/{1}";
    [SerializeField] private DisplayFormatEnum displayType;

    [SerializeField]
    private enum DisplayFormatEnum
    {
        Default,
        Percent,
        ValueOnly
    }

    protected override void Trigger(int val)
    {
        base.Trigger(val);
        switch (displayType)
        {
            case DisplayFormatEnum.Default:
                SetText(String.Format(displayFormat, val, Target));
                break;
            case DisplayFormatEnum.Percent:
                SetText(Target > 0 ? $"{val*100/Target}%" : "100%");
                break;
            case DisplayFormatEnum.ValueOnly:
                SetText(String.Format(displayFormat, val));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetText(string text)
    {
        if (textMesh != null)
            textMesh.text = text;
        if (customText != null)
            customText.SetText(text);
    }
}