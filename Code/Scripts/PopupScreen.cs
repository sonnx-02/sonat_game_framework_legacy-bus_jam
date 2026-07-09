using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupScreen : BasePopup
{
    [IndexAsEnum(BuiltInEnumType.ScreenEnum)]
    [SerializeField] private int testScreen;

    public override void LogCustom()
    {
        base.LogCustom();
        SetScreen(testScreen);
        Show();
    }

    [SerializeField] private BaseBaseActionScript[] onShows;
    [SerializeField] private BaseBaseActionScript[] onHides;

    [SerializeField] private RectTransform rectTransform;
    private BaseScreen _sc;
    public void SetScreen(int screen)
    {
        _sc = ScreenRoot.Resolve(screen);
        if (_sc.IgnoreRelease)
        {
            Debug.LogError("invalid");
            Debug.Break();
        }
        ScreenRoot.HandlerScreenShow(_sc);
        _sc.OnShowHandler();
        _sc.transform.SetParentAndExpand(transform, true, true);
    }

    [SerializeField] private bool turnOffMainScreen;
    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        if(turnOffMainScreen)
            ScreenRoot.CurrentScreen.TurnCanvas(false);
        ScreenRoot.HandlerScreenShow(_sc);
        foreach (var baseBaseActionScript in onShows)
            baseBaseActionScript.StartAction();
    }

    protected override void CloseHandler()
    {
        _sc.OnHideHandler();
        if(turnOffMainScreen)
            ScreenRoot.CurrentScreen.TurnCanvas(true);
        ScreenRoot.HandlerScreenShow(ScreenRoot.CurrentScreen);
        foreach (var baseBaseActionScript in onHides)
            baseBaseActionScript.StartAction();
        ScreenRoot.CurrentScreen.OnShowReInvoke();
        base.CloseHandler();
    }
}