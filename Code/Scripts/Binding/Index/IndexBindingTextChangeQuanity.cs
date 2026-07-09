using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class IndexBindingTextChangeQuanity : IndexBindingScript
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private BaseMyCustomFontText customText;
    [SerializeField] private string defaultText;
    [SerializeField] private QuantityText[] conditions;
    [SerializeField] private bool inactiveIfOutRange;

    protected override void LoadComponent()
    {
        textMesh = GetComponent<TMP_Text>();
        customText = GetComponent<BaseMyCustomFontText>();
    }

    protected override bool IsNoComponent()
    {
        return textMesh == null && customText == null;
    }
  
    protected virtual void SetText(string text)
    {
        if (textMesh != null)
            textMesh.text = text;
        if (customText != null)
            customText.SetText(text);
    }

  

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var s = GetSprite((Quantity)computedValue);
        var outRange = s == null;
        if (inactiveIfOutRange)
        {
            gameObject.SetActive(!outRange);
            if(outRange)
                return;
        }
        
        if(s != null)
            SetText(s);
        else
            SetText(defaultText);
        
        string GetSprite(Quantity val)
        {
            foreach (var quantitySprite in conditions)
                if (quantitySprite.quantity == val)
                    return quantitySprite.text;
            return null;
        }
    }
}