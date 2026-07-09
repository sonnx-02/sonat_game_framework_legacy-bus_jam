using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeEquipHandlerScript : BaseEquipHandlerScript
{
    [SerializeField] private TradeEquipAction tradeEquipAction;
    [SerializeField] private ShopItemView shopItemView;
    
    [System.Serializable]
    public enum TradeEquipAction
    {
        ChangerParameterAsIndex
    }
    public override void Handler(UnlockEquipItem unlockEquipItem, int unlockEquipDataEnum)
    {
        var shopItem = shopItemView.GetShopItem();
        switch (tradeEquipAction)
        {
            case TradeEquipAction.ChangerParameterAsIndex:
                shopItem.Product.parameter = unlockEquipItem.Index;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        shopItemView.ClickHandler();
    }
}
