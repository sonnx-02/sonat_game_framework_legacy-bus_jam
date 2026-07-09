using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static BaseScreen;

// ReSharper disable InconsistentNaming
public enum CanvasRenderModeChange
{
    Default,
    Overlay,
    Camera,
    WorldSpace,
    NoChanged,
}

public abstract class BaseScreen : BaseFrameworkView
{
    [SerializeField] private CanvasRenderModeChange renderModeChange;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private BaseRxAnimator[] anims; // anim In,Out when screen show/close
    [SerializeField] private BaseRxAnimator[] animsResetOnShow; // anim In,Out when screen show/close
    [SerializeField] private IndexBindingScriptBase[] bindCurrentLevelOnShow;
    [SerializeField] private Activator activator;
    [SerializeField] private BaseRxAnimator[] popupDisplayAnims; // anim when a popup show, ex : top bar would hide when a popup show
    [SerializeField] private bool popupAnimsShowInstant; // make popupDisplayAnims anim instant
    [SerializeField] private LocationManager locationManager; // location for anim's target, like gold, gem, etc
    [SerializeField]
    private ButtonOpenOffer[] offerButtons; // button to display offer, here to set it active when screen show
    [SerializeField] private ShopItemView[] checkInStocks; // ex : to check no ads button is in stock or not
    [SerializeField] private BaseScreenDependency[] dependencies; // to customize for games
    [SerializeField] private ToggleScript[] onActives; // ex : when PlayScreenShow, correspond 2D Board show
    [HideInInspector]
    [SerializeField] private BaseChecker[] checkers;
    [SerializeField] private BaseBaseActionScript[] onShowScript; 
    [SerializeField] private BaseBaseActionScript[] beforeShowActions;
    public LocationManager GetLocationManager() => locationManager;
    [SerializeField] private BaseCheckScript[] customCheckShow;
    [SerializeField] private InScreenPopupEnum[] checkShowPopups;
    [SerializeField] private GenericQuestItemView[] checkQuest;
    [SerializeField] private string screenName; // to log screen
    private CanvasScalerByResolution _canvasScalerByResolution;

    public CanvasRenderModeChange RenderModeChange => renderModeChange;
    public string ScreenName => (string.IsNullOrEmpty(screenName) ? GetType().Name.ToLower() : screenName);

    [SerializeField] private bool topBar;

    public bool TopBar => topBar;


    public T ResolveDependency<T>() where T : BaseScreenDependency
    {
        var t2 = typeof(T);
        foreach (var basePopupDependency in dependencies)
        {
            var t = basePopupDependency.GetType();
            if (t == t2 || t.IsSubclassOf(t2))
                return (T) basePopupDependency;
        }

        return null;
    }


    // set anim show/hide when a popup show
    private void OnPopupHideShow(bool popupHidden, bool instant)
    {
        foreach (var baseRxAnimator in popupDisplayAnims)
            if (popupHidden)
                if (instant)
                    baseRxAnimator.SetAtIn();
                else
                    baseRxAnimator.Animator_In();
            else if (instant)
                baseRxAnimator.ResetAnim();
            else
                baseRxAnimator.Animator_Out();
    }

    protected override void Register()
    {
        base.Register();
        _canvasScalerByResolution = GetComponent<CanvasScalerByResolution>();
        OnHideHandler();
        #if UNITY_EDITOR
        if (checkers != null && checkers.Length > 0)
        {
            Debug.LogError("not using now, use on show or before show BaseBaseActionScript");
            Debug.Break();
        }
        #endif
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (gameObject.activeSelf)
            CheckShowOfferButtonAndInStock(); // check show offer button
        SubjectController.OfferActivated.Where(x => gameObject.activeSelf)
            .TakeUntilDestroy(this)
            .Subscribe(data => CheckShowOfferButtonAndInStock()); // check show offer button
        SubjectController.InAppPurchasedEvent.Where(x => gameObject.activeSelf)
            .TakeUntilDestroy(this)
            .Subscribe(data => 0.5f.Timer(CheckShowOfferButtonAndInStock)); // check show offer button

        if (graphicRaycaster == null)
            graphicRaycaster = GetComponent<GraphicRaycaster>();
    }

    private void CheckShowOfferButtonAndInStock()
    {
        foreach (var buttonOpenOffer in offerButtons)
            buttonOpenOffer.CheckActive();
        foreach (var checkInStock in checkInStocks)
            checkInStock.CheckActiveIfInStock();
    }

    public virtual void OnHideHandler()
    {
        onActives.OnChanged(false);
        activator.OnDeactivate();
        _preShowPopup = null;
        
        foreach (var baseScreenDependency in dependencies)
            baseScreenDependency.OnHide();
    }

    public int ShowTimes { get; set; }

    public void TurnGraphicRaycast(bool value)
    {
        if (graphicRaycaster != null)
            graphicRaycaster.enabled = value;
    }

    protected static bool IsInitialized;


    public virtual void OnShowHandler()
    {
        TurnGraphicRaycast(true);
        foreach (var beforeShowAction in onShowScript)
            beforeShowAction.StartAction(0);
        bindCurrentLevelOnShow.OnChanged(PlayerData.CurrentLevel);
        ShowTimes++;
        onActives.OnChanged(true);
        foreach (var baseScreenChecker in checkers)
            baseScreenChecker.Check();
        DailyData.CheckNewDay();
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_In();
        foreach (var baseRxAnimator in animsResetOnShow)
            baseRxAnimator.ResetAnim();
        activator.OnActivate();
        OnPopupHideShow(false, popupAnimsShowInstant);
        CheckShowOfferButtonAndInStock();
        SubjectController.UpdateUserProperty.OnNext(true);
        foreach (var baseScreenDependency in dependencies)
            baseScreenDependency.OnShow();
        Time.timeScale = 1;
        SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
        ScreenRoot.ApplyCanvasScaler(_canvasScalerByResolution);
        if(IsInitialized)
            ShowSomeThing();
        LogScreen();
        ScreenRoot.CheckLogScreen();
        _preShowPopup = null;
    }

    public void OnShowReInvoke()
    {
        ScreenRoot.ApplyCanvasScaler(_canvasScalerByResolution);
    }

//    private void OnEnable()
//    {
//        Debug.LogError("on enabler"+gameObject,gameObject);
//    }
//    
//    private void OnDisable()
//    {
//        Debug.LogError("on enabler"+gameObject,gameObject);
//    }

    [SerializeField] private BaseCheckScriptSkip stupidCheckShowOffer;
    [SerializeField] protected ShowOrder[] stupidShowOrders = {
        ShowOrder.Navigation,
        ShowOrder.CustomScript,
        ShowOrder.Subscription,
        ShowOrder.Base,
        ShowOrder.Quest,
        ShowOrder.BaseNavi,
    };
    public virtual void ShowSomeThing()
    {
        if (_preShowPopup)
        {
            ScreenRoot.popupManager.ShowPopup(_preShowPopup);
            _preShowPopup = null;
            return;
        }

        if (stupidCheckShowOffer != null)
        {
            if(stupidCheckShowOffer.Check())
                return;
            if(GetSomeThingToShowByOrder(stupidShowOrders))
                return;
            if(!stupidCheckShowOffer.Skip)
                return;
        }
        
        bool showed = GetSomeThingShow();
    }

    protected BasePopup _preShowPopup;

    public void SetPreShowPopup(int inScreenPopupEnum)
    {
        _preShowPopup = ScreenRoot.popupManager.GetPopup(inScreenPopupEnum);
    }
    
    public void SetPreShowPopup(string popupName)
    {
        _preShowPopup = ScreenRoot.popupManager.GetPopup(popupName);
    }
    
    public enum ShowOrder
    {
        CustomScript,
        Subscription,
        Offer,
        Base,
        Navigation,
        Popup,
        Quest,
        BaseNavi,
    }
    
    [SerializeField] protected ShowOrder[] showOrders = {
        ShowOrder.Navigation,
        ShowOrder.CustomScript,
        ShowOrder.Subscription,
        ShowOrder.Offer,
        ShowOrder.Base,
        ShowOrder.Popup,
        ShowOrder.Quest,
        ShowOrder.BaseNavi,
    };

    protected virtual bool GetSomeThingShow()
    {
        if (_preShowPopup)
        {
            ScreenRoot.popupManager.ShowPopup(_preShowPopup);
            _preShowPopup = null;
            return true;
        }

        if (GetSomeThingToShowByOrder(showOrders))
            return true;
        
      
        return DefaultGetSomeThingShow();
    }

    public bool GetSomeThingToShowByOrder(ShowOrder[] orders)
    {
        foreach (var showOrder in orders)
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
                    break;
                case ShowOrder.Offer:
                    break;
                case ShowOrder.Base:
                    break;
                case ShowOrder.Navigation:
                    if (ScreenRoot.navigationManager != null && ScreenRoot.navigationManager.IsOnNavigation())
                        return true;
                    if (ScreenRoot.navigationManager != null &&  ScreenRoot.navigationManager.CheckActivate())
                        return true;
                    break;
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
            }
        }

        return false;
    }
    
    protected bool DefaultGetSomeThingShow()
    {
        if (_preShowPopup)
        {
            ScreenRoot.popupManager.ShowPopup(_preShowPopup);
            _preShowPopup = null;
            return true;
        }
        
        if (CheckCustomScript())
            return true;
        if (CheckShowNavi())
            return true;
        if (CheckShowPopup())
            return true;
        if (CheckShowQuest())
            return true;
        
        return false;
    }


    public bool CheckShowNavi() => BaseGameController.CheckShowNavigationDialog();

    public bool CheckShowPopup()
    {
        foreach (var inScreenPopupEnum in checkShowPopups)
        {
            var popup = RootView.rootView.screenRoot.popupManager.GetPopup((int) inScreenPopupEnum);
            if (popup.IsReadyToShow()) // check if popup have dependency that valid to show for each game
            {
                popup.Show();
                return true;
            }
        }

        return false;
    }

    public bool CheckCustomScript()
    {
        foreach (var inScreenPopupEnum in customCheckShow)
        {
            if (inScreenPopupEnum.Check()) // check if popup have dependency that valid to show for each game
            {
                return true;
            }
        }

        return false;
    }

    
    public bool CheckShowQuest()
    {
        foreach (var genericQuestItemView in checkQuest)
        {
            genericQuestItemView.UpdateStatus();
            if (genericQuestItemView.ReadyToTake())
            {
                DuongExtensions.ExecuteAfterInterstitialClosed(genericQuestItemView.GetReward);
                return true;
            }
        }
        return false;
    }



    private void LogScreen()
    {
        var fireBase2 = Kernel.Resolve<FireBaseController>();
        var parameters2 = fireBase2.GetAdmobParameter(10, new[]
        {
            new LogParameter("flow_seq ", "6"),
            new LogParameter("ad_placement ", ScreenName),
        }, AdTypeLog.interstitial).ToArray();
        if (Kernel.Resolve<FireBaseController>().Initialized)
            fireBase2.LogEvent("admob_show_screen", parameters2);
    }


    public virtual void OnPopupShow()
    {
        OnPopupHideShow(true, popupAnimsShowInstant);
    }

    // to call Show directionally 
    public void ShowThis(bool transition = true)
    {
        ScreenRoot.Show(this, transition);
    }

    public virtual void InvokeActionBeforeShow()
    {
        foreach (var beforeShowAction in beforeShowActions)
            beforeShowAction.StartAction(0);
    }

    [SerializeField] private bool ignoreRelease;

    public bool IgnoreRelease => ignoreRelease;

    public void Aquired(Transform transform1, bool b, bool b1)
    {
        if(!ignoreRelease)
            transform.SetParentAndExpand(transform1, b, b1);
        else
        {
            if (b) ;
                transform.SetAsFirstSibling();
            gameObject.SetActive(b1);
        }
    }

    public virtual void Release(bool activeState = false)
    {
        if(!ignoreRelease)
            transform.Release(activeState);
        else
            gameObject.SetActive(false);
    }

    public void TurnCanvas(bool isOn)
    {
        GetComponent<Canvas>().enabled = isOn;
    }
}