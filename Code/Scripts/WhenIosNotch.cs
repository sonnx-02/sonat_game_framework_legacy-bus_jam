using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenIosNotch : BaseActionScript
{
//    [SerializeField] private bool forceIosNotch;
    [SerializeField] private ToggleScript[] toggleWhenOn;
    [SerializeField] private bool onlyIos;
    protected override void DoAction()
    {
        base.DoAction();
//        #if UNITY_EDITOR
//        if (forceIosNotch)
//            toggleWhenOn.OnChanged(true);
//        #else
//#if UNITY_IOS
//        if(IsHaveNotch())
            toggleWhenOn.OnChanged(IsHaveNotch());
//        #endif
    }

    private bool IsHaveNotch()
    {
#if UNITY_IOS
        return Screen.safeArea.height < Screen.height || Screen.safeArea.width < Screen.width;
#endif
#if UNITY_ANDROID
        if (onlyIos)
            return false;
        #endif
        return Screen.safeArea.height < Screen.height || Screen.safeArea.width < Screen.width;
    }

   

    public override void LogCustom()
    {
        base.LogCustom();
    }

    public override void LogCustomInt(int value)
    {
     
    }
}
