using System;
using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;

public class AdsItemButtonForShopItemView : BaseAdsRewardButton
{
    [SerializeField] private RewardView[] rewardViews;

    private Product _reward;

    private Action _onRewarded;
    public void SetOnReward(Action onAdsRewarded,IEnumerable<Product> product)
    {
        _onRewarded = onAdsRewarded;
        foreach (var product1 in product)
        {
            _reward = product1;
            foreach (var rewardView in rewardViews)
                rewardView.SetData(product1);
            break;
        }
    }


    protected override void RewardedHandler()
    {
        base.RewardedHandler();
        _onRewarded.Invoke();
    }
}
