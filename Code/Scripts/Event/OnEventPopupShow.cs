using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class OnEventPopupShow : BaseFrameworkView
{
    [SerializeField] private string popupName;
    [SerializeField] private BaseActionScript[] onShow;
    [SerializeField] private BaseActionScript[] onClose;
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        ScreenRoot.popupManager.onShowingPopups.ObserveAdd().Subscribe(popup =>
        {
            if (popup.Value.PopupName == popupName.ToLower())
                onShow.StartAction();
        });
        ScreenRoot.popupManager.onShowingPopups.ObserveRemove().Subscribe(popup =>
        {
            if (popup.Value.PopupName ==  popupName.ToLower())
                onClose.StartAction();
        });
    }
}
