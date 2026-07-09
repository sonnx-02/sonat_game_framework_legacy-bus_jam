using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


public abstract class BaseFrameworkViewCheck : BaseFrameworkView
{
    [SerializeField] private bool checkOnFocus;
    
    [SerializeField] private GameMainEvent[] checkOns;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SubjectController.OnGameMainEvent.Subscribe(data =>
        {
            foreach (var gameMainEvent in checkOns)
            {
                if (gameMainEvent == data)
                {
                    Check();
                    break;
                }
            }
        });
    }

    
    public abstract void Check();
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (checkOnFocus && hasFocus)
        {
            Check();
        }
    }
}