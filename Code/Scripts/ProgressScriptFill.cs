using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScriptFill : ProgressScript
{
    [SerializeField] private Image fillImg;
    [SerializeField] private TMP_Text textPerfect;
    [SerializeField] private bool useRemap;
    [SerializeField] private Vector2 remap;
    protected override void Trigger(float val)
    {
        base.Trigger(val);
        fillImg.fillAmount = !useRemap ? val : GetRemapValue(val, remap);
        if (textPerfect)
            textPerfect.text = Mathf.Min(Mathf.RoundToInt(val * 100),100) + "%";
    }

    private float GetRemapValue(float val,Vector2 rm)
    {
        var total = rm.y - rm.x;
        return rm.x + val * total;
    }
    
    public void SetInstant(float val)
    {
        fillImg.fillAmount = !useRemap ? val : GetRemapValue(val, remap);
        if (textPerfect)
            textPerfect.text = Mathf.Min(Mathf.RoundToInt(val * 100), 100) + "%";
    }
}
