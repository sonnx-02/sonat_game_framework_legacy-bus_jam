using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionCheckShow : BaseBaseActionScript
{
    [SerializeField] private CheckShowOrder[] showOrders;
    [SerializeField] private bool whenNoPopup;
    private enum CheckShowOrder
    {
        Navigation,
        PopupPack,
        Quest,
    }
    
    protected override void DoAction()
    {
        base.DoAction();
        if(whenNoPopup && ScreenRoot.popupManager.AnyPopupShow)
            return;
        foreach (var checkShowOrder in showOrders)
        {
            switch (checkShowOrder)
            {
                case CheckShowOrder.Navigation:
                    if(ScreenRoot.CurrentScreen.CheckShowPopup())
                        return;
                    break;
                case CheckShowOrder.PopupPack:
                    if(ScreenRoot.CurrentScreen.CheckShowPopup())
                        return;
                    break;
                case CheckShowOrder.Quest:
                    if(ScreenRoot.CurrentScreen.CheckShowQuest())
                        return;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
