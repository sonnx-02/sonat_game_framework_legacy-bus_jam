using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPopupButtonByName : BaseShowPopupButton
{
    [SerializeField] private string popupClassName;
    protected override string GetName()
    {
        return  $"btn_show_popup_{popupClassName}";
    }

    protected override BasePopup ShowPopup()
    {
        return  ScreenRoot.popupManager.ShowPopup(popupClassName, popupLevel,whenPopupClose,parameter);
    }
}


