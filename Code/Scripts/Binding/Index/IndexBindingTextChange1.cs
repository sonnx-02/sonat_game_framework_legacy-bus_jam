using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IndexBindingTextChange1 : IndexBindingScript
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private BaseMyCustomFontText customText;
    [SerializeField] private string displayFormat = "{0}";
    [SerializeField] private string toStringFormat;
    [SerializeField] private int numberAdd;
    [SerializeField] private int multiple = 1;

    [Tooltip("range 0,0 is all value")]
    [SerializeField] private IntInt range = new IntInt(0,0);
    [SerializeField] private string whenOutOfRange; // value when out of from and to
    [SerializeField] private string[] whenOutOfRange2; // value - max % whenOutOfRange2
    [SerializeField] private bool removeHtmlTag; // remove html text
    [SerializeField] private bool hhmmFormat;
    [SerializeField] private BuiltInFormatType builtInFormatType;
    [SerializeField] private BaseBaseActionScript[] actionOnChanged;
    public enum BuiltInFormatType
    {
        None,
        HHmm,
        HHmmTruncateText,
        Alphabet
    }

    public override void LogCustom()
    {
        base.LogCustom();
        int s = 62;
        TimeSpan time = TimeSpan.FromSeconds(s);

//here backslash is must to tell that colon is
//not the part of format, it just a character that we want in output
        string str = time .ToString(@"mm\:ss");
        
        Debug.Log(str);
    }

    protected override void LoadComponent()
    {
        textMesh = GetComponent<TMP_Text>();
        customText = GetComponent<BaseMyCustomFontText>();
    }

    protected override bool IsNoComponent()
    {
        return textMesh == null && customText == null;
    }


    protected override void Register()
    {
        base.Register();
//        if (textMesh == null && customText == null)
//            Debug.LogError("one text must be set", gameObject);
        #if UNITY_EDITOR
        if (!ignoreValidate)
        {
             if(textMesh != null && GetComponent<TMP_Text>() != null && textMesh.gameObject != gameObject)
                           Debug.LogError("it is pointing other game object", gameObject); 
             if(customText != null && GetComponent<BaseMyCustomFontText>() != null && customText.gameObject != gameObject)
                 Debug.LogError("it is pointing other game object", gameObject); 
        }
        #endif
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (overwriteFormat.active)
        {
            displayFormat = overwriteFormat.GetFormat();
            OnChangedLastValue();
        }
    }

    [MyButton(nameof(GetFormat))] public int abc;

    public void GetFormat()
    {
        var text = GetComponent<TMP_Text>().text;
        displayFormat = Regex.Replace(text, "[0-9]{2,}", "{0}");
    }


    public string LastText { get; set; }
    protected virtual void SetText(string text)
    {
        if (textMesh != null)
            textMesh.text = GetText(text);
        if (customText != null)
            customText.SetText(text);
        LastText = text;
        actionOnChanged.StartAction();
    }
    
    private static readonly string[] Alphabet = {"A","B","C","D","E","F","G","H"};

    private string GetText(string text)
    {
      
        if (removeHtmlTag)
            return Regex.Replace(text, "<.*?>", String.Empty);

        return text;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
    }

    public override void OnChanged2(float value)
    {
        base.OnChanged2(value);
        SetText(String.Format(displayFormat,
                            ((value + numberAdd) * multiple).ToString(toStringFormat)));
    }

    [SerializeField] private OverwriteFormat overwriteFormat;

    [Serializable]
    private class OverwriteFormat
    {
        public bool active = false;
        public IntRetrieverItem retriever;
        public string format = "{0]/{1}";

        public string GetFormat()
        {
            return string.Format(format, retriever.GetValue(), "{0}");
        }
    }
}