using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TriggerOnGlobalGameEffect : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.GlobalCallBackEnum)]
    public int key;

    [SerializeField] private BaseRxAnimator[] animators;
    [SerializeField] private BaseBaseActionScript[] actions;
    [SerializeField] private ToggleScript[] toggles;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SubjectController.OnPointParameterCallBack
            .Where(x => x.enumType == BuiltInEnumType.GlobalCallBackEnum && x.enumValue == key &&
                        gameObject.activeInHierarchy)
            .Subscribe(data => { Trigger(); }).DisposeWith(this);
    }

    public override void LogCustom()
    {
        base.LogCustom();
        SubjectController.OnPointParameterCallBack.OnNext(new PointParameter(BuiltInEnumType.GlobalCallBackEnum, key));
    }

    private void Trigger()
    {
        foreach (var baseRxAnimator in animators)
            baseRxAnimator.Animator_In();
        toggles.OnChanged(true);
        foreach (var baseActionScript in actions)
            baseActionScript.StartAction();
    }
}