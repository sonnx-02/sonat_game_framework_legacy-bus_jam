using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CooldownProgressScript : BaseFrameworkView
{
    [SerializeField] private ProgressScript progressScript;
    [SerializeField] private ToggleScript[] activeWhenProgressing;

    private float _time;
    private IDisposable _cooldown;
    private Action _action;

    public bool onProgressing;


    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        StartCooldown(value, null);
    }

    public void ResetCoolDown()
    {
        _cooldown?.Dispose();
        activeWhenProgressing.OnChanged(false);
        onProgressing = false;
        progressScript.OnChanged(0);
    }

    public void StartCooldown(float time, Action afterCooldown)
    {
        _time = time;
        _action = afterCooldown;
        float t = _time;
        activeWhenProgressing.OnChanged(true);
        onProgressing = true;
        _cooldown?.Dispose();
        _cooldown = Observable.EveryUpdate()
            .TakeWhile(x => t > 0)
            .DoOnCompleted(() =>
            {
                onProgressing = false;
                activeWhenProgressing.OnChanged(false);
                _action?.Invoke();
                progressScript.OnChanged(0);
            })
            .Subscribe(data =>
            {
                t -= Time.deltaTime;
                progressScript.OnChanged(t / _time);
            });
    }

}