using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupDailyReward : BasePopup
{
    [SerializeField] private InterstitialLogDataView logShowInterstitial;
    [SerializeField] private DailyGiftItemView[] items;
    [SerializeField] private ToggleScript[] onTaken;
    [SerializeField] private Button takeButton;
    [SerializeField] private RewardView rewardView;
    [SerializeField] private RewardView rewardViewDouble;
    [SerializeField] protected bool rewardInBox;

    private Product[] _rewards;

    [SerializeField] private float delayToClose;

    protected override void Register()
    {
        base.Register();

        _rewards = Kernel.GetDatabase<GameSetting>().dailyCheckInRewards;
        for (var i = 0; i < items.Length; i++)
            items[i].gameObject.SetActive(false);

        for (var i = 0; i < _rewards.Length; i++)
        {
            items[i].gameObject.SetActive(true);
            items[i].Setup(_rewards[i], i, _rewards.Length - 1);

            var btn = items[i].button;
            if (btn != null)
            {
                var i1 = i;
                btn.onClick.AddListener(() =>
                {
                    BaseSoundEnum.ButtonClick.Play();
                    if (PlayerData.dailyGiftChecked) return;
                    if (items[i1].IsCurrent)
                    {
                        TakeReward();
                        SubjectController.NotifyEvent.OnNext(NotifyEnum.DailyGift);
                    }
                });
            }
        }

        SubjectController.ProductSystemActionEvent
            .Where(x => gameObject.activeInHierarchy &&
                        ((ProductSystemAction) x.index) == ProductSystemAction.DoubleReward)
            .Subscribe(data =>
            {
                TakeReward();
                SubjectController.NotifyEvent.OnNext(NotifyEnum.DailyGift);
            });

        takeButton.onClick.AddListener(() =>
        {
            TakeReward();
            SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
        });
    }
    

    public override void LogCustom()
    {
        base.LogCustom();
        PlayerData.dailyGiftChecked = false;
        DailyData.dailyGiftChecked = false;
    }

    private void AfterClaim()
    {
        PlayerData.dailyGiftChecked = true;
        DailyData.dailyGiftChecked = true;

        PlayerData.Save();
        DailyData.Save();
        UpdateStatus();


        Kernel.Resolve<FireBaseController>().LogEvent("daily_reward",
            new[]
            {
//                new LogParameter("diamond", reward.amount * multiple),
                new LogParameter("day", PlayerData.dailyGiftProgress)
            });
    }

    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    private int rewardTypeFlag = 1;

    [SerializeField] private float multi = 2f;


    private void TakeReward()
    {
        if (PlayerData.dailyGiftChecked) return;
        var setting = Kernel.GetDatabase<GameSetting>();
        var index = Mathf.Min(PlayerData.dailyGiftProgress, setting.dailyCheckInRewards.Length - 1);
        var reward = setting.dailyCheckInRewards[index].Clone();
        var price = new Product(Quantity.Achievement, 1, 0, 0, 0);
        var trade = new Trade<Product, Product>(price, reward);
        var input = new ShowRewardInput(trade, rewardTypeFlag, 1, trade.Product.parameter, multi);
        //parameter = 1, // 0 = no tween box, 1 2 3 = tween box
        // parameter2 = product.parameter, // index of box
        if (logShowInterstitial != null) input.logInterstitial = logShowInterstitial.data;
        RootView.rootView.screenRoot.ShowReward(input, AfterClaim);
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        CheckRegister();
        UpdateStatus();
    }


    [SerializeField] private RectTransform border;
    private void UpdateStatus()
    {
        if(border != null)
            border.gameObject.SetActive(false);

        var current = _rewards.Length - 1;
        for (var i = 0; i < items.Length; i++)
        {
            var hasBorder = PlayerData.dailyGiftProgress == i && !DailyData.dailyGiftChecked
                            || PlayerData.dailyGiftProgress + 1 == i && DailyData.dailyGiftChecked
                            || PlayerData.dailyGiftProgress >= _rewards.Length - 1 && i == _rewards.Length - 1;
            items[i].SetBorder(hasBorder);
            if (items[i].UpdateStatus(_rewards.Length))
                current = i;
            if (hasBorder && border != null)
            {
//                border.SetParent(items[i].transform);
                border.gameObject.SetActive(true);
                border.SetParentAndExpand(items[i].transform,false,false);
            }
        }


        if (rewardView != null)
            rewardView.SetData(_rewards[current]);
        if (rewardViewDouble != null)
            rewardViewDouble.SetData(_rewards[current].Multiple(2));

        onTaken.OnChanged(DailyData.dailyGiftChecked);
    }
}