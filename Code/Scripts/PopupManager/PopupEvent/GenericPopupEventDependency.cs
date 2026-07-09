using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class GenericPopupEventDependency : BasePopupDependency
{
    [IndexAsEnum(nameof(EventDayType))]
    [SerializeField] private int eventType;
    private EventDayPlayerData Data => StaticDataController.EventDays[eventType];


    [SerializeField] private IndexBindingScriptBase[] bindEvent;
    [SerializeField] private PoolCollection<RewardViewExtended> rewards;
    [SerializeField] private CountDownView countDownView;
    [SerializeField] private ProgressIntPrefabBar claimProgressBar; // progress claim only
    [SerializeField] private ProgressIntArrayScript arrayProgressBar; // progress of collection
    [SerializeField] private IndexBindingScriptBase[] focusScript; // progress of collection
    [SerializeField] private RewardView currentReward; // progress of collection
    [SerializeField] private RewardView lastReward; // progress of collection
    [SerializeField] private BindingToStringTextChange eventName; // progress of collection
    [SerializeField] private EarningRewardSetting earningSetting;
    [MyButtonInt(nameof(TestFill))] public int testFill;



    private void TestFill(int add)
    {
        Data.progress += add;
        Data.Save();
        UpdateProgress();
    }

    protected override void Register()
    {
        base.Register();
        rewards.Init();
        rewards.ViewFirst(0);
        LoadDataAndRegisterView();
    }


    private void LoadDataAndRegisterView()
    {
        var gc = RootView.rootView.gameController;
        StaticDataController.LoadEventPlayerData(eventType,gc.Level,eventKey => RootView.rootView.gameController.OnLoadedEventDay(eventType,eventKey));

        if (Data != null)
        {
            claimProgressBar.SetTarget(Data.rewards.Length);
            rewards.ReturnAll();
            for (var i = 0; i < Data.rewards.Length; i++)
            {
                var view = rewards.pools[0].Rent();
                view.SetIndex(i);
                view.SetData(Data.rewards[i]);
                view.Button.onClick.RemoveAllListeners();
                view.Button.onClick.AddListener(() => TakeReward(view.Index));
            }

            arrayProgressBar.SetTargets(Data.rewards.Select(x => x.parameter).ToArray());
            UpdateProgress();

            if (countDownView != null)
            {
                var addDays = Data.activeDays - (DuongExtensions.GetEpochDayOfYear() - Data.startEpochDate);
                if (addDays < 0)
                    Data.activeDays = 1;
                countDownView.StartCountDown(DateTime.Today.AddDays(addDays), () => { });
            }

            if (lastReward != null)
                lastReward.SetData(Data.rewards[Data.rewards.Length - 1]);
            eventName.OnChanged(Data.eventName);
            claimProgressBar.transform.SetAsLastSibling();
        }
    }

    private void TakeReward(int i)
    {
        if (!Data.claimed[i])
        {
            Data.claimed[i] = true;
            Data.Save();

            var trade = new Trade<Product, Product>(Product.Free, Data.rewards[i]);
            var input = new ShowRewardInput(trade, earningSetting.rewardTypeFlag)
            {
                closeCurrentPopup = earningSetting.closeCurrentPopupOnRewarded
            };
            RootView.rootView.screenRoot.ShowReward(input);
        }

        UpdateProgress();
        SubjectController.CustomUpdatedEvent.OnNext(CustomUpdateEvent.Event);
    }


    private void UpdateProgress()
    {
        var progressIndex = Data.GetCurrentTargetIndex(out ProgressState state);
        arrayProgressBar.SetClaimed(Data.claimed);
        focusScript.OnChanged(progressIndex);
        claimProgressBar.OnChanged(progressIndex);
        arrayProgressBar.OnChanged(Data.progress);
        currentReward.SetData(rewards.pools[0].ActiveItems[arrayProgressBar.CurrentIndex].Product);

        for (var i = 0; i < rewards.pools[0].ActiveItems.Count; i++)
        {
            rewards.pools[0].ActiveItems[i].SetClaimed(Data.claimed[i]);
            rewards.pools[0].ActiveItems[i]
                .SetReady(progressIndex > i || (state & ProgressState.Ready) != 0 && i == progressIndex);
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        if (Data == null || Data.EventKey < 0)
            bindEvent.OnChanged(-1);
        else
            bindEvent.OnChanged(Data.EventKey);

        if (Data != null && countDownView != null)
        {
            var addDays = Data.activeDays - (DuongExtensions.GetEpochDayOfYear() - Data.startEpochDate);
            if (addDays < 0)
                Data.activeDays = 1;
            countDownView.StartCountDown(DateTime.Today.AddDays(addDays), () => { });
        }

        UpdateProgress();
    }
}