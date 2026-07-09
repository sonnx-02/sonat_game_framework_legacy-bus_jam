using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ToggleScriptSizeDelta : ToggleScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 sizeOn = new Vector2(0,-100);
    [SerializeField] private Vector2 sizeOff;
    [SerializeField] private BoolFloat tween;
    [SerializeField] private bool ratioRemap;
    [SerializeField] private Vector4 ratioRemapValue;
    [SerializeField] private AnimationCurve remapRatioCurve;
//    [SerializeField] private bool resolutionRemap;
//    [SerializeField] private Vector4 resolutionRemapValue;
//    [SerializeField] private AnimationCurve resolutionRemapCurve;
    [SerializeField] private Camera cam;

    protected override void LoadComponent()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual Vector2 GetSize(bool val)
    {
        return val ? sizeOn : sizeOff;
    }

    protected override void Trigger(bool val)
    {
        var size = GetSize(val);
      
        var c = cam != null ? cam : Camera.main;
        if (c != null)
        {
            if (ratioRemap)
                size.y += DuongExtensions.Remap(ratioRemapValue,c.aspect,remapRatioCurve);
//            if (resolutionRemap)
//                size.y += DuongExtensions.Remap(resolutionRemapValue, c.pixelHeight * c.pixelWidth,resolutionRemapCurve);
        }
     
        
        if(!tween.value)
            rectTransform.sizeDelta = size;
        else
        {
            rectTransform.DOKill();
            rectTransform.DOSizeDelta(size, tween.value2);
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
