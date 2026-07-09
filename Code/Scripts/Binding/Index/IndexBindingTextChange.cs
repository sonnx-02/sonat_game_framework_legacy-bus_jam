using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class IndexBindingTextChange : IndexBindingScript
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
        if (range.IsZero() || range.InRange(computedValue))
        {
            switch (builtInFormatType)
            {
                case BuiltInFormatType.None:
                    if (hhmmFormat)
                    {
                        if (computedValue < 3600)
                            SetText(TimeSpan.FromSeconds(computedValue).ToString(@"mm\:ss"));
                        else if (computedValue < 86400)
                            SetText(TimeSpan.FromSeconds(computedValue).ToString(@"hh\:mm\:ss"));
                        else
                            SetText(string.Format("{0:dd} days {0:hh\\:mm\\:ss}", TimeSpan.FromSeconds(computedValue)));
                        //     SetText(TimeSpan.FromSeconds(computedValue).ToString(@"dd\.hh\:mm\:ss"));
                    }
                    else
                        SetText(String.Format(displayFormat,
                            ((computedValue + numberAdd) * multiple).ToString(toStringFormat)));
                    break;
                case BuiltInFormatType.HHmm:
                    if (computedValue < 3600)
                        SetText(TimeSpan.FromSeconds(computedValue).ToString(@"mm\:ss"));
                    else if (computedValue < 86400)
                        SetText(TimeSpan.FromSeconds(computedValue).ToString(@"hh\:mm\:ss"));
                    else
                        SetText(string.Format("{0:dd} days {0:hh\\:mm\\:ss}", TimeSpan.FromSeconds(computedValue)));
                    //     SetText(TimeSpan.FromSeconds(computedValue).ToString(@"dd\.hh\:mm\:ss"));
                    break;
                case BuiltInFormatType.HHmmTruncateText:
                    var timeSpan = TimeSpan.FromSeconds(computedValue);
                    if (computedValue % (60 * 60) == 0)
                        SetText($"{timeSpan.TotalHours}h");
                    else if (computedValue < (60 * 60))
                        SetText($"{timeSpan.Minutes}m");
                    else if (computedValue > (60 * 60))
                        SetText($"{Mathf.FloorToInt((float)timeSpan.TotalHours)}h{timeSpan.Minutes}m");
                    else
                        SetText(string.Format("{0:dd} days {0:hh\\:mm\\:ss}", TimeSpan.FromSeconds(computedValue)));
                    break;
                case BuiltInFormatType.Alphabet:
                    SetText(Alphabet[computedValue]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            if(whenOutOfRange2.Length == 0 || computedValue < range.value)
                SetText(whenOutOfRange);
            else
                SetText(whenOutOfRange2[(computedValue-range.value2)% whenOutOfRange2.Length]);
        }

        foreach (var baseIndexBindingAction in sharedSetting.others)
            baseIndexBindingAction.OnChanged(computedValue);
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