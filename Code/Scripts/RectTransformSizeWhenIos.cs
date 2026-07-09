using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformSizeWhenIos : BaseActionScript
{
    [SerializeField] private bool forceOnEditor;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 sizeWhenNotch = new Vector2(0,-100);
    protected override void DoAction()
    {
        base.DoAction();
#if UNITY_IOS
        if(IsHaveNotch())
            rectTransform.sizeDelta = sizeWhenNotch;
#endif
    }

    private bool IsHaveNotch()
    {
        #if UNITY_EDITOR && UNITY_IOS
        if (forceOnEditor)
            return true;
        #endif
//        return true;
        return Screen.safeArea.height < Screen.height || Screen.safeArea.width < Screen.width;
    }

    

    public override void LogCustom()
    {
        base.LogCustom();
        sizeWhenNotch = rectTransform.sizeDelta;
    }

    public override void LogCustomInt(int value)
    {
        rectTransform.sizeDelta = sizeWhenNotch;
    }
}
