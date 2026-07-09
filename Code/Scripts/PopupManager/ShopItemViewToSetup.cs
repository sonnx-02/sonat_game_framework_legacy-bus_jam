using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemViewToSetup : BaseShopItemView
{
    [SerializeField] private IndexBindingScript[] priceQuantityBindings;
    public void Setup(ShopItem data,int rewardType,Action whenSuccessful)
    {
        ShopItem = data;
        WhenSuccessful = whenSuccessful;
        rewardTypeFlag = rewardType;
        UpdateView();
        priceQuantityBindings.OnChanged((int)data.Price.Quantity);
    }

    private ShopItem _data;

    public override void LogCustom()
    {
        base.LogCustom();
        Setup(ShopItem);
    }

    public void Setup(ShopItem data,Action whenSuccessful = null)
    {
        ShopItem = data;
        WhenSuccessful = whenSuccessful;
        UpdateView();
        priceQuantityBindings.OnChanged((int) data.Price.Quantity);
    }
}