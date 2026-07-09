using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

[System.Serializable]
public struct EffectPoolDescriptor
{
    [IndexAsEnum(BuiltInEnumType.EffectEnum)]
    public int key;
    public EffectView prefab;
    public int prePool;
    public int maxSize;
}

public class EffectView : ParameterOnRentPoolItem
{
//    [IndexAsEnum(BuiltInEnumType.EffectEnum)]
//    [SerializeField] protected int index;
    
    [SerializeField] private float destroyAfter = 1f;
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private int soundEnum;
    [SerializeField] private float playSoundAfter;
    [SerializeField] private BaseRxAnimator[] animationScripts;
    [FormerlySerializedAs("actions")] [SerializeField] 
    private IndexBindingScriptBase[] indexBindings;
    [FormerlySerializedAs("actions2")] 
    [SerializeField] private IndexBindingScriptBase[] indexBindings2;
    [SerializeField] private RxAnimatorActivate[] delayActives;
   
    [FormerlySerializedAs("timeVibrateMilliseconds")] 
    [SerializeField] private long timeVibrate;
    [SerializeField] private long timeVibrateAndroid;

    protected void OnEnable()
    {
        if(Kernel.IsReady())
            AudioManager.Play(soundEnum, playSoundAfter);
    }

    [MyButtonInt(nameof(TestRentSetIndex))]
    public int test;
    public void TestRentSetIndex(int i)
    {
        gameObject.SetActive(true);
        OnRent();
        SetIndex(i);
    }
    
    [MyButton(nameof(OnRent))] public int testOnRent;
    public override void OnRent()
    {
        base.OnRent();
        #if UNITY_ANDROID
        DuongVibration.Vibration(timeVibrateAndroid);
        #else
        DuongVibration.Vibration(timeVibrate);
#endif
        foreach (var delayActiveScript in delayActives)
        {
            delayActiveScript.gameObject.SetActive(false);
            var delay = delayActiveScript.RandomDelay;
            if (delay > 0)
                delay.Timer(() => delayActiveScript.gameObject.SetActive(true));
            else
                delayActiveScript.gameObject.SetActive(true);
        }
        
        if (destroyAfter > 0)
            Invoke(nameof(Return), destroyAfter);

        foreach (var script in animationScripts)
            script.Animator_InThenOut();
    }

    public void BindToTransform(Transform tf)
    {
        Observable.EveryUpdate().TakeUntilDisable(this)
            .Subscribe(data => { transform.position = tf.position; });
    }

    public override void SetParams(Point3Float para)
    {
        
    }

    public override void Return()
    {
        base.Return();
        foreach (var delayActiveScript in delayActives)
            delayActiveScript.gameObject.SetActive(false);
    }


    [SerializeField] [DisplayAsDisable]
    private int lastIndex = -1;
    public void SetIndex(int index)
    {
        indexBindings.OnChanged(index);
        lastIndex = index;
    }
    
    
    public void SetIndex2(int index)
    {
        indexBindings2.OnChanged(index);
    }

    public int GetIndex()
    {
        return lastIndex;
    }
}
