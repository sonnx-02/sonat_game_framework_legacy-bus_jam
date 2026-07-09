using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonShowNavigateDialog : BaseButton
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.NavigateDialogEnum)]
    private int navigate;

    protected override string GetName()
    {
        return "btn_show_navigate_" + BuiltInEnumType.NavigateDialogEnum.GetName(navigate);
    }

    protected override void OnClick()
    {
        base.OnClick();
        ScreenRoot.dialogController.Resolve<DialogNavigate>().ShowNavigate(navigate);
    }
}