using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class EventDayManager : BaseActionScript
{
    [IndexAsEnum(nameof(EventDayType))]
    [SerializeField] private int eventType;
    private EventDayPlayerData Data => StaticDataController.EventDays[eventType];

    [SerializeField] private IntBindingReactiveProperty eventBind;
    [SerializeField] private IntBindingReactiveProperty progressStateBinding;
    [SerializeField] private Button openButton;
    [SerializeField] private BindingToStringTextChange bindEventName;
    [SerializeField] private CountDownView countDownView;
    [SerializeField] private ProgressIntArrayScript arrayProgressBar;
    [SerializeField] private RewardView currentRewardView;
    [SerializeField] private Button takeRewardButton;
    [SerializeField] private EarningRewardSetting earningSetting;
    [SerializeField] [DisplayAsDisable] private ProgressState currentState;
    
    protected override void Register()
    {
        base.Register();
        eventBind.Init();
        progressStateBinding.Init();
        if(openButton != null)
            openButton.onClick.AddListener(() =>
            {
                if (Data != null)
                    ScreenRoot.popupManager.ShowPopup<PopupEvent>();
            });
        if (takeRewardButton != null)
            takeRewardButton.onClick.AddListener(TakeReward);
     
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SubjectController
            .CustomUpdatedEvent.TakeUntilDisable(this)
            .Where(x => x == CustomUpdateEvent.Event)
            .Subscribe(data => { UpdateView(); });
        LoadEvent();
    }

    

    protected override void DoAction()
    {
        base.DoAction();
        UpdateView();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadEvent();
    }

    private void LoadEvent()
    {
        if(!KernelLoadedRegistered)
            return;
        var gc = RootView.rootView.gameController;
        StaticDataController.LoadEventPlayerData(eventType,gc.Level,eventKey => RootView.rootView.gameController.OnLoadedEventDay(eventType,eventKey));

        if (Data != null)
        {
            eventBind.Value = Data.EventKey;
            if (arrayProgressBar != null)
                arrayProgressBar.SetTargets(Data.rewards.Select(x => x.parameter).ToArray());
            if (bindEventName != null)
                bindEventName.OnChanged(Data.eventName);
            UpdateView();
        }
        else
        {
            eventBind.Value = -1;
            if (bindEventName != null)
                bindEventName.SetDefaultText();
            UpdateView();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            LoadEvent();
    }


    private void TakeReward()
    {
        if (Data != null)
        {
            var progressIndex = Data.GetCurrentTargetIndex(out currentState);
            if ((currentState & ProgressState.Ready) != 0)
            {
                Data.claimed[progressIndex] = true;
                Data.Save();
                var trade = new Trade<Product, Product>(Product.Free, Data.rewards[progressIndex]);
                var input = new ShowRewardInput(trade, earningSetting.rewardTypeFlag)
                {
                    closeCurrentPopup = earningSetting.closeCurrentPopupOnRewarded
                };
                RootView.rootView.screenRoot.ShowReward(input);
            }

            UpdateView();
            SubjectController.CustomUpdatedEvent.OnNext(CustomUpdateEvent.Event);
        }
    }

    private void UpdateView()
    {
        if (Data != null)
        {
            var progressIndex = Data.GetCurrentTargetIndex(out currentState);
            progressStateBinding.Value = (int) currentState;
            if (countDownView != null)
            {
                var addDays = Data.activeDays - (DuongExtensions.GetEpochDayOfYear() - Data.startEpochDate);
                countDownView.StartCountDown(DateTime.Today.AddDays(addDays), () => { });
               // UIDebugLog.LogError("count down to " + DateTime.Today.AddDays(addDays) + " addDays" + addDays);
            }

            if (arrayProgressBar != null)
            {
                arrayProgressBar.SetClaimed(Data.claimed);
                arrayProgressBar.OnChanged(Data.progress);
            }

            if (currentRewardView != null)
                currentRewardView.SetData(Data.rewards[progressIndex]);
        }
        else
        {
         //   Debug.LogError("duong no event day data"+Data);
        }
    }

    [MyButtonInt(nameof(TestFill))] public int testFill = 1;

    private void TestFill(int add)
    {
        Data.progress += add;
        Data.Save();
        UpdateView();
    }
}