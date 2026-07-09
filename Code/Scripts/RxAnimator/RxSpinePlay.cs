//#define USE_SPINE

using System;
using System.Collections;
#if USE_SPINE
using Spine.Unity;
#endif
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class SpineSequence
{
    public int track;
    public string animationName;
    public bool loop;
    public bool wait;
    public float delayAfter;
}

public class RxSpinePlay : BaseRxAnimator
{
    public SpineSequence[] spineSequences;
#if USE_SPINE
    public SkeletonGraphic skeleton;

    public float activeAfter;

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        gameObject.SetActive(false);
        skeleton.Initialize(true);
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return new WaitForSeconds(activeAfter);
        
        skeleton.AnimationState.ClearTrack(0);
        skeleton.Skeleton.SetToSetupPose();
        if (!string.IsNullOrEmpty(spineSequences[0].animationName)) {
            skeleton.AnimationState.SetAnimation(0, spineSequences[0].animationName, false);
            skeleton.Update(0f);
        }
        
        gameObject.SetActive(true);
        for (int i = 0; i < spineSequences.Length; i++)
        {
            skeleton.AnimationState.SetAnimation(spineSequences[i].track, spineSequences[i].animationName,
                spineSequences[i].loop);

            Debug.Log("name"+ spineSequences[i].animationName + spineSequences[i].loop);
            if (spineSequences[i].wait)
            {
                float animTime = skeleton.AnimationState.GetCurrent(spineSequences[i].track).Animation.Duration;
                Debug.Log("wait" + animTime);
                yield return new WaitForSeconds(animTime);
            }
            if (spineSequences[i].delayAfter > 0 )
                yield return new WaitForSeconds(spineSequences[i].delayAfter);
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return ThisDoAnimate_In();
    }
#else
    public override bool HasOut()
    {
        throw new NotImplementedException();
    }

    public override bool HasIn()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        throw new NotImplementedException();
    }
#endif
}