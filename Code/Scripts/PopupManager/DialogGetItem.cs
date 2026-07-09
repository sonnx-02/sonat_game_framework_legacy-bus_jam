//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//
///// <summary>
///// Dialog to get an item, you can buy by price or you can watch ads to take it as free
///// </summary>
//public class DialogGetItem : BaseDialog
//{
//    [SerializeField] private IndexBindingScript[] bindingItemIndex;
//    [SerializeField] private StandaloneAdsButtonView adsRewardButton;
//    [SerializeField] private ShopItemViewToSetup shopItemView;
//
//    protected override void Register()
//    {
//        base.Register();
//
//        if (adsRewardButton != null)
//        {
//            var onRewardAction = new BaseAction()
//            {
//                Action = RewardHandler
//            };
//            adsRewardButton.SetAction(onRewardAction);
//        }
//    }
//
//    private void RewardHandler()
//    {
//        switch (shopItemView.rewardingWaysWhenClickBuy)
//        {
//            case ShopItemRewardingType.InstantExchange:
//                PlayerData.Claim(ShopItem.GetProducts());
//                break;
//            case ShopItemRewardingType.BuyWithReturnDialog:
//                var claimed = PlayerData.Claim(ShopItem.GetProducts());
//                if (claimed.Count == 1)
//                    ScreenRoot.ShowRewardClaimed(claimed[0]);
//                break;
//            default:
//                throw new ArgumentOutOfRangeException();
//        }
//    }
//
//
//    private ShopItem ShopItem { get; set; }
//    private bool Bonus { get; set; }
//
//    public void SetData(ShopItem data,bool bonus, ShopItemRewardingType rewardingType)
//    {
//        ShopItem = data;
//        Bonus = bonus;
//        if (adsRewardButton != null)
//            adsRewardButton.gameObject.SetActive(data.rewardAdsAsPrice);
//        bindingItemIndex.OnChanged(data.price.index);
//        shopItemView.Setup(data, rewardingType, () =>
//        {
//            Close();
////            switch (shopItemView.ShopItemRewardingWays)
////            {
////                case ShopItemRewardingType.InstantExchange:
////                    Close();
////                    break;
////                case ShopItemRewardingType.BuyWithReturnDialog:
////                    Close();
////
////                    break;
////                default:
////                    throw new ArgumentOutOfRangeException();
////            }
//        });
//    }
//}