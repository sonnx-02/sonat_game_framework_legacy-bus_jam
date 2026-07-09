using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptActivator : BaseBaseActionScript
{
    [SerializeField] private Activator activator;
    [SerializeField] private BaseRxAnimator[] animsIn;
    [SerializeField] private BaseRxAnimator[] animsOut;
    protected override void DoAction()
    {
        base.DoAction();
        activator.OnActivate();
        foreach (var baseRxAnimator in animsIn)
            baseRxAnimator.Animator_In();
        foreach (var baseRxAnimator in animsOut)
            baseRxAnimator.Animator_Out();
    }
}
