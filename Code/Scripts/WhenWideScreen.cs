using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenWideScreen : BaseActionScript
{
    [SerializeField] private float rate;
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private bool notNotch;
    [SerializeField] private bool reverse;
    [SerializeField] private Camera cam;
    [SerializeField] private bool forceOnIos;
    protected override void DoAction()
    {
        base.DoAction();

            toggles.OnChanged( Valid());
    }

    private bool Valid()
    {
        #if UNITY_IOS
        if (forceOnIos)
            return true;
        #endif
        if (notNotch &&  (Screen.safeArea.height < Screen.height || Screen.safeArea.width < Screen.width))
            return false;
        if(!reverse)
            return (cam == null ? Camera.main : cam).aspect >= rate;
        return (cam == null ? Camera.main : cam).aspect < rate;
    }

    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(Camera.main.aspect);
        Debug.Log(Screen.dpi);
        Debug.Log(Camera.main.pixelHeight * Camera.main.pixelWidth);
    }
}
