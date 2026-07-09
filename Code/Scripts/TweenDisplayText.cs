using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

public class TweenDisplayText : TweenDisplayScript
{
    [SerializeField] private IntString excludeCase = new IntString(){value = -100,value2 = "."};
    [SerializeField] private TMP_Text text;
    [SerializeField] private BaseMyCustomFontText customFontText;
    [SerializeField] private string displayFormat = "{0}";
    [SerializeField] private string toStringFormat = "D4";
   
    
#if UNITY_EDITOR
    void Reset()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();
        if (customFontText == null)
            customFontText = GetComponent<BaseMyCustomFontText>();
    }
#endif
    
    public override void SetDisplayValue(int value)
    {
        if (text != null)
        {
            if (value == excludeCase.value)
                text.text = excludeCase.value2;
            else
                text.text = String.Format(displayFormat, value.ToString(toStringFormat));
        }
        if (customFontText != null)
            if (value == excludeCase.value)
                customFontText.SetText(excludeCase.value2);
            else
             customFontText.SetText(String.Format(displayFormat, value.ToString(toStringFormat)));
    }
}