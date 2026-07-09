using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class IndexBindingTextPreset : IndexBindingScript
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private string[] presetTexts;
    [SerializeField] private Color[] presetColors;
    [SerializeField] private PresetTextDatabase database;
    [SerializeField] private bool alphabet;
    [SerializeField] private string onNegative = "-";
    protected override void LoadComponent()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    protected override bool IsNoComponent()
    {
        return textMesh == null;
    }

    private void SetText(string text)
    {
        textMesh.text = text;
    }

    private static readonly string[] Alphabet = {"A","B","C","D","E","F","G","H"};

    private string GetText(int index)
    {
        if (index < 0)
            return onNegative;
        if (alphabet)
            return Alphabet[index];
        if (database != null) return database[index];
        if (!presetTexts.InRange(index))
        {
            Debug.LogError("not in range " + index,gameObject);
            return "not found";
        }

        return presetTexts[index];
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        SetText(GetText(computedValue));
        if (presetColors.InRange(computedValue) && textMesh != null)
            textMesh.color = presetColors[computedValue];
        foreach (var baseIndexBindingAction in sharedSetting.others)
            baseIndexBindingAction.OnChanged(computedValue);
    }
}
