using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemViewHandler_OpenPopup : BaseShopItemViewClickHandler
{
    [SerializeField] private string popupName = "PopupShopItem2";
    public override void Handler(BaseBaseShopItemView shopItemView)
    {
        var shop = ScreenRoot.popupManager.ShowPopup(popupName);
        var handler = shop.ResolveDependencyAssignFrom<PopupDependencySetShopItem>();
        handler.SetData(shopItemView);
    }
}