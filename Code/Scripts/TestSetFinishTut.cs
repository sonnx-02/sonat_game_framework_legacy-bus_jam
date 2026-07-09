using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetFinishTut : BaseView
{
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        #if UNITY_EDITOR
        BaseFrameworkView.PlayerData.tutorialStep = -1;
        #endif
    }
}
