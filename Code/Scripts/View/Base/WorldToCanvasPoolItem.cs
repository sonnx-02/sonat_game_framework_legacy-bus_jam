using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable PossibleNullReferenceException


public class WorldToCanvasPoolItem : PoolItem
{
    [SerializeField] private ParameterHandlerScript[] handlers;
    [SerializeField] private bool animateOnRent;
    [SerializeField] private BaseRxAnimator[] anims;
    [SerializeField] private StartActionWhen startActionWhen = StartActionWhen.AfterTime;
    [SerializeField] private float delayAction;

    private enum StartActionWhen
    {
        OnRent,
        AfterTime,
        WhenFinishAnim
    }
    
    public virtual void SetPosFromWorld(Vector3 worldPosition)
    {
        (transform as RectTransform).anchoredPosition = FrameworkExtensions.WorldToPositionInCanvas(worldPosition);
    }

    public override void OnRent()
    {
        base.OnRent();
        if (startActionWhen == StartActionWhen.OnRent)
            Action();
        if (startActionWhen == StartActionWhen.AfterTime)
            delayAction.Timer(Action);
        if(animateOnRent)
            StartAnimating();
    }

    private void Action()
    {
        foreach (var parameterHandlerScript in handlers)
            parameterHandlerScript.StartAction();
    }

    public void StartAnimating()
    {
        if (startActionWhen == StartActionWhen.WhenFinishAnim)
            anims.Select(x => x.IO_AnimatorIn()).WhenAll().Subscribe(_ =>
            {
                Action();
            });
        else
            foreach (var baseRxAnimator in anims)
                baseRxAnimator.Animator_In();
    }

    public override void SetParameter(PointParameter param)
    {
        base.SetParameter(param);
        foreach (var parameterHandlerScript in handlers)
            parameterHandlerScript.SetParameter(param);
    }
}

