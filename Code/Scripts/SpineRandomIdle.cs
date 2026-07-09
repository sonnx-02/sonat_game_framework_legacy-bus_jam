//#define USE_SPINE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if USE_SPINE
using Spine.Unity;
#endif
public class SpineRandomIdle : MonoBehaviour
{
    [SerializeField] private IntString[] randoms;
    [SerializeField] private bool loop = true;
    
#if USE_SPINE
    [SerializeField] private SkeletonGraphic skeleton;
 
  
    private void OnEnable()
    {
        var rand = randoms.TakeByWeight(randoms.Select(x => x.value).ToList());
        skeleton.AnimationState.ClearTrack(0);
        skeleton.Skeleton.SetToSetupPose();
        if (!string.IsNullOrEmpty(rand.value2)) {
            skeleton.AnimationState.SetAnimation(0, rand.value2, loop);
            skeleton.Update(0f);
        }
    }
#endif

}

