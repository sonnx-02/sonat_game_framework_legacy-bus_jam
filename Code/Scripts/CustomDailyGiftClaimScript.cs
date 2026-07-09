using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDailyGiftClaimScript : CustomDailyPropertyEnabler
{
    [SerializeField] private Button takeButton;
    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag, 0)] [SerializeField]
    private int rewardTypeFlag = 1;
    [SerializeField] private CustomClaimScript customClaimScript;
    [SerializeField]  protected TradeLog tradeLog; // for placement log firebase stupid admob revenue
   
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        takeButton.onClick.AddListener(() => TakeReward(1));
    }

    private void TakeReward(int multiple = 1)
    {
        var rewards = Kernel.GetDatabase<GameSetting>().GetCustomDailyGift(key);
        var trade = new Trade<Product, Product>(Product.CustomQuantity((int)BaseCustomQuantity.DailyGift), rewards.rewards);
        var input = new ShowRewardInput(trade, rewardTypeFlag,false);
        if (rewardTypeFlag != (int) RewardTypeFlag.CustomClaimScript)
        {
            RootView.rootView.screenRoot.ShowReward(input);
            SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
            SubjectController.OnAdsRewarded.OnNext(key);
        }
        else
        {
            customClaimScript.Handler(input);
            SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
            SubjectController.OnAdsRewarded.OnNext(key);
        }
        Property.BoolValue = true;
        OnChange();
        DailyData.Save();
    }

    private void Reset()
    {
        takeButton = GetComponent<Button>();
    }
    
}
