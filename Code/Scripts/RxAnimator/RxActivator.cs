using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxActivator : ToggleScript
{
    #if UNITY_EDITOR
    [SerializeField] private string desc;
    #endif
    [SerializeField] private BaseRxAnimator[] animsIn;
    [SerializeField] private BaseRxAnimator[] animsOut;
    [SerializeField] private ToggleScript[] toggleOn;
    [SerializeField] private ToggleScript[] toggleOff;
    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        toggleOn.OnChanged(val);
        toggleOff.OnChanged(!val);
        if (val)
        {
            foreach (var baseRxAnimator in animsIn)
                baseRxAnimator.Animator_In();
            foreach (var baseRxAnimator in animsOut)
                baseRxAnimator.Animator_Out();
        }
        else
        {
            foreach (var baseRxAnimator in animsIn)
                baseRxAnimator.Animator_Out();
            foreach (var baseRxAnimator in animsOut)
                baseRxAnimator.Animator_In();
        }
          
    }
    
    protected void TriggerReset(bool val)
    {
        if (val)
        {
            foreach (var baseRxAnimator in animsIn)
                baseRxAnimator.SetAtIn();
            foreach (var baseRxAnimator in animsOut)
                baseRxAnimator.ResetAnim();
        }
        else
        {
            foreach (var baseRxAnimator in animsIn)
                baseRxAnimator.ResetAnim();
            foreach (var baseRxAnimator in animsOut)
                baseRxAnimator.SetAtIn();
        }
          
    }
}
