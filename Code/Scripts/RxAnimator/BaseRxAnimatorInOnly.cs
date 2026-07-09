using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRxAnimatorInOnly : BaseRxAnimator
{
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }


    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
