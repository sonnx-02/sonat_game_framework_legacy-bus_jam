using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AchievementProgressBar : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.QuestEnum)] [SerializeField]
    protected int key;

    [SerializeField] private InterstitialLogDataView logShowInterstitial;
    [SerializeField] protected TweenDisplayScript[] progressFill; // fill bả
    [SerializeField] protected Button[] takeButtons; // claim button correspond level of reward
    [SerializeField] protected Button[] openDoubleAdsButton; // claim button correspond level of reward
    [SerializeField] protected IndexBindingScriptBase[] finishStateBindings; // finish 
    [SerializeField] protected IndexBindingScriptBase[] rewardAmountBindings; // reward
    [SerializeField] protected IndexBindingScriptBase[] conditionBindings; // condition
    [SerializeField] protected bool showReward = true;
    [SerializeField] protected float adsMultiple = 2;


    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    private int rewardTypeFlag1 = 1;

    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    private int rewardTypeFlag2 = 1;
    // show reward
//    [SerializeField] protected bool clearOnFinish = true;


    protected AchievementItem achievementItem; // descriptor of achievement

    protected override void Register()
    {
        base.Register();

        // get descriptor
        var gameSetting = Kernel.GetDatabase<GameSetting>();
        foreach (var data in gameSetting.achievementItems)
            if (data.key == key)
            {
                achievementItem = data;
                break;
            }

        // setup view
        SetUp();
        // setup function for button
        foreach (var takeButton in takeButtons)
            takeButton.onClick.AddListener(Claim);
        foreach (var open in openDoubleAdsButton)
            open.onClick.AddListener(OpenMultiple);
    }

    [SerializeField] protected float delay;

    private void OnEnable()
    {
        // update current progress view
        if (gameObject.activeInHierarchy)
            UpdateCurrentProgress();
    }

    private void SetUp()
    {
        foreach (var tweenDisplayScript in progressFill)
            tweenDisplayScript.SetTarget(achievementItem.rewards[achievementItem.rewards.Length - 1].parameter);
        for (var i = 0; i < rewardAmountBindings.Length; i++)
            rewardAmountBindings[i].OnChanged(achievementItem.rewards[i].amount);
        for (var i = 0; i < conditionBindings.Length; i++)
            conditionBindings[i].OnChanged(achievementItem.rewards[i].parameter);
    }

    [MyButtonInt(nameof(TestAdd))] [SerializeField]
    protected int testAdd;

    public virtual void TestAdd(int value)
    {
        PlayerData.achievementData[key] += value;
        UpdateCurrentProgress();
    }


    protected virtual void UpdateCurrentProgress()
    {
        //clear
        if (PlayerData.achievementChecker[achievementItem.key] >= achievementItem.rewards.Length)
        {
            PlayerData.achievementChecker[achievementItem.key] = 0;
            PlayerData.achievementData[key] = 0;
        }

        int currentProgress = PlayerData.achievementData[key];
        foreach (var tweenDisplayScript in progressFill)
            tweenDisplayScript.SetTo(currentProgress);
        int[] states = GetStates(currentProgress);

        for (var i = 0; i < finishStateBindings.Length; i++)
            finishStateBindings[i].OnChanged(states[i]);
    }

    protected int[] GetStates(int value)
    {
        int[] states = new int[finishStateBindings.Length];
        for (var i = 0; i < states.Length; i++)
            if (value >= achievementItem.rewards[i].parameter)
            {
                states[i] = (int) RewardState.Ready;
                if (PlayerData.achievementChecker[achievementItem.key] > i)
                    states[i] = (int) RewardState.Taken;
            }

        return states;
    }

    private void Claim()
    {
        var price = Product.Free;
        var product = achievementItem.GetReward();
        int rewardType = achievementItem.CurrentIndex == achievementItem.rewards.Length - 1
            ? rewardTypeFlag2
            : rewardTypeFlag1;
        var trade = new Trade<Product, Product>(price, product);
        var input = new ShowRewardInput(trade, rewardType, 0, trade.Product.parameter, adsMultiple);
        if (logShowInterstitial != null) input.logInterstitial = logShowInterstitial.data;
        RootView.rootView.screenRoot.ShowReward(input);

        PlayerData.achievementChecker[achievementItem.key]++;
        SubjectController.GameActionEvent.OnNext(GameActionEvent.SavePlayerData);
        AfterClaim();
    }


    /// <summary>
    /// dont know
    /// </summary>
    protected void OpenMultiple()
    {
        int rewardType = achievementItem.CurrentIndex == achievementItem.rewards.Length - 1
            ? rewardTypeFlag2
            : rewardTypeFlag1;
        var price = Product.Free;
        var product = achievementItem.GetReward();
        var trade = new Trade<Product, Product>(price, product);
        var input = new ShowRewardInput(trade, rewardType, 0, trade.Product.parameter, adsMultiple);
        if (logShowInterstitial != null) input.logInterstitial = logShowInterstitial.data;
        RootView.rootView.screenRoot.ShowReward(input, () =>
        {
            PlayerData.achievementChecker[achievementItem.key]++;
            AfterClaim();
        });
    }

    private void AfterClaim()
    {
        achievementItem.LogFirebase();
  
        1f.Timer(() => { SubjectController.NotifyEvent.OnNext(NotifyEnum.All); });

        UpdateCurrentProgress();
    }
}