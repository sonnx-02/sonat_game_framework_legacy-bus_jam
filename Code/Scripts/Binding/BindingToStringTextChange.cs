using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.RegularExpressions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BindingToStringTextChange : BaseFrameworkView
{
    private Text _text;
    private TextMeshProUGUI _textMesh;
    [FormerlySerializedAs("DisplayFormat")]  
    [SerializeField] private string displayFormat = "{0}";
    [SerializeField] private bool upperCase;
    [SerializeField] private bool removeHtmlTag;
    [SerializeField] private string defaultText;
    public void OnChanged(string text)
    {
        SetText(GetText(FormatText(text)));
    }

    public void SetDefaultText()
    {
        SetText(defaultText);
    }

    public string GetText(string text)
    {
        if (removeHtmlTag)
            return Regex.Replace(text, "<.*?>", String.Empty);

        return text;
    }

    public void OnChanged(string text1, string text2)
    {
        SetText(GetText(FormatText(text1, text2)));
    }

    string FormatText(string text1, string text2)
    {
        if (displayFormat == "")
            displayFormat = "{0}";
        else if (displayFormat.Contains("{0}") == false)
        {
            Debug.Log("DisplayFormat is not valid");
            displayFormat = "{0}";
        }

        if (upperCase)
            return String.Format(displayFormat, text1, text2).ToUpper();
        else
            return String.Format(displayFormat, text1, text2);
    }

    string FormatText(string text)
    {
        if (displayFormat == "")
            displayFormat = "{0}";
        else if (displayFormat.Contains("{0}") == false)
        {
            Debug.Log("DisplayFormat is not valid");
            displayFormat = "{0}";
        }

        if (upperCase)
            return String.Format(displayFormat, text).ToUpper();
        else
            return String.Format(displayFormat, text);
    }

    void SetText(string text)
    {
        if (_textMesh == null)
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh != null)
                _textMesh.text = text;
            else if (_text == null)
            {
                _text = GetComponent<Text>();
                if (_text != null)
                    _text.text = text;
                else
                    Debug.Log("not found text");
            }
            else
                _text.text = text;
        }
        else
            _textMesh.text = text;

    }
}
