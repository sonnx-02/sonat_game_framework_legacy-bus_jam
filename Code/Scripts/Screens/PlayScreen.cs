using UnityEngine;
using UniRx;

public class PlayScreen : BaseScreen
{
    [SerializeField] private TweenDisplayScript tweenCoin;
    [SerializeField] private TweenDisplayScript tweenGem;
    [SerializeField] private TweenDisplayScript tweenScore;

    [SerializeField] private ToggleScript onReady;
    [SerializeField] private ToggleScript onPlaying;
    [SerializeField] private IndexBindingScriptBase[] startGameLevelBindings; // update only when start game not always correspond with current level
//    public IndexBindingScript[] levelBindings; // always correspond with current level
    [SerializeField] private bool prepareGameOnShow;

    [SerializeField] private BaseBaseShopItemView[] boosterItems;

    [SerializeField] private ShowPopupButton noAdsButton;
    [SerializeField] private ShowPopupButton noAds24hButton;

    protected override void Register()
    {
        base.Register();
        if (tweenCoin != null)
            CoinTween.Subscribe(data => tweenCoin.Handler(data));
        if (tweenGem != null)
            GemTween.Subscribe(data => tweenGem.Handler(data));
        if (tweenScore != null)
            ScoreTween.Subscribe(data => tweenScore.Handler(data));
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
    
    }

    public override void OnShowHandler()
    {
        base.OnShowHandler();

        if (prepareGameOnShow)
        {
            SubjectController.GameActionEvent.OnNext(GameActionEvent.PrepareToReadyToPlay);
        }

        //Show banner
        Kernel.Resolve<AdsManager>().ShowBanner();
        //Kernel.Resolve<AdsManager>().onBannerShow.Action += ShowBanner;

        //Check buy noads
        RefreshNoAdsButton();
    }

    public override void OnHideHandler()
    {
        base.OnHideHandler();

        //Kernel.Resolve<AdsManager>().onBannerShow.Action -= ShowBanner;
    }

    public void SetupStartLevel(int currentLevelValue)
    {
        for (int i = 0; i < boosterItems.Length; i++)
        {
            boosterItems[i].ResetFreeStatus();
        }

        ShowThis(true);
        startGameLevelBindings.OnChanged(currentLevelValue);
        if (onReady != null) onReady.OnChanged(true);
        if (onPlaying != null) onPlaying.OnChanged(false);
    }

    private void ShowBanner(bool isLoaded)
    {
        if (isLoaded)
        {
            Kernel.Resolve<AdsManager>().ShowBanner();
        }
    }

    public void RefreshNoAdsButton()
    {
        bool isNoAds = Kernel.Resolve<AdsManager>().IsNoAds();

        noAds24hButton.gameObject.SetActive(!PlayerData.noAds24h && !PlayerData.noAdsForever && !isNoAds);
        noAdsButton.gameObject.SetActive(!PlayerData.noAdsForever && !isNoAds && !noAds24hButton.gameObject.activeSelf);
    }
}
