using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptToggleRxResetSetAtIn : BaseBaseActionScript
{
    [SerializeField] private BaseRxAnimator[] anims;

    public override void StartAction(bool value)
    {
        base.StartAction(value);
        foreach (var baseRxAnimator in anims)
            if(value)
                baseRxAnimator.SetAtIn();
            else
                baseRxAnimator.ResetAnimation();
    }
}
