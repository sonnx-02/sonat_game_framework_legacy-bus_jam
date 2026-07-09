using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public struct RectCopy
{
    [FormerlySerializedAs("AnchorPosition")] public Vector2 anchorPosition;
    [FormerlySerializedAs("Size")] public Vector2 size;
    [FormerlySerializedAs("AnchorMin")] public Vector2 anchorMin;
    [FormerlySerializedAs("AnchorMax")] public Vector2 anchorMax;
    [FormerlySerializedAs("Pivot")] public Vector2 pivot;


    public static RectCopy Copy(RectTransform copy)
    {
        return new RectCopy()
        {
            anchorPosition =  copy.anchoredPosition,
            size =   copy.sizeDelta,
            anchorMin =  copy.anchorMin,
            anchorMax = copy.anchorMax,
            pivot = copy.pivot,
        };
    }

    public void Apply(RectTransform rect)
    {
        rect.anchoredPosition = anchorPosition;
        rect.sizeDelta = size;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
    }
}

public class NotchFixer : ToggleScript
{
    [FormerlySerializedAs("WhenHavingNotch")] public RectCopy whenHavingNotch;
    [FormerlySerializedAs("WhenNotHavingNotch")] public RectCopy whenNotHavingNotch;
    
    public override void Start()
    {
        base.Start();
        if(isHaveNotch())
            whenHavingNotch.Apply(transform as RectTransform);
        else
            whenNotHavingNotch.Apply(transform as RectTransform);
    }


    protected override void LoadOn()
    {
        base.LoadOn();
        CopyWhenHaveNotch();
    }

    protected override void LoadOff()
    {
        base.LoadOff();
        CopyWhenNotHaveNotch();
    }

    public void CopyWhenHaveNotch()
    {
        whenHavingNotch = RectCopy.Copy(transform as RectTransform);
    }
    
    public void CopyWhenNotHaveNotch()
    {
        whenNotHavingNotch = RectCopy.Copy(transform as RectTransform);
    }
    
    public void ApplyWhenHaveNotch()
    {
        whenHavingNotch.Apply(transform as RectTransform);
    }
    
    public void ApplyWhenNotHaveNotch()
    {
        whenNotHavingNotch.Apply(transform as RectTransform);
    }

    bool isHaveNotch()
    {
//        return true;
        return Screen.safeArea.height < Screen.height || Screen.safeArea.width < Screen.width;
    }

    protected override void LoadComponent()
    {
        
    }

    protected override void Trigger(bool val)
    {
        if(isHaveNotch())
            whenHavingNotch.Apply(transform as RectTransform);
    }
}