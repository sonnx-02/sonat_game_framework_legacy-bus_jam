using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IndexBindingTextChange2 : IndexBindingTextChange
{
    [SerializeField] private TMP_InputField inputField;
  

    protected override void LoadComponent()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    protected override bool IsNoComponent()
    {
        return base.IsNoComponent() && inputField != null;
    }

  
    protected override void SetText(string text)
    {
        base.SetText(text);
        if (inputField != null)
            inputField.text = text;
    }
}