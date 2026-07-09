using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ScreenPosition
{
    Coin,
}

public class AssetLoadingAction
{
    public float Time;
    public Action Action;
}

[Serializable]
public class ScreenRootSetting
{
    public Quantity[] showAddCurrencyDefaultIf;
}

public class ScreenRoot : BaseFrameworkView
{
    public static bool LoadFromIntroScene;
    [SerializeField] private ScreenRootSetting setting;
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public Camera playerCam;
    public LoadingScreen loadingScreen;
    public PlayScreen playScreen;
    public NavigationManager navigationManager;
    public FocusUI focusUi;
    public PopupManager popupManager;
    public DialogController dialogController;
    public TutorialController tutorialController;
    [SerializeField] private BaseScreen[] screens;
    [SerializeField] private BaseScreen startScreen;
    [SerializeField] private PopMessageParent popMessageParent;
    [SerializeField] private BaseRxAnimator screenTransition;
    [SerializeField] private BaseRxAnimator[] otherTransitions;
    [SerializeField] private ToggleScript[] topBar;

    [IndexAsEnum(BuiltInEnumType.ActionLink)] [SerializeField]
    private IntActionScript[] actionLinks;

    [SerializeField] private Activator onSceneStartLoading;
    [SerializeField] private Activator onSceneFinishLoading;
    [SerializeField] public EventSystem eventSystem;
    [SerializeField] private RectTransform _rtCanvas;
    public Vector2 CanvasSize => _rtCanvas.sizeDelta;

    public RectTransform RtCanvas
    {
        get
        {
            if (_rtCanvas == null)
                return GetComponent<RectTransform>();
            return _rtCanvas;
        }
    }

    private BaseScreen _currentScreen;
    public BaseScreen CurrentScreen => _currentScreen;

    public bool isPlayScreen;

    public void OnSceneStartLoading()
    {
        onSceneStartLoading.OnActivate();
//        onSceneFinishLoading.OnDeactivate();
    }

    public void OnSceneFinishLoading()
    {
        onSceneFinishLoading.OnActivate();
//        onSceneStartLoading.OnDeactivate();
    }

    protected override void Register()
    {
        base.Register();
        if (_rtCanvas == null)
            _rtCanvas = canvas.GetComponent<RectTransform>();
        foreach (var baseRxAnimator in otherTransitions)
            baseRxAnimator.CheckRegister();
        screenTransition.CheckRegister();
    }

    public BaseRxAnimator ScreenTransition => screenTransition;

    private void SetDefaultScaler()
    {
        var csbr = canvas.GetComponent<CanvasScalerByResolution>();
        if (csbr != null)
            csbr.Apply();
    }

    public void ApplyCanvasScaler(CanvasScalerByResolution inject)
    {
        if (inject != null)
            inject.ApplyInject(canvasScaler);
        else
            SetDefaultScaler();
    }


    [ContextMenu("ShowCanvasSize")]
    void ShowCanvasSize()
    {
        _rtCanvas = canvas.GetComponent<RectTransform>();
        Debug.Log(_rtCanvas);
    }

    public void Setup()
    {
        popupManager.CheckRegister();

#if UNITY_EDITOR
        for (var index = 0; index < screens.Length; index++)
        {
            var baseScreen = screens[index];
            if (baseScreen == null)
            {
                Debug.LogError($"screen{index} is null");
                Debug.Break();
            }
        }

        if (!screens.Contains(loadingScreen))
        {
            Debug.LogError($"screens not contains loading");
            Debug.Break();
        }
#endif

        foreach (var item in screens)
        {
            if (item == null)
                continue;
            Register(item);
            item.Release();
        }

        if (loadingScreen != null && !LoadFromIntroScene)
        {
            Show(loadingScreen);
            loadingScreen.StartLoad(true);
        }


        if (popMessageParent != null)
            popMessageParent.Register();
        SubjectController.MessageBroadcastingEvent.Subscribe(data =>
        {
            Debug.Log(data.message);
            popMessageParent.Show(data.index, data.message);
        });

        SubjectController.ProductSystemActionEvent
            .Where(x => ((ProductSystemAction) x.index) == ProductSystemAction.NoAds)
            .Subscribe(data =>
            {
                Kernel.Resolve<AdsManager>().EnableNoAds();
                PlayerData.noAdsForever = true;
                PlayerData.Save();
                dialogController.ShowMessage("Congratulations! no more ads displayed");
            });

        SubjectController.ProductSystemActionEvent
            .Where(x => ((ProductSystemAction)x.index) == ProductSystemAction.NoAds24h)
            .Subscribe(data =>
            {
                Kernel.Resolve<AdsManager>().EnableNoAds();
                PlayerData.noAds24h = true;
                PlayerData.Save();
                dialogController.ShowMessage("Congratulations! no more ads displayed");

                RefreshNoAds24hStatus();
            });

        RefreshNoAds24hStatus();
    }

    [SerializeField] private bool deleteLoadingScreen;

    private static readonly Subject<bool> LoadingScreenLoaded = new Subject<bool>();
    private static bool _finishLoadingFromScene;

    public static void OnFinishScreenLoading()
    {
        Debug.Log("finish loading");
        LoadingScreenLoaded.OnNext(true);
        LoadingScreenLoaded.Dispose();
        _finishLoadingFromScene = true;
    }

    public override void Start()
    {
        base.Start();
        playScreen.CheckRegister();
        if (!dialogController.gameObject.activeSelf)
            Debug.LogError("dialogController must be active");
        if (!popupManager.gameObject.activeSelf)
            Debug.LogError("popupManager must be active");

        if (tutorialController != null)
            tutorialController.FocusButton(null);

        if (LoadFromIntroScene && _finishLoadingFromScene)
            StartCoroutine(WaitKernelLoaded());
        else
            LoadingScreenLoaded.Subscribe(data => { StartCoroutine(WaitKernelLoaded()); });
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
    }

    private IEnumerator WaitKernelLoaded()
    {
        while (!Kernel.IsReady())
            yield return null;
        BaseGameController.OnKernelLoadedAndFinishLoadingScreen();
        UpdateSize();
        if (deleteLoadingScreen && loadingScreen != null) // delete unused loading in main scene
            DuongExtensions.ExecuteInNextFrame(() =>
            {
                for (var i = 0; i < screens.Length; i++)
                    if (screens[i] == loadingScreen)
                        screens[i] = null;
                Destroy(loadingScreen.gameObject);
            });
    }

//    private bool _loadingChecked;


    public override void LogCustom()
    {
        base.LogCustom();
    }

    private void RefreshNoAds24hStatus()
    {
        //Check Noads 24h
        var now = DateTime.Now;
        TimeSpan difference = PlayerData.CustomDateTime[3].Subtract(now);
        difference = difference.Add(TimeSpan.FromSeconds(1));
        var remain = (int)difference.TotalSeconds;

        Debug.Log("Remain time no ads: " + remain);

        if (remain <= 0 && PlayerData.noAds24h && !PlayerData.noAdsForever)
        {
            Kernel.Resolve<AdsManager>().DisableNoAds();
        }

        if (PlayerData.noAds24h && remain > 0)
        {
            DOVirtual.DelayedCall(remain, () => 
            {
                Kernel.Resolve<AdsManager>().DisableNoAds();
                Resolve<PlayScreen>().RefreshNoAdsButton();
                Resolve<HomeScreen>().RefreshNoAdsButton();
            });
        }
    }

    public void UpdateSize()
    {
        _rtCanvas = canvas.GetComponent<RectTransform>();
    }

    public void ShowStartScreen()
    {
        if (startScreen != null)
            Show(startScreen, false);
    }

    private readonly Dictionary<Type, BaseScreen> _dict = new Dictionary<Type, BaseScreen>();

    private void Register(BaseScreen screen)
    {
        _dict[screen.GetType()] = screen;
    }

    public T Show<T>(bool transition = false) where T : BaseScreen
    {
        var screen = Resolve<T>();
        return Show(screen, transition);
    }


    public T Show<T, T2>(bool transition = false) where T : BaseScreen where T2 : BasePopup
    {
        var screen = Resolve<T>();
        var show = Show(screen, transition);
        popupManager.ShowPopup<T2>();
        return show;
    }


    public T Show<T>(T screen, bool transition = false) where T : BaseScreen
    {
        Kernel.Resolve<AdsManager>().SwitchBanner(false);
        PlayerData.CheckAndTurnOnSave();
        // show instant
        if (!transition || IsInTransition)
            return ShowInstant(screen);
        return ShowTransition(screen);
    }

    [SerializeField] private Transform container;

    private T ShowInstant<T>(T screen) where T : BaseScreen
    {
        if (_currentScreen != null)
        {
            SubjectController.ScreenClosed.OnNext(_currentScreen);
            _currentScreen.OnHideHandler();
        }

        foreach (var baseScreen in screens)
        {
            if (baseScreen == null)
                continue;
            var active = baseScreen == screen;
            if (active)
                baseScreen.InvokeActionBeforeShow();
            baseScreen.gameObject.SetActive(active);
            if (!active)
                baseScreen.Release();
            else
                baseScreen.Aquired(container != null ? container : transform, true, true);
        }

        if (IsInTransition)
            screenTransition.IO_AnimatorOut()
                .Subscribe(_ =>
                {
                    TurnRayCast(true);
                    IsInTransition = false;
                });
        else
        {
            TurnRayCast(true);
        }

        popupManager.CloseAllPopUpInstant();
        dialogController.CloseAll();
        HandlerScreenShow(screen);
        screen.OnShowHandler();
        isPlayScreen = screen is PlayScreen;
        foreach (var sc in BaseGameController.OnPlayScreens)
            sc.OnChanged(isPlayScreen);
        _currentScreen = screen;
        CheckTopBar();
        SubjectController.ScreenShowed.OnNext(screen);
        1f.Timer(UpdateSize);
        return screen;
    }

    public void HandlerScreenShow(BaseScreen screen)
    {
        HandlerRenderModeChange(screen.RenderModeChange);
    }

    [SerializeField] private RenderMode defaultRenderMode = RenderMode.ScreenSpaceCamera;

    private void HandlerRenderModeChange(CanvasRenderModeChange change)
    {
        switch (change)
        {
            case CanvasRenderModeChange.NoChanged:
                break;
            case CanvasRenderModeChange.Overlay:
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case CanvasRenderModeChange.Camera:
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                break;
            case CanvasRenderModeChange.WorldSpace:
                canvas.renderMode = RenderMode.WorldSpace;
                break;
            case CanvasRenderModeChange.Default:
                canvas.renderMode = defaultRenderMode;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(change), change, null);
        }
    }


    public void CheckTopBar()
    {
        topBar.OnChanged((CurrentScreen != null && CurrentScreen.TopBar) || popupManager.HasTopBar());
    }

    public void UnloadCurrentScreen()
    {
        TurnRayCast(false);
        Transition(true).Subscribe();
    }

    public IObservable<Unit> IoUnloadScreen(Action afterUnload)
    {
        TurnRayCast(false);
        return screenTransition.IO_AnimatorIn() // transition fade in
            .SelectMany(() => IeUnloadScreen(afterUnload));
    }


    public bool IsInTransition { get; set; }

    private T ShowTransition<T>(T screen, bool reopen = false) where T : BaseScreen
    {
        if (screen != _currentScreen)
        {
            var current = _currentScreen;
            TurnRayCast(false);
            if (current != null)
            {
                IsInTransition = true;
                screenTransition.IO_AnimatorIn() // transition fade in
                    .SelectMany(() => IeScreenChange(screen, current))
                    .SelectMany(screenTransition.IO_AnimatorOut()) // transition fade out
                    .Subscribe(_ =>
                    {
                        TurnRayCast(true);
                        IsInTransition = false;
                    });
            }
            else
            {
                ShowInstant(screen);
            }

            _currentScreen = screen;
            CheckTopBar();
            return screen;
        }

        if (reopen)
        {
            var current = _currentScreen;
            TurnRayCast(false);
            if (current != null)
            {
                IsInTransition = true;
                screenTransition.IO_AnimatorIn() // transition fade in
                    .SelectMany(() => Reopen(screen))
                    .SelectMany(screenTransition.IO_AnimatorOut()) // transition fade out
                    .Subscribe(_ =>
                    {
                        TurnRayCast(true);
                        IsInTransition = false;
                    });
            }
            else
            {
                ShowInstant(screen);
            }
        }

        return screen;
    }


    private IEnumerator Reopen(BaseScreen screen)
    {
        if (false)
            yield return null;
        if (screen != null)
        {
            SubjectController.ScreenClosed.OnNext(screen);
            screen.OnHideHandler();
        }

        screen.InvokeActionBeforeShow();
        popupManager.CloseAllPopUpInstant();
        dialogController.CloseAll();
        HandlerScreenShow(screen);
        screen.OnShowHandler();
        CheckTopBar();
        SubjectController.ScreenShowed.OnNext(screen);
        1f.Timer(UpdateSize);
    }

    public IObservable<Unit> Transition(bool inOrOut)
    {
        if (inOrOut)
            return screenTransition.IO_AnimatorIn();

        TurnRayCast(true);
        return screenTransition.IO_AnimatorOut();
    }

    public IObservable<Unit> Transition(bool inOrOut, int other)
    {
        if (inOrOut)
            return otherTransitions[other].IO_AnimatorIn();

        TurnRayCast(true);
        return otherTransitions[other].IO_AnimatorOut();
    }

    public BaseScreen ShowScreen(int key, bool transition)
    {
        var screen = Resolve(key);
        return Show(screen, transition);
    }

    public BaseScreen ShowScreen(string screenName, bool transition)
    {
        var screen = Resolve(screenName);
        return Show(screen, transition);
    }


    public BaseScreen Resolve(int screenEnum)
    {
        var screen = BaseGameController.GetScreen(screenEnum);
        if (screen != null)
            return screen;

        switch ((ScreenEnum) screenEnum)
        {
            case ScreenEnum.Home:
                return Resolve<HomeScreen>();
            case ScreenEnum.Loading:
                return Resolve<LoadingScreen>();
            case ScreenEnum.PlayScreen:
                return Resolve<PlayScreen>();
            case ScreenEnum.LevelSelect:
                return Resolve<LevelSelectScreen>();
            case ScreenEnum.ByName:
                throw new ArgumentOutOfRangeException(nameof(screenEnum), screenEnum, null);
            case ScreenEnum.ShopScreen:
                return Resolve<ShopScreen>();
            default:
                throw new ArgumentOutOfRangeException(nameof(screenEnum), screenEnum, null);
        }
    }

    public BaseScreen Resolve(string screenName)
    {
        foreach (var keyValuePair in _dict)
        {
            if (keyValuePair.Key.Name == screenName)
                return keyValuePair.Value;
        }

        return null;
    }

    private IEnumerator IeUnloadScreen(Action onUnload)
    {
        IsInTransition = true;
        if (false)
            yield return null;
        foreach (var baseScreen in screens)
        {
            if (baseScreen == null)
                continue;
            baseScreen.gameObject.SetActive(false);
            baseScreen.Release();
        }

        if (_currentScreen != null)
        {
            SubjectController.ScreenClosed.OnNext(_currentScreen);
            _currentScreen.OnHideHandler();
            _currentScreen = null;
        }

        popupManager.CloseAllPopUpInstant();
        dialogController.CloseAll();
        isPlayScreen = false;
        RootView.rootView.gameController.OnPlayScreens.OnChanged(false);
        CheckTopBar();
        onUnload.Invoke();
    }

    private IEnumerator IeScreenChange(BaseScreen screen, BaseScreen oldScreen)
    {
        if (false)
            yield return null;
        foreach (var baseScreen in screens)
        {
            if (baseScreen == null)
                continue;
            var active = baseScreen == screen;
            if (active)
                baseScreen.InvokeActionBeforeShow();
            baseScreen.gameObject.SetActive(active);
            if (!active)
                baseScreen.Release();
        }

        if (oldScreen != null)
        {
            SubjectController.ScreenClosed.OnNext(oldScreen);
            oldScreen.OnHideHandler();
        }

        screen.transform.SetParentAndExpand(transform, true, true);
        popupManager.CloseAllPopUpInstant();
        dialogController.CloseAll();
        isPlayScreen = screen is PlayScreen;
        HandlerScreenShow(screen);
        screen.OnShowHandler();


        CheckTopBar();

        bool screenIsPlayScreen = screen is PlayScreen;
        RootView.rootView.gameController.OnPlayScreens.OnChanged(screenIsPlayScreen);
        SubjectController.ScreenShowed.OnNext(screen);
        1f.Timer(UpdateSize);
    }

    public void AccquiredScreen(BaseScreen screen)
    {
        screen.transform.SetParentAndExpand(transform, true, true);
    }

    public T Resolve<T>() where T : BaseScreen
    {
        return (T) _dict[typeof(T)];
    }

    public Vector2 WorldObjectScreenPosition(Vector3 worldPosition)
    {
        Vector2 viewportPosition = playerCam.WorldToViewportPoint(worldPosition);
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * Screen.width)), // - (CanvasSize.x * 0.5f)),
            ((viewportPosition.y * Screen.height))); //- (CanvasSize.y * 0.5f)));

        return worldObjectScreenPosition;
    }

    public Vector2 WorldObjectToViewPort(Vector3 worldPosition)
    {
        return playerCam.WorldToViewportPoint(worldPosition);
    }

    public RectTransform GetPosition(ScreenPosition screenPosition)
    {
        switch (screenPosition)
        {
            case ScreenPosition.Coin:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(screenPosition), screenPosition, null);
        }

        return null;
    }

    public LocationManager GetLocationManager()
    {
        for (var i = popupManager.levels.Length - 1; i >= 0; i--)
        {
            var level = popupManager.levels[i];
            if (level.CurrentPopup != null && level.CurrentPopup.LocationManager != null)
                return level.CurrentPopup.LocationManager;
        }

        return _currentScreen.GetLocationManager();
    }

    public IEnumerable<LocationManager> GetCurrentLocationManagers()
    {
        for (var i = popupManager.levels.Length - 1; i >= 0; i--)
        {
            var level = popupManager.levels[i];
            if (level.CurrentPopup != null && level.CurrentPopup.LocationManager != null)
                yield return level.CurrentPopup.LocationManager;
        }
    }

    public RectTransform GetLocation(LocationEnum key)
    {
        var location = GetLocationManager();
        if (location == null)
            return null;
        var rt = location.GetLocation(key);
        if (rt != null)
            return rt;

        if (_currentScreen.GetLocationManager() != null)
            return _currentScreen.GetLocationManager().GetLocation(key);

        return null;
    }


    public void ShowReward(ShowRewardInput input, Action afterClaim = null)
    {
        if ((input.rewardTypeFlag & (int) RewardTypeFlag.ClaimQuietly) != 0)
        {
            if (input.closeCurrentPopup)
                popupManager.CloseCurrentPopupAsObservable().Subscribe();
            input.trade.Spend();
            PlayerData.Claim(input.trade.Products);
            PlayerData.Save();
            DailyData.Save();
            afterClaim?.Invoke();
        }
        else if ((input.rewardTypeFlag & (int) RewardTypeFlag.ClaimInDialogAddCurrency) != 0)
        {
            if (input.closeCurrentPopup)
                popupManager.CloseCurrentPopupAsObservable().Subscribe(data => ShowWithAddCurrencyDialog());
            else
                ShowWithAddCurrencyDialog();

            void ShowWithAddCurrencyDialog()
            {
                if (RootView.rootView.screenRoot.dialogController.ShowReward(input.trade.Product))
                {
                    input.trade.Spend();
                    afterClaim?.Invoke();
                }
                else
                    ShowRewardDefault(input, afterClaim);
            }
        }
        else if ((input.rewardTypeFlag & (int) RewardTypeFlag.CustomClaimScript) != 0)
        {
            if (input.closeCurrentPopup)
                popupManager.CloseCurrentPopupAsObservable().Subscribe(data => Show2());
            else
                Show2();

            void Show2()
            {
                input.CustomClaimScript.Handler(input);
                afterClaim?.Invoke();
            }
        }
        else if ((input.rewardTypeFlag & (int) RewardTypeFlag.ClaimInstantInPopup) != 0)
        {
            if (input.closeCurrentPopup)
                popupManager.CloseCurrentPopupAsObservable()
                    .Subscribe(data => ShowWithPopupShowReward(input, afterClaim));
            else
                ShowWithPopupShowReward(input, afterClaim);
        }
        else
        {
            ShowRewardDefault(input, afterClaim);
        }

        DuongExtensions.ExecuteInNextFrame(() =>
        {
            DuongDataHelper.JsonPreSavePlayerDataSpendClaim(input.trade.Prices, input.trade.Products);
            PlayerData.IgnoreSaveUntilTurnOn();
        });
    }


    private void ShowRewardDefault(ShowRewardInput input, Action afterClaim)
    {
        if (setting.showAddCurrencyDefaultIf.Contains(input.trade.Product.Quantity))
        {
            if (input.closeCurrentPopup)
                popupManager.CloseCurrentPopupAsObservable().Subscribe(data => ShowWithAddCurrencyDialog());
            else
                ShowWithAddCurrencyDialog();

            void ShowWithAddCurrencyDialog()
            {
                if (RootView.rootView.screenRoot.dialogController.ShowReward(input.trade.Product))
                {
                    input.trade.Spend();
                    afterClaim?.Invoke();
                }
                else
                    ShowRewardDefault(input, afterClaim);
            }
        }
        else
        {
            input.rewardTypeFlag = (int) RewardTypeFlag.ClaimInstantInPopup;
            ShowWithPopupShowReward(input, afterClaim);
        }
    }

    private void ShowWithPopupShowReward(ShowRewardInput input, Action afterClaim)
    {
        if (input.trade.IsMultiReward && popupManager.Resolve<PopupShowRewards>() != null)
        {
            if (input.closeCurrentPopup)
                popupManager.ShowPopup<PopupShowRewards>(-1).SetData(input, afterClaim);
            else
                popupManager.ShowPopupNext<PopupShowRewards>().SetData(input, afterClaim);
        }
        else
        {
            if (input.showRewardAsNext)
                popupManager.ShowPopupNext<PopupShowReward>().SetData(input, afterClaim); 
            else if (input.closeCurrentPopup)
                popupManager.ShowPopup<PopupShowReward>(-1).SetData(input, afterClaim);
            else
                popupManager.ShowPopupNext<PopupShowReward>().SetData(input, afterClaim);
        }
    }

    public void LoadingAssets(AssetLoadingAction[] actions)
    {
        Observable.FromCoroutine(() => IEAssetLoading(actions)).Subscribe();
    }

    private IEnumerator IEAssetLoading(AssetLoadingAction[] actions)
    {
        loadingScreen.isActive.Value = true;
        if (_currentScreen != null)
            _currentScreen.gameObject.SetActive(false);

        var property = new FloatReactiveProperty();
        loadingScreen.InjectProgress(property);
        float totalTime = actions.Sum(x => x.Time);
        float deltaTime = 0;

        yield return new WaitForSeconds(0.25f);

        foreach (var assetLoadingAction in actions)
        {
            while (assetLoadingAction.Time > 0)
            {
                assetLoadingAction.Time -= Time.deltaTime;
                deltaTime += Time.deltaTime;
                property.Value = deltaTime / totalTime;

                if (assetLoadingAction.Time <= 0)
                {
                    assetLoadingAction.Action?.Invoke();
                }

                yield return null;
            }
        }

        property.Dispose();
        loadingScreen.isActive.Value = false;
    }

    private string _lastScreenName;

    public static string CurrentScreenName;

    public void CheckLogScreen()
    {
        CurrentScreenName = null;
        var popup = popupManager.GetCurrentPopup();
        if (popup != null && !popup.SkipLog)
            CurrentScreenName = popup.PopupName;
        else if (CurrentScreen != null)
            CurrentScreenName = CurrentScreen.ScreenName;

        if (CurrentScreenName != null && _lastScreenName != CurrentScreenName)
        {
            _lastScreenName = CurrentScreenName;
            new SonatLogScreenView()
            {
                screen_name = _lastScreenName
            }.Post();

            UIDebugLog.Log0("LogScreen " + _lastScreenName);
        }
    }


    public bool AnyPopupOrDialogShow()
    {
        return popupManager.AnyPopupShow && dialogController.AnyDialogShow;
    }

    public void TurnGraphicRaycaster(bool b)
    {
        CurrentScreen.TurnGraphicRaycast(b);
        if (popupManager.GetCurrentPopup() != null)
            popupManager.GetCurrentPopup().TurnGraphicRaycast(b);
    }

    public void TurnRayCast(bool isOn)
    {
        eventSystem.enabled = isOn;
    }

    public bool IsEventSystemEnable => eventSystem.enabled;

    public void ReopenCurrentScene()
    {
        ShowTransition(_currentScreen, true);
    }

    public void InvokeActionLink(int actionLink)
    {
        foreach (var intActionScript in actionLinks)
            if (intActionScript.value == actionLink)
            {
                intActionScript.value2.StartAction();
                return;
            }

        Debug.LogError("not found");
    }


    public void LocationClick(BaseFocusLocation location)
    {
        if (navigationManager != null)
            navigationManager.LocationClick(location);
    }

    public void LocationDispose(BaseFocusLocation location)
    {
        if (navigationManager != null)
            navigationManager.LocationDispose(location);
    }
}