using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupShopItemShowButton : BaseButton
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    private int shopItemKey;

    [SerializeField] private int popupShopItem;
    protected override void OnClick()
    {
        base.OnClick();
        if(popupShopItem == 0)
            ScreenRoot.popupManager.ShowPopup<PopupShopItem>().SetShopItem(shopItemKey);
        else if(popupShopItem == 1)
            ScreenRoot.popupManager.ShowPopup<PopupShopItem2>().SetShopItem(shopItemKey);
        else if(popupShopItem == 2)
            ScreenRoot.popupManager.ShowPopup<PopupShopItem3>().SetShopItem(shopItemKey);
        else if(popupShopItem == 3)
            ScreenRoot.popupManager.ShowPopup<PopupShopItem4>().SetShopItem(shopItemKey);
        else
            ScreenRoot.popupManager.ShowPopup<PopupShopItem>().SetShopItem(shopItemKey);
    }

    protected override string IconShortCutName => "ShowPopupItem" + BuiltInEnumType.ShopItemKey.GetName(shopItemKey);
}
