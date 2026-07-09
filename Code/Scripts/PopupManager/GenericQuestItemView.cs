using System;
using System.Collections;
using System.Collections.Generic;
using Sonat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GenericQuestItemView : BaseFrameworkView
{
    [SerializeField] protected Image avatar;
    [SerializeField] protected RewardView rewardView;
    [SerializeField] protected IndexBindingScriptBase[] keyBindings;
    [SerializeField] protected TMP_Text desc;
    [SerializeField] protected TMP_Text progressText;
    [SerializeField] protected IndexBindingScriptBase[] conditionBinds;
    [SerializeField] protected Image fillProgress;
    [SerializeField] protected ProgressScript[] progressScripts;
    [SerializeField] protected Button btnGet;
    [SerializeField] protected IndexBindingScriptBase[] stateBindings;
    [SerializeField] protected InterstitialLogDataView logShowInterstitial;

    [SerializeField] protected string placement = "Achievement";

    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    private int rewardTypeFlag = 1;

    [SerializeField] private float multi = 2f;

    [SerializeField] [DisplayAsDisable] private QuestState questState;
    protected BaseQuestItem _data;

    public QuestState QuestState => questState;


    protected override void Register()
    {
        base.Register();
        if (btnGet != null)
            btnGet.onClick.AddListener(GetReward);
    }

    public void GetReward()
    {
        BaseGameController.LogShortcut("get_quest_reward");
        //  DailyData.dailyQuestData[(int) BaseQuestEnum.CompleteQuest] += 1;
        var product = _data.GetReward();
       
        var trade = new Trade<Product, Product>(Product.Free, product);
        var input = new ShowRewardInput(trade, rewardTypeFlag, 0, trade.Product.parameter, multi);
        if (logShowInterstitial != null) input.logInterstitial = logShowInterstitial.data;
        RootView.rootView.screenRoot.ShowReward(input);
        SubjectController.GameActionEvent.OnNext(GameActionEvent.SavePlayerData);
        1f.Timer(() => { SubjectController.NotifyEvent.OnNext(NotifyEnum.All); });
        Setup(_data);
        _data.FillData();
        _data.LogFirebase();
        UpdateStatus();
    }

    private void OnEnable()
    {
        UpdateStatus();
    }

    public bool ReadyToTake()
    {
        UpdateStatus();
        return (questState & QuestState.Complete) != 0;
    }


    public virtual void Setup(BaseQuestItem data)
    {
        _data = data;
        keyBindings.OnChanged(data.Key);
        if (avatar != null)
            avatar.sprite = data.QuestAvatar;
        if (rewardView != null)
            rewardView.SetData(_data.GetReward());
        if (desc != null)
            desc.text = string.Format(data.Description, _data.Condition);
        UpdateStatus();
    }

    public virtual void UpdateStatus()
    {
        if (_data == null)
            return;
        BaseUpdateStatus();
    }

    protected virtual void BaseUpdateStatus()
    {
        var progress = _data.GetProgress();
        var progressSrt = _data.GetProgressString();
        conditionBinds.OnChanged(_data.Condition);
        if (progressText != null)
            progressText.text = progressSrt;
        if (fillProgress != null)
            fillProgress.fillAmount = progress;
        foreach (var progressScript in progressScripts)
            progressScript.OnChanged(progress);
        questState = _data.GetCurrentState();
        stateBindings.OnChanged((int) questState);
        if (questState == QuestState.Complete)
            SubjectController.NotifyEvent.OnNext(_data.NotifyEnum);
    }
}