using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxPlaySound : BaseRxAnimator
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int sound;

    [SerializeField] private int repeat = 1;
    [SerializeField] private float interval = 0.25f;

    public float inTime;
    public float outTime;
   
    public override bool HasOut()
    {
        return outTime > 0f;
    }

    public override bool HasIn()
    {
        return inTime > 0f;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
        {
            AudioManager.Play(settings.animInClip);
            AudioManager.Play(settings.animInSound);
        }
//        yield return CoroutineUtilities.WaitForRealTime(inTime);
        
        sound.PlaySound();
        int count = 1;
        var i  = CoroutineUtilities.WaitForRealTime(interval);
        while (count < repeat)
        {
            count++;
            yield return i;
            sound.PlaySound();
        }
        
        InvokeCallBack();

        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
        {
            AudioManager.Play(settings.animInClip);
            AudioManager.Play(settings.animInSound);
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);

        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
        {
            AudioManager.Play(settings.animOutClip);
            AudioManager.Play(settings.animOutSound);
        }
        yield return new WaitForSeconds(outTime);
        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
        {
            AudioManager.Play(settings.animOutClip);
            AudioManager.Play(settings.animOutSound);
        }
        
        if (settings.deactiveAfterOut && HasIn())
        {
            gameObject.SetActive(false);
        }
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
      
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
        {
            AudioManager.Play(settings.animInClip);
            AudioManager.Play(settings.animInSound);
        }
        yield return new WaitForSeconds(inTime);
        
        sound.PlaySound();
        int count = 1;
        var i  = new WaitForSeconds(interval);
        while (count < repeat)
        {
            yield return i;
            sound.PlaySound();
        }
        
        InvokeCallBack();

        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
        {
            AudioManager.Play(settings.animInClip);
            AudioManager.Play(settings.animInSound);
        }
        
        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
        {
            AudioManager.Play(settings.animOutClip);
            AudioManager.Play(settings.animOutSound);
        }
        yield return new WaitForSeconds(outTime);
        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
        {
            AudioManager.Play(settings.animOutClip);
            AudioManager.Play(settings.animOutSound);
        }
        
        if (settings.deactiveAfterOut && HasIn())
        {
            gameObject.SetActive(false);
        }
    }
}
