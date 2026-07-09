
using System.Collections;
using System.Collections.Generic;
#if !USE_SPINE
public class IndexBindingSpineSourceChange : IndexBindingScript
{

}
#else
using Spine;
using Spine.Unity;
using UnityEngine;

public class IndexBindingSpineSourceChange : IndexBindingScript
{
    [SerializeField] private  SkeletonGraphic skeleton;
    [SerializeField] private  SkeletonDataAsset[] assets;
    [SerializeField] private BaseRxAnimator[] animOnChanged;
    protected override void OnValueChanged(int computedValue)
    {
        CheckRegister();
        base.OnValueChanged(computedValue);
        skeleton.skeletonDataAsset = assets[computedValue];
        skeleton.Initialize(false);
        skeleton.Skeleton.SetSlotsToSetupPose();
        skeleton.LateUpdate();
        foreach (var baseRxAnimator in animOnChanged)
            baseRxAnimator.Animator_In();
    }

    protected override bool IsNoComponent()
    {
        return skeleton == null;
    }
    

    protected override void LoadComponent()
    {
        skeleton = GetComponent<SkeletonGraphic>();
    }
}
#endif
