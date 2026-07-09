using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PopupRateUs : BasePopup
{
    public IntReactiveProperty star;
    [SerializeField] private int starOnEnable = 5;

    private void OnEnable()
    {
        star.Value = starOnEnable;
    }
    

    private Action _overrideActionWhenClose;

    protected override void HandlerCloseClick()
    {
        if(_overrideActionWhenClose != null)
            _overrideActionWhenClose.Invoke();
        else
            base.HandlerCloseClick();
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        _overrideActionWhenClose = null;
    }
    
    public void OverrideClose(Action action)
    {
        _overrideActionWhenClose = action;
    }

    protected override void CloseHandler()
    {
        base.CloseHandler();
        _overrideActionWhenClose = null;
    }
}