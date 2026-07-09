using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ToggleScriptSizeDelta2 : ToggleScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 sizeOn = new Vector2(0,-100);
    [SerializeField] private Vector2 sizeOff;
    [SerializeField] private BoolFloat tween;
    [SerializeField] private bool ratioRemap;
    [SerializeField] private Vector4 remap;
    [SerializeField] private AnimationCurve remapCurve;
    protected override void LoadComponent()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected override void Trigger(bool val)
    {
        if(!tween.value)
            rectTransform.sizeDelta = val ? sizeOn : sizeOff;
        else
        {
            rectTransform.DOKill();
            rectTransform.DOSizeDelta(val ? sizeOn : sizeOff, tween.value2);
        }
    }

    protected override void LoadOff()
    {
        base.LoadOff();
        sizeOff = rectTransform.sizeDelta;
    }

    protected override void LoadOn()
    {
        base.LoadOn();
        sizeOn = rectTransform.sizeDelta;
    }
}
