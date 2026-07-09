using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;



public class ShowPopupScreenButton : BaseShowPopupButton
{
    [IndexAsEnum(BuiltInEnumType.ScreenEnum)]
    [SerializeField] private int screenEnum;
    public void Invoke()
    {
        OnClick();
    }

    [SerializeField] private bool releaseCurrent = true;
    protected override BasePopup ShowPopup()
    {
        ScreenRoot.popupManager.Resolve<PopupScreen>().SetScreen(screenEnum);
        int toShow = (int)InScreenPopupEnum.PopupScreen;
        if (!ScreenRoot.popupManager.Exist(toShow) && toShow != (int)InScreenPopupEnum.Close)
            toShow = (int) InScreenPopupEnum.ComingSoon;

        var popup = showPopupNext
            ? ScreenRoot.popupManager.ShowPopupNext(toShow, whenPopupClose, parameter)
            : ScreenRoot.popupManager.ShowPopup(toShow, popupLevel, whenPopupClose, parameter);
        return popup;
    }

    protected override string GetName()
    {
        return $"btn_show_popup_{InScreenPopupEnum.PopupScreen.ToString()}";
    }
}