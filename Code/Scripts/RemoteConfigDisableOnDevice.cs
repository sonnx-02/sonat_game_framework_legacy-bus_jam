using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RemoteConfigDisableOnDevice : BaseFrameworkView
{
    [SerializeField] private string key;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();

        if (key != null)
        {
            Debug.LogError("Caution to turn off this "+gameObject,gameObject);
            Observable.Interval(TimeSpan.FromSeconds(5))
                .Take(10)
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    if(RemoteConfigHelper.HasKey(key) && RemoteConfigHelper.GetValueBoolean(key))
                        gameObject.SetActive(false);
                });
        }
     
    }
}
