using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public enum NotifyEnum
{
    DailyGift = 0,
    DailyGem = 1,
    DailyQuest = 2,
    Achievement = 3,
    OnlineGift = 4,
    DailyQuestWatchAds = 5,
    AdsRewarded = 6,
    

    All = 999
}

public class NotifyChecker : BaseFrameworkView
{
    public NotifyEnum[] notifies;
    public NotifyChecker[] notifyCheckers;
    public AdsItemView[] adsRewardNotifies;
    public int[] adsItemKeys;

    public ToggleScript[] actives;

    public bool opened;
    public Button[] visitButtons;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SubjectController.NotifyEvent
            .Where(x => x == NotifyEnum.All || notifies.Contains(x))
            .Subscribe(notify => { CheckNotify(); })
            .DisposeWith(this);
        CheckNotify();
    }

    protected override void Register()
    {
        base.Register();
        foreach (var visitButton in visitButtons)
            visitButton.onClick.AddListener(SetOpened);
    }

    [ContextMenu("enable")]
    void OnEnable()
    {
        CheckNotify();
    }

    public void CheckNotify()
    {
        var hasNotify = HasNotify();
        gameObject.SetActive(hasNotify);

        foreach (var active in actives)
            active.OnChanged(hasNotify);

        foreach (var otherChecker in notifyCheckers)
            otherChecker.CheckNotify();
    }

    public void SetOpened()
    {
        opened = true;
        CheckNotify();
    }

    public bool HasNotify()
    {
        if (opened)
            return false;
        foreach (var otherNotify in notifies)
            if (HasNotify(otherNotify))
                return true;

        foreach (var otherChecker in notifyCheckers)
            if (otherChecker.HasNotify())
                return true;

        foreach (var otherChecker in adsRewardNotifies)
            if (otherChecker.HasNotify())
                return true;

        foreach (var key in adsItemKeys)
            if (HasNotify(key))
                return true;

        return false;
    }

    bool HasNotify(int key)
    {
        if (!Kernel.IsReady())
            return false;

        var state = RootView.rootView.ShopDatabase.GetAdsItem(key).GetState();
        return state == AdsItemState.WatchAds || state == AdsItemState.Receivable;
    }

    [MyButton(nameof(Test))] public int testAbc;
    public void Test()
    {
        Debug.Log(HasNotify());
    }

    bool HasNotify(NotifyEnum notify)
    {
        switch (notify)
        {
            case NotifyEnum.DailyGift:
                return !DailyData.dailyGiftChecked;
            case NotifyEnum.DailyGem:
                return false;
//                return PlayerDataController.IsSubscriptionReceivable(SubscriptionKey.DailyGem);
            case NotifyEnum.DailyQuest:
                foreach (var dailyQuestItem in GameSetting.dailyQuestItems)
                    if (dailyQuestItem.GetCurrentState() == QuestState.Complete)
                        return true;
                return false;
            case NotifyEnum.DailyQuestWatchAds:
                return false;
            case NotifyEnum.Achievement:
                foreach (var dailyQuestItem in GameSetting.achievementItems)
                    if (dailyQuestItem.GetCurrentState() == QuestState.Complete)
                        return true;
                return false;
            case NotifyEnum.OnlineGift:
                return false;
//                return TimerController.OnlineGiftRemain.Value <= 0 &&
//                       !PlayerDailyData.FinishOnlineGift() && TimerController.Started;
            case NotifyEnum.All:
                Debug.LogError("cannot notify all");
                throw new ArgumentOutOfRangeException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
