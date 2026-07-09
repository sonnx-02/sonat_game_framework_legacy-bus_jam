using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxRotate2D : BaseRxAnimator
{
    [SerializeField] private float inValue;
    [SerializeField] private Ease easeIn;
    [SerializeField] private float timeIn = 0.3f;
    
    [SerializeField] private float outValue;
    [SerializeField] private Ease easeOut;
    [SerializeField] private float timeOut = 0.3f;
   
    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        transform.localEulerAngles = new Vector3(0,0,outValue);
    }

    public override void SetAtAnimationIn()
    {
        base.SetAtAnimationIn();
        transform.localEulerAngles = Vector3.zero;
    }

    public override bool HasIn()
    {
        return true;
    }
    
    public override bool HasOut()
    {
        return true;
    }


    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        busy = true;
        yield return new WaitForSeconds(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        state = RxAnimatorState.MovingIn;
        yield return DOTween.To(() => outValue, x => { transform.localEulerAngles = new Vector3(0, 0, x); }, inValue, timeIn)
            .SetEase(easeIn);
        
        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        CheckRegister();
        yield return new WaitForSeconds(settings.delayAfterOut);
        StartOutSound();
        busy = true;
        state = RxAnimatorState.MovingOut;
        
        yield return DOTween.To(() => inValue, x => { transform.localEulerAngles = new Vector3(0, 0, x); }, outValue, timeOut)
            .SetEase(easeOut);
        
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);

        busy = false;
        state = RxAnimatorState.Reset;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        CheckRegister();
        TriggerStartParticle();
        busy = true;
        yield return new WaitForSeconds(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        state = RxAnimatorState.MovingIn;
        yield return DOTween.To(() => transform.localEulerAngles.z, x => { transform.localEulerAngles = new Vector3(0, 0, x); }, inValue, timeIn)
            .SetEase(easeIn);
        
        state = RxAnimatorState.Idle;
        
        yield return new WaitForSeconds(settings.delayAfterOut);
        StartOutSound();
        state = RxAnimatorState.MovingOut;
        yield return DOTween.To(() => transform.localEulerAngles.z, x => { transform.localEulerAngles = new Vector3(0, 0, x); }, outValue, timeOut)
            .SetEase(easeOut);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        busy = false;
        state = RxAnimatorState.Reset;
    }
}
