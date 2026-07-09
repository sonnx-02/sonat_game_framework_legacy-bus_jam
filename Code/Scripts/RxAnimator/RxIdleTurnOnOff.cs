using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxIdleTurnOnOff : BaseRxAnimator
{

    [SerializeField] private float timeOn = 1;
    [SerializeField] private float timeOff = 1;
    [SerializeField] private ToggleScript[] toggles;
    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        var waitOn = new WaitForSeconds(timeOn);
        var waitOff = new WaitForSeconds(timeOff);
        while (true)
        {
            toggles.OnChanged(true);
            yield return waitOn;
            toggles.OnChanged(false);
            yield return waitOff;
        }
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
