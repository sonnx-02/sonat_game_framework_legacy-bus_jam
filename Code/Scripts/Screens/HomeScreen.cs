using System;
using UniRx;
using UnityEngine;

public class HomeScreen : BaseScreen
{
    private IDisposable _showDisposable;

    [IndexAsEnum(BuiltInEnumType.SubscriptionKey)] [SerializeField]
    private int[] subscriptions;

    [SerializeField] private float delayTime = 0;
    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    private int rewardTypeFlag = 1;

    [SerializeField] private ShowPopupButton noAdsButton;
    [SerializeField] private ShowPopupButton noAds24hButton;

    public override void OnShowHandler()
    {
        base.OnShowHandler();

        //Hide banner
        Kernel.Resolve<AdsManager>().HideBanner();

        //if (Kernel.Resolve<AdsManager>().IsBannerShowing())
        //    Kernel.Resolve<AdsManager>().HideBanner();
        //Kernel.Resolve<AdsManager>().onBannerShow.Action += HideBanner;

        //Check buy noads
        RefreshNoAdsButton();
    }

    public override void OnHideHandler()
    { 
        base.OnHideHandler();

        //Kernel.Resolve<AdsManager>().onBannerShow.Action -= HideBanner;
    }

    protected override bool GetSomeThingShow()
    {
        if (_preShowPopup)
        {
            ScreenRoot.popupManager.ShowPopup(_preShowPopup);
            _preShowPopup = null;
            return true;
        }
        
        foreach (var showOrder in showOrders)
        {
            switch (showOrder)
            {
                case ShowOrder.CustomScript:
                    // check show offer first
                    var checkCustom = CheckCustomScript();
                    if (checkCustom)
                        return true;
                    break;
                case ShowOrder.Subscription:
                    var subscriptionReward = CheckSubscriptionReward();
                    if (subscriptionReward)
                        return true;
                    break;
                case ShowOrder.Offer:
                    // check show offer first
                    var showOffer = CheckOffer();
                    if (showOffer)
                        return true;
                    break;
                case ShowOrder.Base:
                    // check show other
                    bool someThingToShowBase = base.GetSomeThingShow();
                    if (someThingToShowBase)
                        return true;
                    break;
                case ShowOrder.Navigation:
                    if (ScreenRoot.navigationManager != null && ScreenRoot.navigationManager.IsOnNavigation())
                        return true;
                    if (ScreenRoot.navigationManager != null && ScreenRoot.navigationManager.CheckActivate())
                        return true;
                    if (CheckShowNavi())
                        return true;
                    break;
                    // if navigation not showing anything
                case ShowOrder.Popup:
                    if (CheckShowPopup())
                        return true;
                    break;
                case ShowOrder.Quest:
                    if (CheckShowQuest())
                        return true;
                    break;
                case ShowOrder.BaseNavi:
                    if (CheckShowNavi())
                        return true;
                    break;
                default:
                    return base.GetSomeThingShow();
            }
        }
        return base.GetSomeThingShow();
    }
    

    private bool CheckSubscriptionReward()
    {
        foreach (var subscription in subscriptions)
        {
            if (IsSubscriptionReadyTake(subscription))
            {
                delayTime.Timer(() =>
                {
                    var subscriptionItem = RootView.rootView.ShopDatabase.GetSubscription(subscription);
                    if (subscriptionItem.autoTakeReward &&
                        !(ScreenRoot.popupManager.AnyPopupShow || ScreenRoot.dialogController.AnyDialogShow))
                        TakeReward(subscription);
                });
                return true;
            }

            break;
        }

        return false;
    }
    

    // duong_comment_flow_offer_show0 : Check if any offer ready to show when home screen
    private bool CheckOffer()
    {
        if(PlayerData.CurrentLevel + 1 < RemoteConfigKey.start_level_show_offer.GetValueInt())
            return false;
        
        var toShow = Kernel.GetDatabase<ShopDatabase>().GetAnOfferReadyToShowWhenHomeScreen();
        if (toShow != null && toShow.RandomShowRate())
        {
            DuongExtensions.ExecuteInNextFrame(() =>
            {
                toShow.delayToShow.Timer(() =>
                    {
                        // duong_comment_flow_offer_show5 : Check if screen ready to show not being used by other popup
                        if (!(ScreenRoot.popupManager.AnyPopupShow || ScreenRoot.dialogController.AnyDialogShow))
                            ActiveAndShow(toShow);
                        else
                            _showDisposable = Observable.Interval(TimeSpan.FromSeconds(0.5f))
                                .Where(x => !(ScreenRoot.popupManager.AnyPopupShow ||
                                              ScreenRoot.dialogController.AnyDialogShow))
                                .Subscribe(data =>
                                {
                                    ActiveAndShow(toShow);
                                }).AddToScreenChangeDisposable()
                                .AddToGameDisposable();
                    })
                    .AddToScreenChangeDisposable()
                    .AddToGameDisposable();
            });

            return true;
        }

        return false;
    }

    // duong_comment_flow_offer_show6 : active and show offer
    private void ActiveAndShow(Offer toShow)
    {
        // duong_comment_flow_offer_show7 : save offer info to DailyData to check it won't show many times later
        toShow.ActiveOffer();
        ScreenRoot.popupManager.Resolve<PopupOffer>().CheckShow(toShow.key);
        _showDisposable?.Dispose();
    }

    private void TakeReward(int key)
    {
        if (IsSubscriptionReadyTake(key))
        {
            var reward = TakeSubscriptionReward(key);
            var trade = new Trade<Product, Product>(Product.Free, reward);
            var input = new ShowRewardInput(trade, rewardTypeFlag);
            RootView.rootView.screenRoot.ShowReward(input);
        }
    }

    private void HideBanner(bool isLoaded)
    {
        if (isLoaded)
        {
            Kernel.Resolve<AdsManager>().HideBanner();
        }
    }

    public void RefreshNoAdsButton()
    {
        bool isNoAds = Kernel.Resolve<AdsManager>().IsNoAds();

        noAds24hButton.gameObject.SetActive(!PlayerData.noAds24h && !PlayerData.noAdsForever && !isNoAds);
        noAdsButton.gameObject.SetActive(!PlayerData.noAdsForever && !isNoAds && !noAds24hButton.gameObject.activeSelf);
    }
}