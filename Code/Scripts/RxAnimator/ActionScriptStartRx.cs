using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptStartRx : BaseActionScript
{
    [SerializeField] private BaseRxAnimator[] moveIns;
    [SerializeField] private BaseRxAnimator[] moveOuts;
    protected override void DoAction()
    {
        base.DoAction();
        foreach (var baseRxAnimator in moveIns)
            baseRxAnimator.Animator_In();
        foreach (var baseRxAnimator in moveOuts)
            baseRxAnimator.Animator_Out();
    }
}
