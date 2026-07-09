using System;
using System.Linq;
using System.Timers;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class AdsItemView : BaseAdsRewardButton
{
    [IndexAsEnum(BuiltInEnumType.AdsItemKey)] [SerializeField]
    protected int key;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int takeRewardClickSound;

    [SerializeField] private IndexBindingScriptBase[] adsStateBindings;
    [SerializeField] private ToggleScript[] activeBeforeClaim;


    public IndexBindingScriptBase[] AdsStateBindings => adsStateBindings;

    [SerializeField] private IndexPropertySourceRetriever[] conditions;
    [SerializeField] private RewardView rewardView;
    [SerializeField] private IndexBindingScriptBase amountBinding;
    [SerializeField] private IndexBindingScriptBase[] adsCounterBindings; // counter to possible to claim
    [SerializeField] private BindingToStringTextChange progressText;
    [SerializeField] private CountDownView countDownView;
    [SerializeField] private bool notCloseCurrentDialog;
    [SerializeField] private bool autoTakeReward = true;
    [SerializeField] private bool updateRewardOnEnable; // to re-update reward custom 
    [SerializeField] private CustomClaimScript customClaimScript;

    [SerializeField] [ConditionalField(nameof(autoTakeReward), new object[] {true}, true)] 
    private Button btnTakeReward;
    [DisplayAsDisable] [SerializeField] private ProgressType progressType;
    [DisplayAsDisable] [SerializeField] protected AdsItemState state;

    // private
    private AdsItem _adsItem;
    // ReSharper disable once MemberCanBePrivate.Global
    private IDisposable _wait;
    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag, 0)] [SerializeField]
    private int rewardTypeFlag = 1;
    
    #if UNITY_EDITOR
    [SerializeField] [LabelHeader("CustomCondition defined in AdsItem",1,0,1)] private bool a;
    #endif

    // protected 
    protected virtual AdsItem AdsItem => _adsItem ??= RootView.rootView.ShopDatabase.GetAdsItem(key);

    protected override void Register()
    {
        base.Register();

        if (!autoTakeReward)
            btnTakeReward.onClick.AddListener(() =>
            {
                if (IsNotValidToTake())
                    return;
                takeRewardClickSound.PlaySound();
                TakeReward();
            });

        if (AdsItem == null)
            Debug.LogError("null", gameObject);

        SetRewardView();
    }

    public void OverrideAdsItemKey(int k)
    {
        key = k;
        _adsItem = null;
        SetRewardView();
        UpdateState();
    }

    protected virtual Product GetRewardItem()
    {
        if (AdsItem.products.Length == 1)
        {
            var reward =  AdsItem.products[0];
            if (reward.quantity == Quantity.CustomProduct)
                return BaseGameController.GetCustomProduct(reward);
            return reward;
        }
#if UNITY_EDITOR
        Debug.LogError("AdsItem not valid " + BuiltInEnumType.AdsItemKey.GetName(key));
#endif
        return null;
    }

    private void SetRewardView()
    { 
        var reward = GetRewardItem();
        if (rewardView != null) rewardView.SetData(reward);
        if (amountBinding != null) amountBinding.OnChanged(reward.amount);
    }


    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        UpdateState();
        SubjectController.OnAdsItemKeyDataUpdated.Subscribe(data => UpdateState());
    }

    protected override string GetName()
    {
        return base.GetName() + "_" + ProjectSettings.GetKey(BuiltInEnumType.AdsItemKey.ToString(), key);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        if (!Kernel.IsReady())
            return;
    
        base.OnEnable();
        if (updateRewardOnEnable)
            SetRewardView();
        foreach (var indexProperty in conditions)
            indexProperty.GetProperty().TakeUntilDisable(this)
                .Subscribe(data =>
                {
                    UpdateState();
                    UpdateReadyToWatchAds();
                });
        UpdateState();
        UpdateReadyToWatchAds();
        activeBeforeClaim.OnChanged(true);
    }

    protected override bool CheckReady()
    {
        bool customCondition = GetState() == AdsItemState.WatchAds && (AdsItem.customCondition == 0 ||
                                                                       RootView.rootView != null &&
                                                                       BaseGameController.GetCustomCondition( AdsItem.customCondition));
        return freeTake || (base.CheckReady() && customCondition);
    }

    public virtual AdsItemState GetState()
    {
        if (freeTake)
            return AdsItemState.FreeTake;
        foreach (var indexProperty in conditions)
            if (!indexProperty.GetProperty().BoolValue)
                return AdsItemState.ConditionNotValid;
        return AdsItem.GetState();
    }

    public bool AdsValidAndReadyToWatch() => GetState() == AdsItemState.WatchAds && AdsManager.videoAdsReady.Value;

    protected void UpdateState()
    {
        foreach (var adsCounterBinding in adsCounterBindings)
            adsCounterBinding.OnChanged(AdsItem.AdsRewardCounter[key]);

        state = GetState();
        adsStateBindings.OnChanged((int) state);

        switch (progressType)
        {
            case ProgressType.Progress:
                if (progressText != null)
                    progressText.OnChanged(AdsItem.AdsRewardCounter[key] + "/" + AdsItem.countToGetReward);
                break;
            case ProgressType.Remain:
                if (progressText != null)
                    progressText.OnChanged((AdsItem.countToGetReward - AdsItem.AdsRewardCounter[key]).ToString());
                break;
            case ProgressType.RemainProgress:
                if (progressText != null)
                    progressText.OnChanged((AdsItem.countToGetReward - AdsItem.AdsRewardCounter[key]) + "/" +
                                           AdsItem.countToGetReward);
                break;
            case ProgressType.StockProgress:
                if (progressText != null)
                    progressText.OnChanged((AdsItem.maxInDay - AdsItem.AdsRewardTaken[key]) + "/" + AdsItem.maxInDay);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (state == AdsItemState.Wait)
        {
            if (countDownView != null)
                countDownView.StartCountDown(DateTime.FromBinary(AdsItem.AdsTimeTick[key]), UpdateState);
            else
            {
                TimeSpan difference = DateTime.FromBinary(AdsItem.AdsTimeTick[key]).Subtract(DateTime.Now);
                difference = difference.Add(TimeSpan.FromSeconds(1));
                Debug.Log(difference.Seconds);
                _wait?.Dispose();
                _wait = Observable.Timer(difference).Subscribe(data => { UpdateState(); });
            }
        }
    }


    protected override void FreeTake()
    {
        base.FreeTake();
        TakeReward();
    }

    protected override void RewardedHandler()
    {
        base.RewardedHandler();
        RootView.rootView.screenRoot.dialogController.ShowOverride = notCloseCurrentDialog;
        AdsItem.AdsRewardCounter[key]++;
        AdsItem.AdsTimeTick[key] = DateTime.Now.AddSeconds(AdsItem.adsTimeGap).ToBinary();
        if (autoTakeReward && GetState() == AdsItemState.Receivable)
            TakeReward();
        else
            UpdateState();

        SubjectController.NotifyEvent.OnNext(NotifyEnum.AdsRewarded);
        SubjectController.NotifyCustomGameCondition.OnNext(true);
        Kernel.LogShowVideo($"{AdsItem.products[0].quantity}_{AdsItem.products[0].index}");

//        if (AdsItem.products[0].quantity == Quantity.Diamond || AdsItem.products[0].quantity == Quantity.Gold)
//            PlayerData.playGameInfo.AddViewAdsByGem(AdsItem.products[0].amount);
    }

    protected virtual void TakeReward()
    {
        TakeReward(Multiple);
    }

    [SerializeField] private BaseBaseActionScript[] actionOnClaim;
    [SerializeField] private BaseBaseActionScript[] actionOnClaimByAds;
    [SerializeField] private BaseBaseActionScript[] actionOnClaimByNoAds;
    [SerializeField] private BaseBaseActionScript[] booleanActionOnClaimByAds;
    [SerializeField] private RewardView[] finalRewards;

    
    protected virtual void TakeReward(float multi,bool byAds = true)
    {
        if (IsNotValidToTake())
            return;
        Taken = true;
        AdsItem.AdsRewardCounter[key] = 0;
        AdsItem.AdsRewardTaken[key]++;
        var reward = GetRewardItem().Multiple(multi);
        var price = new Product(Quantity.WatchAds, 1, key, 0, 0);
        var trade = new Trade<Product, Product>(price, reward);
        var input = new ShowRewardInput(trade, rewardTypeFlag,byAds);
        if ((rewardTypeFlag & (int) RewardTypeFlag.CustomClaimScript) != 0 && input.CustomClaimScript == null)
            input.CustomClaimScript = customClaimScript;
        
        foreach (var view in finalRewards)
            view.SetData(reward);
        
        RootView.rootView.screenRoot.ShowReward(input);
        UpdateState();
        SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
        SubjectController.OnAdsRewarded.OnNext(key);
        actionOnClaim.StartAction();
        if(byAds)
            actionOnClaimByAds.StartAction();
        else
            actionOnClaimByNoAds.StartAction();
        booleanActionOnClaimByAds.StartAction(byAds);

//        if (rewardTypeFlag != (int) RewardTypeFlag.CustomClaimScript)
//        {
//          
//        }
//        else
//        {
//            customClaimScript.Handler(input);
//            UpdateState();
//            SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
//            SubjectController.OnAdsRewarded.OnNext(key);
//        }
    }

  

    protected virtual float Multiple => 1;

    public virtual bool HasNotify()
    {
        if (!Kernel.IsReady())
            return false;

        var state1 = GetState();
        var valid = state1 == AdsItemState.WatchAds || state1 == AdsItemState.Receivable;
        return valid && AdsManager.videoAdsReady.Value;
    }

    protected override void AfterRewarded()
    {
        base.AfterRewarded();
        activeBeforeClaim.OnChanged(false);
    }

    public void CheckActiveIfInStock()
    {
        gameObject.SetActive(AdsItem.IsInStock());
    }

    public void SetFreeTake(bool set)
    {
        freeTake = set;
        UpdateState();
        UpdateReadyToWatchAds();
    }
}