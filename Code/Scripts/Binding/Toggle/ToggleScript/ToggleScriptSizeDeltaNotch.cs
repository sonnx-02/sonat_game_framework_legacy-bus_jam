using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ToggleScriptSizeDeltaNotch : ToggleScriptSizeDelta
{
    [SerializeField] private Vector2 sizeOnNotch = new Vector2(0,-100);
    [SerializeField] private Vector2 sizeOffNotch;

    public override Vector2 GetSize(bool val)
    {
        if(!IsHaveNotch())
            return base.GetSize(val);
        else
            return val ? sizeOnNotch : sizeOffNotch;
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

}
