using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class PopupShowRewardBase : BasePopup
{
    [SerializeField] protected Button claimButton;
    protected ShowRewardInput Input;
    protected Action Action;

    public virtual void SetData(ShowRewardInput input, Action actionAfterClaim = null)
    {
        Input = input;
        Action = actionAfterClaim;
//        DuongExtensions.ExecuteInNextFrame(() =>
//        {
//            DuongDataHelper.JsonPreSavePlayerDataSpendClaim(input.trade.Prices,input.trade.Products);
//            PlayerData.IgnoreSaveUntilTurnOn();
//        });
    }
    
    
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(claimButton != null)
            claimButton.onClick.AddListener(() => { Claim(false); });
    }

    public virtual void Claim(bool byAds)
    {
     
    }
}



public class PopupShowReward : PopupShowRewardBase
{
    [SerializeField] private float delayToClaim = 1;
    [SerializeField] private float delayToClose = 1;
    [SerializeField] private StandaloneAdsButtonView adsRewardButton;
    [SerializeField] private BaseRxAnimator[] animatorByParameter;
    [SerializeField] private BaseRxAnimator[] appearAnimations;
    [SerializeField] private BaseRxAnimator[] animatorBeforeClaim;
    [SerializeField] private ToggleScript[] toggleOpened;
    [SerializeField] private IndexBindingScriptBase[] toggleOpenedByParameter; // 1 is box
    [SerializeField] private BaseActionProductScript[] productBindings;
    [SerializeField] [DisplayAsDisable] private RewardView productView;
    [SerializeField] private RewardView[] productViews;
    [SerializeField] private RewardView productByAdsView;
    [SerializeField] private ToggleScript[] instantClaimBindings; // obsolete
    [SerializeField] private IndexBindingFlagState[] rewardTypeStateBindings;
    [SerializeField] private IndexBindingScriptBase[] bindingFinalRewardAmount;
    
    private Product _multipleProduct;
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(claimButton != null)
            claimButton.onClick.AddListener(() => { Claim(false); });
    }
    
    public override void SetData(ShowRewardInput input, Action actionAfterClaim = null)
    {
        base.SetData(input,actionAfterClaim);
        _multipleProduct = Input.trade.Product.Multiple(Input.multipleByAds);
        foreach (var rewardView in productViews)
            rewardView.SetData(Input.trade.Product);
        if(productByAdsView != null)
            productByAdsView.SetData(_multipleProduct);
        foreach (var productBinding in productBindings)
            productBinding.StartAction(Input.trade.Product);
        
        rewardTypeStateBindings.OnChanged(Input.rewardTypeFlag);
        bool claimInstantly = (Input.rewardTypeFlag & (int)RewardTypeFlag.ClaimInstantInPopup) != 0;
        instantClaimBindings.OnChanged(claimInstantly);
        if (claimInstantly) // 1 == ClaimInstantly = 1,
            Claim(false);

        foreach (var baseRxAnimator in appearAnimations)
            baseRxAnimator.ResetAnimation();
        foreach (var baseRxAnimator in animatorByParameter)
            baseRxAnimator.SetAtIn();
     
        if (Input.parameter == 0)
            AnimIn();
        else if (Input.parameter == 1) // is open box classic from locations
            foreach (var anim in animatorByParameter) // set position of tween box
            {
                anim.SetIndex(Input.parameter2);
                anim.IO_AnimatorIn().Subscribe(data => { AnimIn(); });
            }
    }

    protected override void CloseHandler()
    {
        base.CloseHandler();
        if (Input != null && Input.parameter > 0)
            foreach (var anim in animatorByParameter)
                anim.SetToggle(false);
    }

    private void AnimIn()
    {
        foreach (var baseRxAnimator in appearAnimations)
            baseRxAnimator.Animator_In();
    }


    private void ClaimByAds()
    {
        Claim(true);
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        toggleOpened.OnChanged(false);
        toggleOpenedByParameter.OnChanged(-1);
    }

    public override void Claim(bool byAds)
    {
        base.Claim(byAds);
        var toClaim = byAds ? _multipleProduct : Input.trade.Product;
        bindingFinalRewardAmount.OnChanged(toClaim.amount);
        toggleOpened.OnChanged(true);
        toggleOpenedByParameter.OnChanged(Input.parameter);
        Input.trade.OverrideProduct(new []{toClaim});
        Input.trade.Spend();
        delayToClaim.TimerUnscale(() =>
        {
            PlayerData.Claim(toClaim);
            PlayerData.Save();
            DailyData.Save();
            Action?.Invoke();
            DuongExtensions.ExecuteInNextFrame(PlayerData.CheckAndTurnOnSave);
        });
        
        if (byAds)
        {
            if(Input.logWhenAdsClaim != null)
                foreach (var log in Input.logWhenAdsClaim)
                    log.Post();
        }
        else
        {
            if(Input.logWhenClaim != null)
                foreach (var log in Input.logWhenClaim)
                    log.Post();
        }

        Observable.WhenAll(animatorBeforeClaim.ToList().Select(x => x.IO_AnimatorIn()).ToArray())
            .Subscribe(
                data =>
                {
                    // nếu có log thì show quảng cáo
                    if (Input.logInterstitial != null && !byAds)
                        (delayToClose + delayToClaim).TimerUnscale(() => Close(() =>
                            Kernel.Resolve<AdsManager>().ShowInterstitial(Input.logInterstitial)));
                    else
                        (delayToClose + delayToClaim).TimerUnscale(Close);
                });
    }
  
}