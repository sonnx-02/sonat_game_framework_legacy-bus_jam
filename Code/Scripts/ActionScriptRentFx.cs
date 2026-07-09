using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionScriptRentFx : BaseActionScript
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.EffectEnum)]
    private int effect;

    [SerializeField] private float delay;
    [SerializeField] private int times = 1;
    [SerializeField] private float interval = 0.1f;

    protected override void DoAction()
    {
        base.DoAction();
        if (effect != 0)
            delay.Timer(RealAction);
    }

    private void RealAction()
    {
        if (times > 1)
            Observable.Interval(TimeSpan.FromSeconds(interval))
                .Take(times)
                .Subscribe(data => RentFx());
        else
            RentFx();
    }

    private void RentFx()
    {
        var rent = PoolManager.RentFx(effect,PoolManager.uiFxParent,transform.position);
        rent.transform.localEulerAngles = Vector3.zero;
        rent.transform.position = transform.position;
        rent.transform.localPosition = rent.transform.localPosition.RemoveZ();
    }

    
}