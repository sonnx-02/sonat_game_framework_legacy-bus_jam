using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SubscriptionToggle : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.SubscriptionKey)]
    public int key;

    [SerializeField] private ToggleScript[] bindOnActives;
    [SerializeField] private ToggleScript[] bindReadyTakes;
    [SerializeField] private IndexBindingScriptBase[] bindDayActives;
    [SerializeField] private Button[] btnTakeReward;
    [SerializeField] private bool autoTakeReward;

//    [SerializeField] private ShopItemRewardingType rewardingType = BuyWithReturnDialog;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        foreach (var button in btnTakeReward)
        {
            button.onClick.AddListener(TakeReward);
        }
    }
    
    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    private int rewardTypeFlag = 1;


    void TakeReward()
    {
        if (IsSubscriptionReadyTake(key))
        {
            var reward = TakeSubscriptionReward(key);
            var trade = new Trade<Product, Product>(Product.Free,reward);
            var input = new ShowRewardInput(trade, rewardTypeFlag);
            RootView.rootView.screenRoot.ShowReward(input);
        }
    }

    private void OnEnable()
    {
        SubjectController.SubscriptionActivated.Where(x => x == key)
            .TakeUntilDisable(gameObject)
            .Subscribe(data =>
            {
                UpdateView();
                if(autoTakeReward && IsSubscriptionReadyTake(key))
                    TakeReward();
            });

        UpdateView();
    }

    private void UpdateView()
    {
        bool active = IsSubscriptionActive(key);
        bindOnActives.OnChanged(active);
        bool take = IsSubscriptionReadyTake(key);
        bindReadyTakes.OnChanged(take);
        if (active)
        {
            bindDayActives.OnChanged(PlayerData.subscription[key]);
        }
        else
        {
            bindReadyTakes.OnChanged(false);
            bindDayActives.OnChanged(0);
        }
    }
}
