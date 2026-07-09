using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class IndexIndexBindingTextChangeQuanity : IndexIndexBindingScriptBase
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private BaseMyCustomFontText customText;
    [SerializeField] private string defaultText;
    [SerializeField] private QuantityIndexText[] conditions;
    [SerializeField] private bool inactiveIfOutRange;

    protected override void LoadComponent()
    {
        textMesh = GetComponent<TMP_Text>();
        customText = GetComponent<BaseMyCustomFontText>();
    }

  
  
    protected virtual void SetText(string text)
    {
        if (textMesh != null)
            textMesh.text = text;
        if (customText != null)
            customText.SetText(text);
    }

  

    public override void OnChanged(int value, int value2)
    {
        var s = GetSprite((Quantity)value,value2);
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
        
        string GetSprite(Quantity val,int val2)
        {
            foreach (var quantitySprite in conditions)
                if (quantitySprite.quantity == val && quantitySprite.index == val2)
                    return quantitySprite.text;
            return null;
        }
    }
}