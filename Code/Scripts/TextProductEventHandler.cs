using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;



public class TextProductEventHandler : BaseFrameworkView
{
    [SerializeField] private BaseRxAnimator[] anims;
    [IndexAsEnum(BuiltInEnumType.ProductIndexEventEnum)]
    [SerializeField] private int activeOn;
    [SerializeField] private IndexBindingScriptBase[] parameterBinds;
    [SerializeField] private IndexBindingScriptBase[] valueBinds;
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private int soundOnEvent;

    protected override void Register()
    {
        base.Register();
        SubjectController.ProductEvent.Subscribe(data =>
        {
            if (data.index == activeOn)
            {
                parameterBinds.OnChanged(data.parameter);
                valueBinds.OnChanged(data.amount);
                foreach (var baseRxAnimator in anims)
                {
                    baseRxAnimator.Animator_In();
                }
                soundOnEvent.PlaySound();
            }
        });
    }
}
