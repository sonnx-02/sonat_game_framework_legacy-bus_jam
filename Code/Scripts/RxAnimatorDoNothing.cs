using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxAnimatorDoNothing : BaseRxAnimator
{
    [SerializeField] private float timeIn = 1;

    public override bool HasOut() => false;

    public override bool HasIn() => true;

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        state = RxAnimatorState.MovingIn;

        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        yield return new WaitForSeconds(timeIn);

        TriggerAfterInParticle(settings.DelayBeforeIn - settings.particleTimeAdjust);
        InvokeCallBack();
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
