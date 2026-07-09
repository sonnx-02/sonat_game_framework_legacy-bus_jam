using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptCheckWon : BaseActionScript
{
    protected override void DoAction()
    {
        base.DoAction();
        BaseGameController.CheckWon();
    }
}
