using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActionScriptToggleRxFrame : BaseBooleanActionScript
{
    [SerializeField] private RxFrameAnimationScript animData;

    public override void StartAction(bool value)
    {
        base.StartAction(value);
        if (value)
            animData.PlayOnce(0);
        else
            animData.PlayReverseOnce(0);
    }

    public override void Prepare()
    {
        
    }
}