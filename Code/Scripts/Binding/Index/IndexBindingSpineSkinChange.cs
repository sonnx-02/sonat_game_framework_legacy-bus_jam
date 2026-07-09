using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if !USE_SPINE
public class IndexBindingSpineSkinChange : IndexBindingScript
{
    
}
#else
using Spine.Unity;
public class IndexBindingSpineSkinChange : IndexBindingScript
{
    [SerializeField] private string[] skinNames;
    [SerializeField] private string activeAnim;
    [SerializeField] private string idleAnim;
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private SkeletonAnimation skeleton2;
    [SerializeField] private ActionScriptTweenPosition activeAnimToggle;

    private bool valid = false;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        skeleton = GetComponent<SkeletonGraphic>();
        skeleton2 = GetComponent<SkeletonAnimation>();
    }

    protected override void Register()
    {
        base.Register();
        if (skeleton != null)
            skeleton.Initialize(false);
        if (skeleton2 != null)
            skeleton2.Initialize(false);
        _lastSkin = defaultSkin;
    }

    public int defaultSkin;
    private int _lastSkin;
    protected override void OnValueChanged(int computedValue)
    {
        CheckRegister();

        if (_lastSkin == computedValue)
        {
            if (skeleton != null)
            {
                PlayActiveAnimUI();
            }
            return;
        }

        _lastSkin = computedValue;
        base.OnValueChanged(computedValue);

        if (skeleton != null)
        {
            skeleton.Skeleton.SetSkin(skinNames[computedValue]);
            skeleton.Skeleton.SetSlotsToSetupPose();
            skeleton.LateUpdate();
            PlayActiveAnimUI();
        }

        if (skeleton2 != null)
        {
            skeleton2.Skeleton.SetSkin(skinNames[computedValue]);
            skeleton2.Skeleton.SetSlotsToSetupPose();
            skeleton2.LateUpdate();
        }
    }

    private void PlayActiveAnimUI()
    {
        if (!string.IsNullOrEmpty(activeAnim))
        {
            if (!activeAnimToggle.valid)
            {
                PlayIdleAnimUI();
                return;
            }

            skeleton.AnimationState.SetAnimation(0, activeAnim, false);
            float animTime = skeleton.AnimationState.GetCurrent(0).Animation.Duration;

            DOVirtual.DelayedCall(animTime, () => 
            {
                PlayIdleAnimUI();
            }).SetUpdate(false);
        }
    }

    private void PlayIdleAnimUI()
    {
        if (!string.IsNullOrEmpty(idleAnim))
        {
            skeleton.AnimationState.SetAnimation(0, idleAnim, true);
        }
    }
}
#endif
