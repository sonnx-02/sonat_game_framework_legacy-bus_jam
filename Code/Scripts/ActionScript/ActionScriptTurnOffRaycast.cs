using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptTurnOffRaycast : BaseActionScript
{
    [SerializeField] private BoolFloat turnOn;
    protected override void DoAction()
    {
        base.DoAction();
        if(turnOn.value)
            ScreenRoot.TurnRayCast(true);
        else
        {
            ScreenRoot.TurnRayCast(false);
            if (turnOn.value2 > 0)
                turnOn.value2.Timer(() => ScreenRoot.TurnRayCast(true));
        }

    }
}
