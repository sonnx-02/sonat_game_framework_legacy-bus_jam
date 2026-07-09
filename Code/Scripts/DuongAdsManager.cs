using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Serialization;

//#if UNITY_IOS
//using Unity.Advertisement.IosSupport;
//#endif


public class DuongAdsManager : AdsManager
{
    public override bool IsNoAds()
    {
        if (RootView.rootView.gameController.CustomNoAds())
            return true;
        return base.IsNoAds();
    }

    public override void EnableNoAds()
    {
        base.EnableNoAds();
        SubjectController.OnShopItemBought.OnNext(0);
    }

    public override Type GetThisType()
    {
        return typeof(AdsManager);
    }

    public BoolReactiveProperty videoAdsReady = new BoolReactiveProperty();

    protected override void Register()
    {
        base.Register();
        videoAdsReady.Value = IsVideoAdsReady();
        base.onVideoLoaded.Action += loaded => videoAdsReady.Value = loaded;
    }

    [SerializeField] private PopupManager popupManager;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (popupManager != null)
            StartCoroutine(ContinueCheckSwitchBanner());
    }

    private IEnumerator ContinueCheckSwitchBanner()
    {
        yield return new WaitForSeconds(10);
        while (true)
        {
            if(popupManager.IsNativePopupOnShow())
                Kernel.Resolve<AdsManager>().SwitchBanner(false);
            yield return new WaitForSeconds(1);
        }
    }

    protected override bool CustomValid()
    {
//        if(BaseFrameworkView.PlayerData.tutorialStep >= 0)
//            return false;

        return true;
    }

    protected override bool OnApplicationFocusValid()
    {
        if (RootView.rootView != null && RootView.rootView.screenRoot.dialogController.CurrentDialog != null
                                      && RootView.rootView.screenRoot.dialogController.CurrentDialog.turnOffInFocusAds)
            return false;

        return true;
    }

    protected override void SetRewardedLogName(SonatLogVideoRewarded log)
    {
        SonatAnalyticTracker.RewardedLogPlacement = RootView.rootView.gameController.GetCurrentPlacement();
    }
}