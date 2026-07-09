using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public abstract class BaseShowPopupButton : BaseButton
{
    [SerializeField] protected bool showPopupNext;
    [SerializeField] protected int popupLevel;
    [SerializeField] protected WhenPopupClose whenPopupClose;
    [SerializeField] protected CustomParameter parameter;


    protected override void OnClick()
    {
        base.OnClick();
        var popup = ShowPopup();
        if (whenPopupClose == WhenPopupClose.OpenLastDialog)
            ScreenRoot.dialogController.CloseAll();
    }



    protected abstract BasePopup ShowPopup();
}

public class ShowPopupButton : BaseShowPopupButton
{
    [IndexAsEnum(BuiltInEnumType.InScreenPopupEnum)] [SerializeField]
    private int popupEnum;

    protected override string IconShortCutName => GetType().Name+BuiltInEnumType.InScreenPopupEnum.GetName(popupEnum);

    public void Invoke()
    {
        OnClick();
    }

    protected override BasePopup ShowPopup()
    {
        int toShow = popupEnum;
        if (!ScreenRoot.popupManager.Exist(toShow) && toShow != (int)InScreenPopupEnum.Close)
            toShow = (int) InScreenPopupEnum.ComingSoon;

        var popup = showPopupNext
            ? ScreenRoot.popupManager.ShowPopupNext(toShow, whenPopupClose, parameter)
            : ScreenRoot.popupManager.ShowPopup(toShow, popupLevel, whenPopupClose, parameter);
        if (popup is PopupRateUs)
            new SonatLogShowRate()
            {
                open_by = SonatLogShowRate.ShowRateOpenBy.user,
                action = SonatLogShowRate.ShowRateAction.open
            }.Post();
        return popup;
    }

    protected override string GetName()
    {
        return $"btn_show_popup_{((InScreenPopupEnum) popupEnum).ToString()}";
    }
}