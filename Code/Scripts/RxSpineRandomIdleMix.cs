//#define USE_SPINE
using System;
using System.Collections;
#if USE_SPINE
using Spine.Unity;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

public class RxSpineRandomIdleMix : BaseRxAnimator
{
    [SerializeField] private IntString[] randoms;

#if USE_SPINE
    public SkeletonGraphic skeleton;

    protected override void Register()
    {
        base.Register();
        skeleton.Initialize(true);
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        CheckRegister();
    }

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        yield return null;

        var rand = randoms.TakeByWeight(randoms.Select(x => x.value).ToList());

        skeleton.AnimationState.ClearTrack(0);
        skeleton.Skeleton.SetToSetupPose();
        skeleton.AnimationState.SetAnimation(0, rand.value2, false);
        skeleton.Update(0f);
        float animTime = skeleton.AnimationState.GetCurrent(0).Animation.Duration;
        yield return new WaitForSeconds(animTime);
        yield return null;

        while (true)
        {
            rand = randoms.TakeByWeight(randoms.Select(x => x.value).ToList());
            skeleton.AnimationState.ClearTrack(0);
            skeleton.Skeleton.SetToSetupPose();
            skeleton.AnimationState.SetAnimation(0, rand.value2, false);
            animTime = skeleton.AnimationState.GetCurrent(0).Animation.Duration;
            yield return new WaitForSeconds(animTime);

            yield return null;
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return null;
    }
#else
    public override bool HasOut()
    {
        throw new System.NotImplementedException();
    }

    public override bool HasIn()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        throw new System.NotImplementedException();
    }

#endif
}