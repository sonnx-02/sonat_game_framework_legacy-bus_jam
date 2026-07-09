using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShowPopupShopItem : BaseActionScript
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    private int shopItemKey;
    [SerializeField] private bool showNext;
    [SerializeField] private int popupLevel;
    
    protected override void DoAction()
    {
        base.DoAction();
        if (!showNext)
            ScreenRoot.popupManager.ShowPopup<PopupShopItem>(popupLevel).SetShopItem(shopItemKey);
        else
            ScreenRoot.popupManager.ShowPopupNext<PopupShopItem>().SetShopItem(shopItemKey);
    }

    
}
