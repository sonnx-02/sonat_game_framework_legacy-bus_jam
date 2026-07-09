using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTurnRaycast : BaseActionScript
{
    [SerializeField] private float delay = 1f;

    public override void StartAction()
    {
        base.StartAction();
        ScreenRoot.TurnRayCast(false);
        delay.Timer(() =>
        {
//            if(!ScreenRoot.IsEventSystemEnable)
                ScreenRoot.TurnRayCast(true);
        });
    }
}
