//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UniRx;
//
//public class ShopItemViewWithStock : ShopItemView
//{
//    [IndexAsEnum(BuiltInEnumType.ShopItemKey)] [SerializeField]
//    private int getMoreItemKey;
//
//    [SerializeField] private IndexBindingScript stockText;
//    [SerializeField] private ToggleScript[] affordableBinding;
//
//
//    protected override void OnKernelLoaded()
//    {
//        base.OnKernelLoaded();
//        UpdateStock();
//        switch (ShopItem.price.quantity)
//        {
//            case Quantity.Item:
//                PlayerData.items.DataUpdated.Subscribe(data => UpdateStock());
//                break;
//        }
//    }
//
//
//    protected override void UpdateStock()
//    {
//        base.UpdateStock();
//        switch (ShopItem.price.quantity)
//        {
//            case Quantity.Item:
//                if (stockText != null)
//                    stockText.OnChanged(PlayerData.items[ShopItem.price.index]);
//                break;
//        }
//
//        var affordable = PlayerData.items[ShopItem.price.index] >= ShopItem.price.amount;
//        affordableBinding.OnChanged(affordable);
//    }
//
//    public override void NotAffordableHandler()
//    {
//        var dialog1 = ScreenRoot.dialogController.Resolve<DialogGetItem>();
//        dialog1.SetData(RootView.rootView.ShopDatabase.GetShopItem(getMoreItemKey), bonus, rewardingWaysWhenClickBuy);
//        dialog1.Show();
//    }
//}