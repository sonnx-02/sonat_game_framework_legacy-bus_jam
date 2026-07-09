using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxAnimatorTransformScale : BaseRxAnimator
{
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 inTo;
    [SerializeField] private Vector3 outTo;
    [SerializeField] private BoolFloatFloat timeIn;// active,time,delay
    [SerializeField] private BoolFloatFloat timeOut;
    [SerializeField] private Ease easeIn;
    [SerializeField] private Ease easeOut;
    
    
     public override void ResetAnimation(bool setActive = false)
    {
        CheckRegister();
        base.ResetAnimation(setActive);
        state = RxAnimatorState.Reset;
        Transform transform1;
        (transform1 = transform).DOKill();
        transform1.localEulerAngles = @from;
        TurnOffStartParticle();
    }

    public override void SetAtAnimationIn(bool setActive)
    {
        CheckRegister();
        base.SetAtAnimationIn(setActive);
        state = RxAnimatorState.Idle;
        Transform transform1;
        (transform1 = transform).DOKill();
        transform1.localEulerAngles = inTo;
        TurnOffStartParticle();
    }

    
    public override bool HasOut()
    {
        return timeOut.value;
    }

    public override bool HasIn()
    {
        return timeIn.value;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (timeIn.value)
        {
            busy = true;
            state = RxAnimatorState.MovingIn;
            var waitMoveIn = timeIn.value2 + timeIn.value3;
            var tween = transform.DOLocalRotate(inTo,timeIn.value2)
                .SetDelay(timeIn.value3)
                .SetEase(easeIn)
                .SetUpdate(true);
            
            TriggerAfterInParticle(waitMoveIn - settings.particleTimeAdjust);
            yield return CoroutineUtilities.WaitForRealTime(waitMoveIn);
            InvokeCallBack();
            busy = false;
            state = RxAnimatorState.Idle;
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        CheckRegister();
        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        StartOutSound();

        if (timeOut.value)
        {
            busy = true;
            state = RxAnimatorState.MovingOut;
            var waitMoveIn = timeOut.value2 + timeOut.value3;
            var tween = transform.DOLocalRotate(outTo,timeOut.value2)
                .SetDelay(timeOut.value3)
                .SetEase(easeIn)
                .SetUpdate(true);
            
            yield return CoroutineUtilities.WaitForRealTime(waitMoveIn);
            if (settings.deactiveAfterOut)
                gameObject.SetActive(false);

            busy = false;
            state = RxAnimatorState.Reset;
        }
        
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        CheckRegister();
        TriggerStartParticle();
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (timeIn.value)
        {
            busy = true;
            state = RxAnimatorState.MovingIn;
            var waitMoveIn = timeIn.value2 + timeIn.value3;
            var tween = transform.DOLocalRotate(inTo,timeIn.value2)
                .SetDelay(timeIn.value3)
                .SetEase(easeIn)
                .SetUpdate(true);
            
            TriggerAfterInParticle(waitMoveIn - settings.particleTimeAdjust);
            yield return CoroutineUtilities.WaitForRealTime(waitMoveIn);
            InvokeCallBack();
            busy = false;
            state = RxAnimatorState.Idle;
        }
        
        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        StartOutSound();
        if (timeOut.value)
        {
            busy = true;
            state = RxAnimatorState.MovingOut;
            var waitMoveIn = timeOut.value2 + timeOut.value3;
            var tween = transform.DOLocalRotate(outTo,timeOut.value2)
                .SetDelay(timeOut.value3)
                .SetEase(easeIn)
                .SetUpdate(true);
            
            yield return CoroutineUtilities.WaitForRealTime(waitMoveIn);
            if (settings.deactiveAfterOut)
                gameObject.SetActive(false);

            busy = false;
            state = RxAnimatorState.Reset;
        }
    }
}
