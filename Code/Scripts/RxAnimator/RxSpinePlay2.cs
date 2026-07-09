//#define USE_SPINE

using System;
using System.Collections;
using UniRx;
#if USE_SPINE
using Spine.Unity;
#endif
using UnityEngine;
using UnityEngine.Serialization;


public class RxSpinePlay2 : BaseRxAnimator
{
    [SerializeField] private SpineSequence[] spineSequences;
#if USE_SPINE
    [SerializeField] private  SkeletonGraphic skeleton;
    [SerializeField] private  SkeletonAnimation skeleton2;
    [DisplayAsDisable]
    [SerializeField] private  float activeAfter;

    protected virtual SpineSequence[] SpineSequences => spineSequences;
    public override void SetIndex(int value)
    {
        base.SetIndex(value);
    }

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override void Register()
    {
        base.Register();
        if (skeleton == null)
            skeleton = GetComponent<SkeletonGraphic>();
        if (skeleton2 == null)
            skeleton2 = GetComponent<SkeletonAnimation>();
        if (skeleton != null)
            skeleton.Initialize(true);
        if (skeleton2 != null)
            skeleton2.Initialize(true);

//        if (settings.startOnEnable)
//        {
//            Debug.LogError("cannot start on Enable this");
//            Debug.Break();
//        }
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        if (!settings.startOnEnable)
            gameObject.SetActive(false);
        CheckRegister();
    }

    [MyButtonString("Test", "Test")] public string testabc;

    public void Test(string n)
    {
//        skeleton.AnimationState.ClearTrack(0);
//        skeleton.Skeleton.SetToSetupPose();
//        if (!string.IsNullOrEmpty(n))
//        {
//            skeleton.AnimationState.SetAnimation(0, n, false);
//            skeleton.Update(0f);
//        }
    }

   

    public override void Animator_In()
    {
        if (!settings.startOnEnable)
            if (activeAfter > 0)
                gameObject.SetActive(false);
        base.Animator_In();
    }

    [SerializeField] private bool loop;
    [SerializeField] private Vector2 loopRandom;


    protected override IEnumerator ThisDoAnimate_In()
    {
        do
        {
            if(activeAfter > 0)
                yield return new WaitForSeconds(activeAfter);
            else
                if(settings.DelayBeforeIn > 0)
                    yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
          
            if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
                StartInSound();
     
            
            var sequences = SpineSequences;
            if (skeleton != null)
            {
                settings.animInSound.PlaySound();
                if(skeleton.AnimationState == null)
                    Debug.LogError(transform.gameObject,transform.gameObject);
                skeleton.AnimationState.ClearTrack(0);
                skeleton.Skeleton.SetToSetupPose();
                if (!string.IsNullOrEmpty(sequences[0].animationName))
                {
                    skeleton.AnimationState.SetAnimation(0, sequences[0].animationName, false);
                    skeleton.Update(0f);
                }

                if (!settings.startOnEnable)
                    gameObject.SetActive(true);

                for (int i = 0; i < sequences.Length; i++)
                {
                    if (i > 0)
                        skeleton.AnimationState.SetAnimation(sequences[i].track, sequences[i].animationName,
                            sequences[i].loop);

                    //      Debug.Log("name"+ sequences[i].animationName + sequences[i].loop);
                    if (sequences[i].wait)
                    {
                        float animTime = skeleton.AnimationState.GetCurrent(sequences[i].track).Animation.Duration;
                        //      Debug.Log("wait" + animTime);
                        yield return new WaitForSecondsRealtime(animTime);
                    }

                    if (sequences[i].delayAfter > 0)
                        yield return new WaitForSecondsRealtime(sequences[i].delayAfter);
                }
                
                if (loopRandom.magnitude > 0.1)
                    yield return new WaitForSecondsRealtime(loopRandom.GetRandom());
            }

            if (skeleton2 != null)
            {
                settings.animInSound.PlaySound();
                skeleton2.AnimationState.ClearTrack(0);
                skeleton2.Skeleton.SetToSetupPose();
                if (!string.IsNullOrEmpty(sequences[0].animationName))
                {
                    skeleton2.AnimationState.SetAnimation(0, sequences[0].animationName, false);
                    skeleton2.Update(0f);
                }

                gameObject.SetActive(true);
                for (int i = 0; i < sequences.Length; i++)
                {
                    if (i > 0)
                        skeleton2.AnimationState.SetAnimation(sequences[i].track, sequences[i].animationName,
                            sequences[i].loop);

                    //      Debug.Log("name"+ sequences[i].animationName + sequences[i].loop);
                    if (sequences[i].wait)
                    {
                        float animTime = skeleton2.AnimationState.GetCurrent(sequences[i].track).Animation.Duration;
                        //      Debug.Log("wait" + animTime);
                        yield return new WaitForSecondsRealtime(animTime);
                    }

                    if (sequences[i].delayAfter > 0)
                        yield return new WaitForSecondsRealtime(sequences[i].delayAfter);
                }
            }
        } while (loop);
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
#else
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return false;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return null;
    }
#endif
}