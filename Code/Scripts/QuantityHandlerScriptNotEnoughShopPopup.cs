using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct QuantityShopItemPair
{
    public Quantity quantity;
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int shopItemKey;
}

public class QuantityHandlerScriptNotEnoughShopPopup : QuantityHandlerScriptBase
{
    public QuantityShopItemPair[] dict;
    [SerializeField] private bool showNext;
    [SerializeField] private int popupLevel;
    
    public override void Handler(Quantity currency)
    {
        foreach (var quantityShopItemPair in dict)
        {
            if (quantityShopItemPair.quantity == currency)
            {
                if (!showNext)
                    ScreenRoot.popupManager.ShowPopup<PopupShopItem>(popupLevel).SetShopItem(quantityShopItemPair.shopItemKey);
                else
                    ScreenRoot.popupManager.ShowPopupNext<PopupShopItem>().SetShopItem(quantityShopItemPair.shopItemKey);
                break;
            }
        }
    }
}
