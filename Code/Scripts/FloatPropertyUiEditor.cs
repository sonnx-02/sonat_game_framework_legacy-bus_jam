using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FloatPropertyUiEditor : BaseFloatPropertySourceRetriever
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button addButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private float delta = 0.5f;
    [SerializeField] private TMP_Text desc;
    [SerializeField] private string label;


    public override void Start()
    {
        base.Start();
        var p = GetProperty();
        if (desc != null)
        {
            desc.text = label + "(" + delta + ")";
        }

        p.Subscribe(data => text.text = data.ToString(CultureInfo.InvariantCulture));
        if (addButton != null)
            addButton.onClick.AddListener(() => p.Value += delta);
        if (minusButton != null) 
            minusButton.onClick.AddListener(() => p.Value -= delta);
    }

    [MyButtonInt(nameof(TestChange), nameof(Log))] [SerializeField]
    private int testChanged;

    public void TestChange(int value)
    {
        Property.Value = value;
    }

    public void Log(int value)
    {
        Debug.Log(Property.Value);
    }

    protected override string GetName()
    {
        desc.text = label;
        return base.GetName() + "_" + label;
    }
}