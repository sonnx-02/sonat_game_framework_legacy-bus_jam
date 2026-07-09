using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CloseDialogButton : BaseButton
{
    protected override string GetName()
    {
        return "close_dialog";
    }

    protected override void OnClick()
    {
        base.OnClick();
        ScreenRoot.dialogController.CloseCurrentAsObservable(true).Subscribe();
    }
}
