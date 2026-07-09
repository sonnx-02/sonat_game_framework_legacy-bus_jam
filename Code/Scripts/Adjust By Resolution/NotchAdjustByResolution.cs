using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotchAnchorApply : ResolutionAdjustApply
{
    public float NotchRate = 0.075f;
}

public class NotchAdjustByResolution : BaseAdjustByResolution
{
    public RectTransform NotchTop;
    public RectTransform NotchBottom;
    public RectTransform[] Screens;

    public List<NotchAnchorApply> Conditions = new List<NotchAnchorApply>();
    public override ResolutionAdjustApply this[int index] => Conditions[index];
    public override int Count => Conditions.Count;

    public static float NotchRate = 0;

    protected override void DoApply<T>(T apply)
    {
        var currentApply = apply as NotchAnchorApply;
        NotchTop.anchorMin = new Vector2(0, 1 - currentApply.NotchRate);
        NotchTop.anchorMax = Vector2.one;
        NotchTop.anchoredPosition = Vector2.zero;
        NotchTop.sizeDelta = Vector2.zero;

        NotchBottom.anchorMin = Vector2.zero;
        NotchBottom.anchorMax = new Vector2(1, currentApply.NotchRate);
        NotchBottom.anchoredPosition = Vector2.zero;
        NotchBottom.sizeDelta = Vector2.zero;

        foreach (var rectTransform in Screens)
        {
            rectTransform.anchorMin = new Vector2(0, currentApply.NotchRate);
            rectTransform.anchorMax = new Vector2(1, 1 - currentApply.NotchRate);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        NotchRate = currentApply.NotchRate;
    }

    public override void AddCurrentAdjust()
    {
        Conditions.Add(new NotchAnchorApply()
        {
            aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight,
            NotchRate = (float)Math.Round(1- NotchTop.anchorMin.y,3)
        });
    }

    public override void ModifyCondition(ResolutionAdjustApply apply)
    {
        var adjust = apply as NotchAnchorApply;
        adjust.NotchRate = (float) Math.Round(1 - NotchTop.anchorMin.y, 3);
    }
}
