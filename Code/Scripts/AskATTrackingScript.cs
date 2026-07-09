using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskATTrackingScript : BaseActionScript
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void DoAction()
    {
        base.DoAction();
        AdsManager.AskAtt();
        Debug.Log("AskATTrackingScript");
    }

    
}
