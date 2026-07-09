using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptShowDialogNavigateToShop : BaseActionScriptIndexPropertyChange
{
    [SerializeField] private string message;

    protected override void DoAction()
    {
        base.DoAction();
        ScreenRoot.dialogController.ShowMessage(message, () =>
        {
            base.DoAction();
            ScreenRoot.popupManager.ShowPopup<PopupShop>();
        });
    }

    
}
