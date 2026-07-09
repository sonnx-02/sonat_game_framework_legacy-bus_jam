using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum WhenPopupClose
{
    Close,
    OpenLastDialog,
    OpenLastPopup
}

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePopup : BaseFrameworkView
{
    [MyButton(nameof(Show), nameof(Close), nameof(Load))] [SerializeField]
    // ReSharper disable once NotAccessedField.Local
    private int testShow;
    
    [SerializeField] private LocationManager locationManager;
    [FormerlySerializedAs("resumeIfHome")] [SerializeField]
    private bool resumeIfPlayScreen = false;
    [SerializeField] public bool nativeAds;
    [SerializeField] private Button close;
    [SerializeField] private bool hasTopBar;

    [SerializeField] private BaseAdsRewardButton adsToLog;
    [SerializeField] private bool log;
    [SerializeField] private Activator onShowActivator; 
    [SerializeField] private BaseBaseActionScript[] onShowScripts; 
    [SerializeField] private BaseBaseActionScript[] onCloseScripts; 
//    [SerializeField] private BaseBaseActionScript[] beforeShowActions;
    [SerializeField] private BaseRxAnimator[] animators; // wait animator
    [SerializeField] private BaseRxAnimator[] unWaitAnimators; // can stop anytime
    [SerializeField] private ToggleScript[] activeOnShow;
    [HideInInspector]
    [SerializeField] private BaseChecker[] checkers;
    [SerializeField] private BasePopupDependency[] dependencies;
    [SerializeField] private BaseScreenDependency[] screenDependencies;
    [SerializeField] private BoolBool stopTime = new BoolBool(false, false);
    [SerializeField] private bool skipLog;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private string popupName; // to log screen
    [SerializeField] private bool pauseMusic;
    [SerializeField] private IndexBindingScriptBase[] bindParameter0;
    [SerializeField] private IndexBindingScriptBase[] bindParameter1;
    [SerializeField] private IndexBindingScriptBase[] bindParameter2;
    public string PopupName => (string.IsNullOrEmpty(popupName) ? GetType().Name.ToLower() : popupName);
    public LocationManager LocationManager => locationManager;
    public bool SkipLog => skipLog;
    public bool PauseMusic => pauseMusic;
 
    public int Level { get; set; } // Popup Level In Popup Manager, Set by PopupManager
    private WhenPopupClose _whenClose; // set action when popup Close
    
    private void Load()
    {
        animators = transform.GetComponentsInFirstBornChildren<BaseRxAnimator>().ToArray();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    protected override void Register()
    {
        base.Register();
#if UNITY_EDITOR
        if (checkers != null && checkers.Length > 0)
        {
            Debug.LogError("not using now, use on show or before show BaseBaseActionScript");
            Debug.Break();
        }
#endif

        foreach (var baseRxAnimator in animators)
            baseRxAnimator.CheckRegister();
        if (close != null)
            close.onClick.AddListener(HandlerCloseClick);

        foreach (var basePopupDependency in dependencies)
            basePopupDependency.SetParent(this);

        foreach (var baseRxAnimator in animators)
            baseRxAnimator.CheckRegister();
        foreach (var baseRxAnimator in unWaitAnimators)
            baseRxAnimator.CheckRegister();

        if (graphicRaycaster == null)
            graphicRaycaster = GetComponent<GraphicRaycaster>();
    }

    public virtual void SetParameter(CustomParameter parameter)
    {
        if (parameter != null)
        {
            bindParameter0.OnChanged(parameter.intValue);
            bindParameter1.OnChanged(parameter.intValue2);
            bindParameter2.OnChanged(parameter.intValue3);
        }
    }

    private bool _onShowed;
    public void Show()
    {
        ScreenRoot.popupManager.ShowPopup(this);
        ScreenRoot.CheckLogScreen();
        TurnGraphicRaycast(true);
    }

    [SerializeField] private bool closeInstant;
    // ReSharper disable once MemberCanBeProtected.Global
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public virtual void Close()
    {
        if(!closeInstant)
            ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe(data =>
            {
    //            Kernel.Resolve<AdsManager>().SwitchBanner(false);
            });
        else
            ScreenRoot.popupManager.CloseCurrentInstant();

        if (log && adsToLog != null)
        {
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(10, new[]
            {
                new LogParameter("flow_seq ", "9"),
                new LogParameter("ad_placement ", BaseGameController.GetCurrentPlacement()),
            }, AdTypeLog.interstitial).ToArray();
            fireBase2.LogEvent("admob_cancel_watch", parameters2);
        }

        if (this is PopupRateUs)
        {
            new SonatLogShowRate()
            {
                open_by = SonatLogShowRate.ShowRateOpenBy.user,
                action = SonatLogShowRate.ShowRateAction.close
            }.Post();
        }
    }

    public void Close(Action afterClose)
    {
        ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe(data =>
        {
//            Kernel.Resolve<AdsManager>().SwitchBanner(false);
            afterClose.Invoke();
        });

        if (log && adsToLog != null)
        {
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(10, new[]
            {
                new LogParameter("flow_seq ", "9"),
                new LogParameter("ad_placement ", BaseGameController.GetCurrentPlacement()),
            }, AdTypeLog.interstitial).ToArray();
            fireBase2.LogEvent("admob_cancel_watch", parameters2);
        }

        if (this is PopupRateUs)
        {
            new SonatLogShowRate()
            {
                open_by = SonatLogShowRate.ShowRateOpenBy.user,
                action = SonatLogShowRate.ShowRateAction.close
            }.Post();
        }
    }

    protected virtual void HandlerCloseClick()
    {
        #if UNITY_EDITOR
        if(resumeIfPlayScreen)
            Debug.LogError("close as Resume");
        #endif
        if (_whenClose == WhenPopupClose.OpenLastPopup)
        {
            _whenClose = WhenPopupClose.Close;
            ScreenRoot.popupManager.ReopenLastPopup();
        }
        else if (ScreenRoot.isPlayScreen && resumeIfPlayScreen)
            SubjectController.GameActionEvent.OnNext(GameActionEvent.Resume);
        else
            ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe();
    }

    private Action _onClose;

    public virtual void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        CheckRegister();
        if (Level == 0 && stopTime.value && !Pause.Value)
            Time.timeScale = 0f;

        _whenClose = whenPopupClosed;
        _onClose = null;
        foreach (var baseScreenDependency in dependencies)
        {
            baseScreenDependency.CheckRegister();
            baseScreenDependency.OnShow();
        }
        foreach (var baseScreenDependency in screenDependencies)
        {
            baseScreenDependency.CheckRegister();
            baseScreenDependency.OnShow();
        }
        foreach (var toggleScript in activeOnShow)
            toggleScript.OnChanged(true);
        foreach (var baseScreenChecker in checkers)
            baseScreenChecker.Check();
        onShowScripts.Prepare();
        onShowScripts.StartAction(0);
        onShowActivator.OnActivate();
        
        Kernel.Resolve<AdsManager>().SwitchBanner(nativeAds);

        if (log && adsToLog != null)
        {
            var fireBase2 = Kernel.Resolve<FireBaseController>();
            var parameters2 = fireBase2.GetAdmobParameter(10, new[]
            {
                new LogParameter("flow_seq ", "7"),
                new LogParameter("ad_placement ", BaseGameController.GetCurrentPlacement()),
            }, AdTypeLog.interstitial).ToArray();
            fireBase2.LogEvent("admob_enter_scene", parameters2);
        }
        
        SubjectController.OnPopupShowed.OnNext(this);
        _onShowed = true;
    }

    public void SetActionWhenClose(Action onClose)
    {
        _onClose = onClose;
    }
    
//    public virtual void InvokeActionBeforeShow()
//    {
//        foreach (var beforeShowAction in beforeShowActions)
//            beforeShowAction.StartAction();
//    }


    protected virtual void CloseHandler()
    {
        if (Level == 0 && stopTime.value && !Pause.Value)
            Time.timeScale = 1;

        transform.Release();
        gameObject.SetActive(false);
        switch (_whenClose)
        {
            case WhenPopupClose.Close:
                break;
            case WhenPopupClose.OpenLastDialog:
                ScreenRoot.dialogController.ReopenLastDialog();
                break;
            case WhenPopupClose.OpenLastPopup:
                ScreenRoot.popupManager.ReopenLastPopup();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_onClose != null)
        {
            _onClose.ExecuteInNextFrame();
            _onClose = null;
        }

        if (_onShowed)
        {
            SubjectController.OnPopupClosed.OnNext(this);
            _onShowed = false;
        }

        if(onCloseScripts.Length > 0)
            DuongExtensions.ExecuteInNextFrame(() => { onCloseScripts.StartAction(); });
    }

//    public void Show()
//    {
//        gameObject.SetActive(true);
//    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public virtual IObservable<Unit> ObservableAnim_In(PopupManagerLevel level,bool ignoreToggle = false)
    {
        CheckRegister();
        foreach (var toggleScript in level.toggles)
            toggleScript.OnChanged(!ignoreToggle);
        gameObject.SetActive(true);
        foreach (var baseRxAnimator in animators)
            baseRxAnimator.ResetAnim();

        foreach (var unWaitAnimator in unWaitAnimators)
            unWaitAnimator.Animator_In();

        var parallel = Observable.WhenAll(animators.ToList().Select(x => x.IO_AnimatorIn()).ToArray())
            .SelectMany(() => AfterIn(level));
        return parallel;
    }

    protected virtual IEnumerator AfterIn(PopupManagerLevel level)
    {
//        level.CurrentPopup = this;
        if (false)
            yield return null;
    }


    public virtual IObservable<Unit> ObservableAnim_Out(PopupManagerLevel level, bool closeUIblock)
    {
        if (!closeInstant)
        {
            var parallel = Observable.WhenAll(animators.Select(x => x.IO_AnimatorOut()).ToArray())
                .SelectMany(() => AfterOut(level, closeUIblock));
            return parallel;
        }

        CloseInstantThis(level, closeUIblock);
        return Observable.FromCoroutine(Empty);
    }

    private IEnumerator Empty()
    {
        if (false)
            // ReSharper disable once HeuristicUnreachableCode
            yield return null;
    }

    public void CloseInstantThis(PopupManagerLevel level, bool closeUIblock)
    {
        gameObject.SetActive(false);
        CloseHandler();
        if (closeUIblock)
        {
            foreach (var toggleScript in level.toggles)
                toggleScript.OnChanged(false);
        }

        if (level.CurrentPopup == this)
        {
            level.CurrentPopup = null;
            RootView.rootView.screenRoot.CheckLogScreen();
        }

        ScreenRoot.popupManager.CheckAnyPopupShow();
        gameObject.SetActive(false);

        if (nativeAds)
        {
            Kernel.Resolve<AdsManager>().SwitchBanner(true);
            Kernel.Resolve<AdsManager>().SwitchBanner(false);
        }
        else
        {
            Kernel.Resolve<AdsManager>().SwitchBanner(ScreenRoot.popupManager.GetCurrentPopup() != null &&
                                                      ScreenRoot.popupManager.GetCurrentPopup().nativeAds);
        }
    }
    protected virtual IEnumerator AfterOut(PopupManagerLevel level, bool closeUIblock)
    {
        CloseHandler();
        if (closeUIblock)
        {
            foreach (var toggleScript in level.toggles)
                toggleScript.OnChanged(false);
        }

        if (level.CurrentPopup == this)
        {
            level.CurrentPopup = null;
            RootView.rootView.screenRoot.CheckLogScreen();
        }

        ScreenRoot.popupManager.CheckAnyPopupShow();
        gameObject.SetActive(false);

        if (nativeAds)
        {
            Kernel.Resolve<AdsManager>().SwitchBanner(true);
            Kernel.Resolve<AdsManager>().SwitchBanner(false);
        }
        else
        {
            Kernel.Resolve<AdsManager>().SwitchBanner(ScreenRoot.popupManager.GetCurrentPopup() != null &&
                                                      ScreenRoot.popupManager.GetCurrentPopup().nativeAds);
        }
    
        
        if (false)
            yield return null;
    }

    public void TurnGraphicRaycast(bool value)
    {
        if (graphicRaycaster != null)
            graphicRaycaster.enabled = value;
    }
    
    public bool IsReadyToShow()
    {
        foreach (var basePopupDependency in dependencies)
            if (basePopupDependency.ReadyToShow())
                return true;

        return false;
    }


    public T ResolveDependency<T>() where T : BasePopupDependency
    {
        foreach (var basePopupDependency in dependencies)
        {
            if (basePopupDependency.GetType() == typeof(T))
                return (T) basePopupDependency;
        }

        return null;
    }
    
    public T ResolveDependencyAssignFrom<T>() where T : BasePopupDependency
    {
        foreach (var basePopupDependency in dependencies)
        {
            if (basePopupDependency.GetType().IsAssignableFrom(typeof(T)))
                return (T) basePopupDependency;
        }

        return null;
    }
    public bool ignoreBg;

    public bool HasTopBar()
    {
        return hasTopBar;
    }
    
    
    
#if UNITY_EDITOR
    [MyButton(nameof(LoadDependency))] [SerializeField]
    // ReSharper disable once NotAccessedField.Local
    private int loadDependency;
    public void LoadDependency()
    {
        dependencies = GetComponents<BasePopupDependency>();
    }
#endif
}