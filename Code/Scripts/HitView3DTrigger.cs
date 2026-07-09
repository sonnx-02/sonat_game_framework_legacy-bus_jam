using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

public class HitView3DTrigger : BaseFrameworkView
{
    [SerializeField] private BaseBaseActionScript[] scripts;
    [SerializeField] private BaseActionScriptTransform[] scriptsWithTarget;

    [SerializeField] private float delayListener;
    // for rent an effect to hit

    protected override void Register()
    {
        base.Register();
    }

    private void OnEnable()
    {
        delayListener.Timer(() =>
        {
            this.OnTriggerEnterAsObservable()
                .TakeUntilDisable(this).Subscribe(data =>
            {
                //  Debug.LogError(data.gameObject);
                foreach (var baseActionScript in scripts)
                    baseActionScript.StartAction();
                foreach (var baseActionScript in scriptsWithTarget)
                {
                    baseActionScript.SetTarget(data.gameObject.transform);
                    baseActionScript.StartAction();
                }
            });
        });
    }
}