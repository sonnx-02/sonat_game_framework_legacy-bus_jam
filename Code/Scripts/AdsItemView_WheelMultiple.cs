using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AdsItemView_WheelMultiple : AdsItemView
{
    [ConditionalField(nameof(indicator),new object[]{null})]
    [SerializeField] private int multiple = 2;
    protected override float Multiple => indicator != null ? indicator.GetMultiple() : multiple;

    [SerializeField] private Button takeNotMultipleReward;
    [SerializeField] private RxTweenScript_MultipleIndicator indicator;
    [SerializeField] private IndexBindingScriptBase[] multipleBindings;
    [SerializeField] private IndexBindingScriptBase[] multipleValueBindings;
    [SerializeField] private bool showInterstitial;

    [SerializeField] [ConditionalField(nameof(showInterstitial))]
    private string showIntersLog;

    [SerializeField] [ConditionalField(nameof(showInterstitial))]
    private bool conditionLevel = true;

    [SerializeField] private GameActionEvent actionAfterClaim = GameActionEvent.None;
    [SerializeField] private BaseBaseActionScript[] actionAfterClaim2;
    [SerializeField] private float delayAction;
    [SerializeField] private IndexBindingScriptBase[] bindIndicatorIndex;
    [SerializeField] private bool resetAdsCounterOnStart = true;
    [SerializeField] private bool stopStateBindAfterClaim = true;

    [SerializeField] private IntValueRetrieverBase overrideAmount;
    [SerializeField] private RewardView overrideRewardView;

    protected override Product GetRewardItem()
    {
        if (overrideAmount == null)
            return base.GetRewardItem();
        var clone = base.GetRewardItem().Clone();
        clone.amount = overrideAmount.Value;

        if (overrideRewardView != null)
        {
            overrideRewardView.SetData(clone);
        }

        return clone;
    }

    protected override void Register()
    {
        base.Register();
        if (resetAdsCounterOnStart)
            AdsItem.AdsRewardCounter[key] = 0;

        takeNotMultipleReward.onClick.AddListener(() =>
        {
            if (showInterstitial)
                Kernel.Resolve<AdsManager>()
                    .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false, conditionLevel);
            0.2f.Timer(() =>
            {
                ((int) BaseSoundEnum.ButtonClick).PlaySound();
                // AfterRewarded();
                TakeRewardOnly();
            });
        });

        if (indicator != null)
            WatchAdsButton.onClick.AddListener(() => { indicator.StopTween(); });

        _currentIndicate.Subscribe(bindIndicatorIndex.OnChanged);
    }

    protected override void TakeReward()
    {
 
        if (stopStateBindAfterClaim)
            foreach (var indexBindingScript in AdsStateBindings)
            {
                if(indexBindingScript is IndexBindingScript script)
                    script.Ignore = true;
            }
        base.TakeReward();
        delayAction.Timer(() =>
        {
            SubjectController.GameActionEvent.OnNext(actionAfterClaim);
            actionAfterClaim2.StartAction();
        });
    }


    private IDisposable _multipleDisposable;
//    [SerializeField] private bool 

    protected override void OnEnable()
    {
        base.OnEnable();

        foreach (var indexBindingScript in AdsStateBindings)
            if(indexBindingScript is IndexBindingScript script)
                script.Ignore = true;
        UpdateState();

        if (indicator != null)
        {
            var reward = GetRewardItem();
            _multipleDisposable = Observable.EveryUpdate().TakeUntilDisable(this).Subscribe(data =>
            {
                var m = indicator.GetMultiple(out int index);
                _currentIndicate.Value = index;
                multipleBindings.OnChanged(m * reward.amount);
                multipleValueBindings.OnChanged(m);
                if (state == AdsItemState.Receivable)
                    _multipleDisposable.Dispose();
            });
        }
        else
        {
            var reward = GetRewardItem();
            multipleBindings.OnChanged(Multiple * reward.amount);
            multipleValueBindings.OnChanged(Multiple);
        }
    }

    private readonly IntReactiveProperty _currentIndicate = new IntReactiveProperty();

    protected virtual void TakeRewardOnly()
    {
        
        if (indicator != null)
            indicator.StopTween();
        if(IsNotValidToTake())
            return;
        TakeReward(1, false);
    }
}