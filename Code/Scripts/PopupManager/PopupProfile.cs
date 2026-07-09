using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PopupProfile : BasePopup
{
    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        SubjectController.GlobalError.Where(x => (BaseGlobalError) x == BaseGlobalError.PlayFabNameTaken)
            .TakeUntilDisable(this)
            .Subscribe(data =>
            {
                if(!ScreenRoot.dialogController.AnyDialogShow && ScreenRoot.popupManager.GetCurrentPopup() == this)
                    ScreenRoot.dialogController.ShowMessage($"Name not available");
            });
    }
}
