using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !USE_SPINE
public class IndexBindingSpineSkinChange2 : IndexBindingScript
{
        [SerializeField] private  string[] skinNames;
        [SerializeField] private bool random;
}
#else
using Spine;
using Spine.Unity;

public class IndexBindingSpineSkinChange2 : IndexBindingScript
{
    [SerializeField] private  SkeletonGraphic skeleton;
    [SerializeField] private  string[] skinNames;

    [SerializeField] private bool random;


    protected override bool IsNoComponent()
    {
        return skeleton == null;
    }
    

    protected override void LoadComponent()
    {
        skeleton = GetComponent<SkeletonGraphic>();
    }

    private bool _initialized2;
    protected override void Register()
    {
        base.Register();
        if(skeleton == null)
            LoadComponent();
        skeleton.Initialize(false);
        _initialized2 = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_initialized2 && random)
            OnValueChanged(UnityEngine.Random.Range(0,skinNames.Length));
    }

    protected override void OnValueChanged(int computedValue)
    {
        CheckRegister();
        base.OnValueChanged(computedValue);
        skeleton.Skeleton.SetSkin(skinNames[computedValue]);
        skeleton.Skeleton.SetSlotsToSetupPose();
        skeleton.LateUpdate();
    }
}
#endif