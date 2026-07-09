using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RxAnimatorActivate : BaseRxAnimator
{
    [SerializeField]
    private float delay = 1;
    [SerializeField]
    private float randomRangeAdd;
    public float RandomDelay => delay + Random.Range(0, randomRangeAdd);
    [SerializeField] private bool hasOut;

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }


    public override bool HasOut() => hasOut;

    public override bool HasIn()
    {
        return true;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        gameObject.SetActive(false);
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        state = RxAnimatorState.MovingIn;
  
        if(settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        
        TriggerAfterInParticle(settings.DelayBeforeIn-settings.particleTimeAdjust);
        
        InvokeCallBack();
        
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        if (hasOut)
        {
            yield return new WaitForSeconds(RandomDelay);
            if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
                StartInSound();
            gameObject.SetActive(false);
            state = RxAnimatorState.Reset;
        }
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() 
    {
        CheckRegister();
        TriggerStartParticle();
        state = RxAnimatorState.MovingIn;
  
        if(settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        
        TriggerAfterInParticle(settings.DelayBeforeIn-settings.particleTimeAdjust);
        
        InvokeCallBack();
        
        state = RxAnimatorState.Idle;
        
        if (hasOut)
        {
            yield return new WaitForSeconds(RandomDelay);
            if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
                StartInSound();
            gameObject.SetActive(false);
            state = RxAnimatorState.Reset;
        }
    }
    
   

}
