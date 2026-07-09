using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShowPopup : BaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.InScreenPopupEnum)] [SerializeField]
    private int popupEnum;
    [SerializeField] private int popupLevel;
    [SerializeField] protected bool showPopupNext;
    [SerializeField] protected WhenPopupClose whenPopupClose;
    [SerializeField] protected CustomParameter parameter;
    [SerializeField] protected BaseBaseActionScript[] beforeInvoke;
    
    protected override void DoAction()
    {
        beforeInvoke.StartAction();
        base.DoAction();
        int toShow = popupEnum;
        if (!ScreenRoot.popupManager.Exist(toShow) && toShow != (int)InScreenPopupEnum.Close)
            toShow = (int) InScreenPopupEnum.ComingSoon;
        var popup = showPopupNext
            ? ScreenRoot.popupManager.ShowPopupNext(toShow, whenPopupClose, parameter)
            : ScreenRoot.popupManager.ShowPopup(toShow, popupLevel, whenPopupClose, parameter);
    }
}
