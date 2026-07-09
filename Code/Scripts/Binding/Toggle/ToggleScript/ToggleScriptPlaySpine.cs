//#define USE_SPINE

using System.Collections;
using System.Collections.Generic;
#if USE_SPINE
using System;
using Spine.Unity;
using UniRx;
#endif
using UnityEngine;

public class ToggleScriptPlaySpine : ToggleScript
{
    [SerializeField] private float delayBefore;
    [SerializeField] private SpineSequence[] spineSequencesOn;
    [SerializeField] private SpineSequence[] spineSequencesOff;


#if USE_SPINE
    
    [SerializeField] private  SkeletonGraphic skeleton;

    protected override void LoadComponent()
    {
        skeleton = GetComponent<SkeletonGraphic>();
    }

    private IDisposable _disposable;
    protected override void Trigger(bool val)
    {
        _disposable?.Dispose();
        _disposable =  Observable.FromCoroutine(() => Play(val)).Subscribe();
    }

    private IEnumerator Play(bool val)
    {
        yield return new WaitForSeconds(delayBefore);
        var spineSequences = val ? spineSequencesOn : spineSequencesOff;
        skeleton.AnimationState.ClearTrack(0);
        skeleton.Skeleton.SetToSetupPose();
        if (!string.IsNullOrEmpty(spineSequences[0].animationName))
        {
            skeleton.AnimationState.SetAnimation(0, spineSequences[0].animationName, false);
            skeleton.Update(0f);
        }

        gameObject.SetActive(true);

        for (int i = 0; i < spineSequences.Length; i++)
        {
            if (i > 0)
                skeleton.AnimationState.SetAnimation(spineSequences[i].track, spineSequences[i].animationName,
                    spineSequences[i].loop);

            //      Debug.Log("name"+ spineSequences[i].animationName + spineSequences[i].loop);
            if (spineSequences[i].wait)
            {
                float animTime = skeleton.AnimationState.GetCurrent(spineSequences[i].track).Animation.Duration;
                //      Debug.Log("wait" + animTime);
                yield return new WaitForSeconds(animTime);
            }
            if (spineSequences[i].delayAfter > 0 )
                yield return new WaitForSeconds(spineSequences[i].delayAfter);
        }
    }

#else
    protected override void LoadComponent()
    {
        
    }

    protected override void Trigger(bool val)
    {
    }
#endif
}