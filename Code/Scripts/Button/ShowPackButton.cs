using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPackButton : BaseButton
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int key;

    protected override void OnClick()
    {
        base.OnClick();
        ScreenRoot.popupManager.Resolve<PopupPack>().ShowPack(key);
    }
}
