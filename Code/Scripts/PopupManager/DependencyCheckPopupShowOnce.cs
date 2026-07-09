using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyCheckPopupShowOnce : BasePopupDependency
{
    [SerializeField] private string key;
    [SerializeField] private ValidScriptBase[] others;
    public enum ShowTimes
    {
        OnceADay,
        OnceInSession
    }

    [SerializeField] private ShowTimes showTimes;
    public override void OnShow()
    {
        base.OnShow();
        switch (showTimes)
        {
            case ShowTimes.OnceADay:
                PlayerPrefs.SetInt("showed_" + key,DateTime.Today.ToEpoch());
                break;
            case ShowTimes.OnceInSession:
                _showed = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool _showed;
    public override bool ReadyToShow()
    {
        foreach (var validScriptBase in others)
        {
            if (!validScriptBase.IsValid())
                return false;
        }
        switch (showTimes)
        {
            case ShowTimes.OnceADay:
                return PlayerPrefs.GetInt("showed_" + key) < DateTime.Today.ToEpoch();
            case ShowTimes.OnceInSession:
                return !_showed;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return !_showed;
    }
}
