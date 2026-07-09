using System;
using System.Linq;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum AdsItemCountDownType
{
    Unlimited = 0, // or stock
    Daily = 1,
    Weekly = 2,
    WaveSession = 3,
}


[Flags]
public enum AdsItemState : byte
{
    // Decimal     // Binary
    None = 0, // 000000
    WatchAds = 1, // 000001
    Receivable = 2, // 000010
    Wait = 4, // 000100
    Complete = 8, // 001000
    ConditionNotValid = 16, // 010000
    FreeTake = 32 // 100000
}

public enum ProgressType
{
    Progress,
    Remain,
    RemainProgress,
    StockProgress,
}


public enum CustomCallback
{
    None,
    ClosePopup,
    CloseDialog,
    CloseAllPopupAndDialog,
}

public abstract class BaseAdsRewardButton : BaseFrameworkView
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int clickSound = 1;

    [SerializeField] protected CustomCallback afterRewarded;

    private DuongAdsManager _adsManager;

    protected DuongAdsManager AdsManager
    {
        get
        {
            if (_adsManager == null)
                _adsManager = Kernel.Resolve<AdsManager>() as DuongAdsManager;
            return _adsManager;
        }
    }

    public Button WatchAdsButton => watchAdsButton;
    [SerializeField] private Button watchAdsButton;
    [SerializeField] private ToggleScript[] playableBindings; // if video ready
    [SerializeField] public bool freeTake;
    [SerializeField] protected bool takenOnceOnEnable;
    protected bool Taken;

    protected bool IsNotValidToTake()
    {
        return (takenOnceOnEnable && Taken);
    }
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();

        AdsManager.videoAdsReady.Subscribe(data => { 0.1f.Timer(UpdateReadyToWatchAds); }).DisposeWith(this);
        watchAdsButton.onClick.AddListener(() =>
        {
            clickSound.PlaySound();
            if (IsNotValidToTake())
                return;
            if (!freeTake)
            {
                Debug.Log("Watch ads");
                AdsManager.ShowVideoAds(RewardedHandler, null);
                var fireBase2 = Kernel.Resolve<FireBaseController>();
                var parameters2 = fireBase2.GetAdmobParameter(10, new[]
                {
                    new LogParameter("flow_seq ", "9"),
                    new LogParameter("ad_placement ", BaseGameController.GetCurrentPlacement()),
                }, AdTypeLog.interstitial).ToArray();
            }
            else
                FreeTake();
        });
        SubjectController.NotifyCustomGameCondition.TakeUntilDestroy(this)
            .Subscribe(data => UpdateReadyToWatchAds());
    }

    protected virtual void FreeTake()
    {
        freeTake = false;
        Debug.Log("Handler FreeTake");
    }


    protected virtual void OnEnable()
    {
        if (!Kernel.IsReady())
            return;
        UpdateReadyToWatchAds();
        if (takenOnceOnEnable && Taken)
            Taken = false;
    }

    protected void UpdateReadyToWatchAds()
    {
        var check = CheckReady();
        watchAdsButton.interactable = check;
        foreach (var toggleScript in playableBindings)
            toggleScript.OnChanged(check);
    }

    protected virtual bool CheckReady()
    {
        return freeTake || AdsManager.videoAdsReady.Value;
    }

    protected virtual void RewardedHandler()
    {
        0.1f.Timer(AfterRewarded);
//        AfterRewarded();
    }

    protected virtual void AfterRewarded()
    {
        switch (afterRewarded)
        {
            case CustomCallback.None:
                break;
            case CustomCallback.ClosePopup:
                ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe();
                break;
            case CustomCallback.CloseDialog:
                ScreenRoot.dialogController.CloseAll();
                break;
            case CustomCallback.CloseAllPopupAndDialog:
                ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe();
                ScreenRoot.dialogController.CloseAll();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}