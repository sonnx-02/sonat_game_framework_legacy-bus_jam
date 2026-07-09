using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RxTrigger : BaseRxAnimator
{
    public override bool HasOut()
    {
        return false;
    }

    [SerializeField] private Button[] buttons;
    [SerializeField] private BaseBaseActionScript[] actions;

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return  CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        foreach (var button in buttons)
        {
            button.onClick.Invoke();
        }
        actions.StartAction();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
