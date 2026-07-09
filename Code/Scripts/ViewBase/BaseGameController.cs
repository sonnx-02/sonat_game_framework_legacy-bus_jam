using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sonat;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable CollectionNeverUpdated.Global

// ReSharper disable InconsistentNaming
[Serializable]
public class GameControllerBinding
{
    public IndexBindingScriptBase[] gameStateBindings;
    public BaseBaseActionScript[] onBoardLose;
    public BaseBaseActionScript[] onBoardWin;
    public BaseBaseActionScript[] onStartGame;
    public BaseBaseActionScript[] onWin;
    public BaseBaseActionScript[] onGameOver;
    public BaseBaseActionScript[] onQuitGame;
    public BaseFrameworkViewCheck[] continuouslyChecks;
    public BaseRxAnimator[] onWinAnims;
    public ToggleScript[] onPlayScreens;
    public ToggleScript[] boardBusyBinds;
}

//[CreateAssetMenu(fileName = nameof(CurrentGameSetting) + ".asset",
//    menuName = "Duong/" + nameof(CurrentGameSetting))]
public class BaseCurrentGameSetting : ScriptableObject
{
    public int timeToHintIdle = 10;
    public IntInt levelSetFinishTutorial = new IntInt(int.MaxValue, -1);

    [ArrayLabelIndexAsEnum(BuiltInEnumType.UnlockConditionEnum)]
    public int[] levelUnlocks = new int[10];

    public float cooldownCombo = 2;
    public float delayToShowPopupWin = 0;

    [ArrayElementIndexTitleAttribute2(BuiltInEnumType.CurveEnum)]
    public AnimationCurve[] curves;

    public ListProduct[] starAcquiredRewards;
    public ListProduct[] levelFinishRewards;
    public ListProduct[] levelFinishRewardsRandom;

    [FormerlySerializedAs("miniModeReward")]
    public ListProduct[] dailyChallengerRewards;

    public ListProduct[] challengerRewards;
}

[Serializable]
public class GameControllerSetting
{
    public int targetFps = 61;
    public int vSync = 0;
    public bool deleteGameSaveWhenTutorialUnfinished = true;
    public IntBool countDownEnergy = new IntBool(1800, false);
    public int maxEnergy = 5;
    public int startEnergyNewDay = 3;
    public int maxSaveState = 5;
    public ValidScriptBase[] validInputs;
}

public abstract partial class BaseGameController : BaseFrameworkView, ILogParameterValueRetriever
{
    protected BaseGameUI gameUi;
    [SerializeField] protected GameControllerSetting setting;
    [HideInInspector]
    public IntReactiveProperty shopItemPending = new IntReactiveProperty();


    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(GetType());
    }

    [SerializeField] private GameRootStartAction autoStart;
#if UNITY_EDITOR
    [SerializeField] private GameRootStartAction autoStartEditor;
    GameRootStartAction AutoStart => autoStartEditor;
#else
      GameRootStartAction AutoStart => autoStart;
#endif

    // init daily data, ex : have one rotate each new day
    [SerializeField] public Camera cam;
    [SerializeField] private DailyCustomDataSetting[] customDailyDataSetting;

    [IntRxAsEnum(BuiltInEnumType.GameState)]
    public IntReactiveProperty gameState;

    public GameControllerBinding binding;
    public GameEventBinding eventBinding;
    public GameDataProcessing dataProcessing;

    public abstract BaseCurrentGameSetting BaseCurrentGameSetting { get; }
    public abstract BaseLevelDatabaseClass BaseLevelDatabaseClass { get; }
    public bool CreatingLevel { get; set; }


    protected int[] GameSaveDisplayRemap;

    // remap to show selected game save property
    protected IntReactiveProperty[] GameSaveDisplayProperty;

    // do not remember
    protected IntegerSerializableDictionary CustomDisplayProperty = new IntegerSerializableDictionary();

    // input disposer
    protected BoolReactiveProperty BoardBusy = new BoolReactiveProperty();

    // interface for GameTutorial
    protected virtual IActiveTutorialComponents IActiveTutorialComponents => null;
    private IDisposable _inputDisposable;
    public ToggleScript[] OnPlayScreens => binding.onPlayScreens;

    [IndexAsEnum(BuiltInEnumType.MusicEnum)] [SerializeField]
    protected int[] musicOnStartGame = {(int) MusicEnum.GamePlay0};

    [IndexAsEnum(BuiltInEnumType.MusicEnum)] [SerializeField]
    protected int musicOnGameEnd;

    // to stop input from user
    public bool DisableInput { get; set; }

    public bool Stop { get; set; }

    // ReSharper disable once CollectionNeverUpdated.Local
    protected readonly CompositeDisposable startGameCompositeDisposable = new CompositeDisposable();

    // list log name from LogTypes, for get Name to handle logic at base class
    protected virtual BuiltInEnumType[] initLogTypes => new[]
    {
//        BuiltInEnumType.LogPlacement,
        BuiltInEnumType.LogItemType,
        BuiltInEnumType.QuestEnum,
        BuiltInEnumType.ShopItemKey,
        BuiltInEnumType.AdsItemKey,
        BuiltInEnumType.ProductGameAction,
        BuiltInEnumType.ProductSystemAction,
        BuiltInEnumType.SoundEnum,
        BuiltInEnumType.MusicEnum,
        BuiltInEnumType.AchievementEnum,
        BuiltInEnumType.CustomGameCondition,
        BuiltInEnumType.CustomDailyDataProperty,
        BuiltInEnumType.CustomSessionDataProperty,
        BuiltInEnumType.CustomWaveDataProperty,
        BuiltInEnumType.TutorialButtonEnum,
        BuiltInEnumType.EventEnum,
        BuiltInEnumType.NavigationEnum,
        BuiltInEnumType.InScreenPopupEnum,
        BuiltInEnumType.GameActionEvent,
        BuiltInEnumType.GameRootButtonEnum,
        BuiltInEnumType.CustomProductEnum,
        BuiltInEnumType.GameRootButtonEnum,
        BuiltInEnumType.UnlockEquipDataEnum,
        BuiltInEnumType.PlayerDataStock,
        BuiltInEnumType.SubscriptionKey,
        BuiltInEnumType.CustomQuantity,
        BuiltInEnumType.LogPlacement,
        BuiltInEnumType.CustomPlayerDataProperty,
        BuiltInEnumType.CustomTimeTick,
    };


    private IEnumerable<BuiltInEnumTypeValueList> GetEnumValueLists()
    {
        foreach (var builtInEnumType in initLogTypes)
        {
            var valueList = BuiltInEnumTypeValueList.Create(builtInEnumType, NameSpace);
            if (valueList != null)
                yield return BuiltInEnumTypeValueList.Create(builtInEnumType, NameSpace);
        }
    }

    protected abstract string NameSpace { get; }

    public virtual void CheckWon()
    {
        if (PlayerData.won)
        {
            HandlerWon();
            PlayerData.won = false;
            PlayerData.Save();
        }
    }

    public void Check()
    {
#if UNITY_EDITOR
        EnumList enumList = new EnumList(nameof(BuiltInEnumType));
        if (enumList.Found && enumList.Values.Length > 0)
        {
            IList<int> values = enumList.Values.ToList();
            IList<GUIContent> names = enumList.Names.ToList();

            foreach (var guiContent in names)
            {
                EnumList enumList2 = new EnumList(guiContent.text);
                if (enumList2.Found && enumList2.Values.Length > 0)
                {
                    IList<int> names2 = enumList2.Values.ToList();
                    if (names2.HasDuplicate())
                        Debug.LogError("has duplicate on " + guiContent.text);
                }
            }
        }
#endif
    }

    protected override void Register()
    {
        base.Register();
        Check();
        CheckWon();
        LoadOldDataToNewSystem();
        WaveData.Clear();
        SubjectController.GameActionEvent.Subscribe(GameActionEventHandler);
        SubjectController.ProductGameActionEvent.Subscribe(ProductGameActionEventHandler);
        SubjectController.SubscriptionActivated.Subscribe(OnSubscriptionActivate);

        BoardBusy.Subscribe(data =>
        {
            binding.boardBusyBinds.OnChanged(data);
            if (gameUi != null)
                gameUi.BindingUi.boardBusyBinds.OnChanged(data);
        });

        PendingShopItem.Subscribe(data =>
        {
            pendingShopItemKey.Value = data != null ? data.Key() : -1;
        });

        ToBuyShopItem.Subscribe(data =>
        {
            if (data != null)
            {
                //BlockJam.GameController gameCtr = RootView.rootView.gameController as BlockJam.GameController;
                BaseGameController.DoGoStraight();
            }
        });

        RegisterDisplayProperty(GetGameSaveDisplayPropertyRemap());
        BaseGameControllerExtension.SetEnumValueList(GetEnumValueLists().ToArray());
        RegisterCurrentGame();
        UIDebugLog.Log("refreshRate:" + Screen.currentResolution.refreshRate);
        Application.targetFrameRate = Screen.currentResolution.refreshRate < 110 ? 120 : setting.targetFps;
        QualitySettings.vSyncCount = setting.vSync;
    }


    protected virtual void LoadOldDataToNewSystem()
    {
    }

    protected virtual void RegisterBoard()
    {
#if UNITY_EDITOR
        if (setting.maxEnergy == 0)
            Debug.LogError("max energy should not be zero");
        PlayerData.energy.Subscribe(data => Debug.Log("energy" + data));
#endif
        if (setting.countDownEnergy.value2)
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(data =>
                {
                    if (!PlayerData.IsInRange((int) BaseCustomTimeTick.CountDownGiveEnergy) &&
                        PlayerData.energy.Value < setting.maxEnergy)
                    {
                        PlayerData.energy.Value = Mathf.Min(setting.maxEnergy, PlayerData.energy.Value + 1);
                        if (IsEnergyMax())
                            PlayerData.AddCustomTimeTickFromNow((int) BaseCustomTimeTick.CountDownGiveEnergy, 86400);
                        else
                            ResetEnergyCountDown(true);
                    }
                    else if (PlayerData.energy.Value == setting.maxEnergy)
                        ResetEnergyCountDown(false);
                });
        }
    }

    protected bool IsEnergyMax() => PlayerData.energy.Value == setting.maxEnergy;


    protected void ResetEnergyCountDown(bool save = true)
    {
        PlayerData.AddCustomTimeTickFromNow((int) BaseCustomTimeTick.CountDownGiveEnergy, setting.countDownEnergy.value,
            save);
    }


    protected virtual void HandlerWon()
    {
        PlayerData.currentLevel.Value++;
    }

    protected abstract void RegisterCurrentGame();
//    {
//        BasePoolItemGameView<GameController, CurrentGameSetting>.Set(this, currentGameSetting);
//    }

    public abstract void SaveTemp();
    public abstract void LoadTemp();


    private void RegisterDisplayProperty(int[] remap)
    {
        GameSaveDisplayRemap = remap;
        GameSaveDisplayProperty = new IntReactiveProperty[remap.Length];
        for (var i = 0; i < remap.Length; i++)
        {
            GameSaveDisplayProperty[i] = new IntReactiveProperty();
        }
    }

    protected virtual void OnStartANewGame()
    {
        if (Level >= BaseCurrentGameSetting.levelSetFinishTutorial.value && PlayerData.tutorialStep >= 0)
            PlayerData.tutorialStep = BaseCurrentGameSetting.levelSetFinishTutorial.value2;
        PlayerData.playTimes++;
        //  PlayerData.playGameInfo = new PlayGameInfo();
        PlayerData.logEachGame.ClearDict();
        SonatTrackingHelper.TryToPostQueues();
        PlayerData.levelPlayTimes[PlayerData.currentLevel.Value] += 1;

        
#if UNITY_EDITOR
        var value = PlayerData.levelPlayTimes[PlayerData.currentLevel.Value];
        1f.Timer(() =>
        {
            if (value != PlayerData.levelPlayTimes[PlayerData.currentLevel.Value])
            {
                Debug.LogError("Err:  PlayerData.levelPlayTimes++ twice");
            }
        });
#endif
    }

    public abstract void ClearMovesThenRestartGame();
    protected AdsManager _adsManager;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        Input.multiTouchEnabled = false;
        _adsManager = Kernel.ResolveDirty<AdsManager>();
        _adsManager.onInterstitialShow.Action += OnInterstitialShow;
        _adsManager.onVideoRewarded.Action += OnVideoRewarded;
        _adsManager.onPaidAdEvent.Action += OnPaidAdEvent;

        SubjectController.OnPointParameterCallBack.Subscribe(HandlePointParameterCallBack);
        if (!DailyData.gameControllerChecked)
            OnNewDailyData();
        SubjectController.OnDailyDataReset.Subscribe(data => OnNewDailyData());

        CalculateSessionData();
        LoadGameUi();
        gameState.Subscribe(data =>
        {
            binding.gameStateBindings.OnChanged(data);
            if (gameUi != null)
                gameUi.GameStateBindings.OnChanged(data);
        });
        1f.Timer(ValidateStoreProduct);
    }

    protected virtual void OnInterstitialShow(bool on)
    {
        PlayerData.logEachGame[(int) LogEachGameEnum.interstitial_count]++;
        PlayerData.logAllTime[(int) LogAllTimeEnum.interstitial_count]++;
    }

    protected virtual void OnVideoRewarded()
    {
        PlayerData.logEachGame[(int) LogEachGameEnum.video_count]++;
        PlayerData.logAllTime[(int) LogAllTimeEnum.video_count]++;
    }

    protected virtual void OnPaidAdEvent()
    {
        PlayerData.logAllTime[(int) LogAllTimeEnum.paid_ad_count]++;
    }


    private void ValidateStoreProduct()
    {
        var purchaser = Kernel.ResolveDirty<BasePurchaser>();
        if (purchaser.IsInitialized())
        {
            var shop = Kernel.GetDatabase<ShopDatabase>();
            foreach (var shopSubscriptionItem in shop.SubscriptionItems)
            {
                bool found = false;
                foreach (var desc in shop.storeProductDescriptors)
                    if (desc.StoreProductId == shopSubscriptionItem.storeProductId)
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    Debug.LogError("not found correspond store product " + shopSubscriptionItem.storeProductId);
                else
                {
                    Debug.Log("check subscription " + shopSubscriptionItem.storeProductId);
                    if (PlayerData.subscription[shopSubscriptionItem.key] <= 0
                        && purchaser.IsSubcribed(shopSubscriptionItem.storeProductId, out int expiredIn))
                    {
                        if (shopSubscriptionItem.activeDays <= 0)
                            Debug.LogError("active days cannot be zero", RootView.rootView.ShopDatabase);
                        var remainDays = Mathf.Min(shopSubscriptionItem.activeDays, expiredIn);
                        Debug.Log("reactive remain day of subscriptions");
                        PlayerData.subscription[shopSubscriptionItem.key] = remainDays;
                        SubjectController.SubscriptionActivated.OnNext(shopSubscriptionItem.key);
                    }
                }
            }
        }
    }

    private void LoadGameUi()
    {
        gameUi = ScreenRoot.playScreen.ResolveDependency<BaseGameUI>();
        if (gameUi != null)
            gameUi.CheckRegister();
    }

    public virtual void OnRemoteConfigFetched(bool complete)
    {
    }

    protected virtual void HandlePointParameterCallBack(PointParameter value)
    {
        // example
//        if (value.enumType == BuiltInEnumType.AfterInCallBackEnum)
//        {
//            switch ((AnimatorCallbackEnum) value.enumValue)
//            {
//                case AnimatorCallbackEnum.None:
//                    break;
//                case AnimatorCallbackEnum.ShakeCamera:
//                    break;
//                case AnimatorCallbackEnum.StarHitBox:
//                    WaveData.coin.Value++;
////                    starProgress.TweenTo(WaveData.coin);
////                    if (WaveData.coin >= currentGameSetting.starBoxProgress)
////                        1f.Timer(() =>
////                        {
////                            if (WaveData.coin >= currentGameSetting.starBoxProgress)
////                            {
////                                starProgress.SetTo(0);
////                                WaveData.coin = WaveData.coin % currentGameSetting.starBoxProgress;
////                                starProgress.TweenTo(WaveData.coin);
////                            }
////                        });
//                    break;
//                case AnimatorCallbackEnum.CoinHit:
//                    ((int) SoundEnum.CoinHitPanelSound).PlaySound();
//                    //    PlayerData.Claim(currentGameSetting.starRewards);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
//            }
//        }
    }


    protected virtual void OnNewDailyData()
    {
        var now = DateTime.Now;

        if (PlayerData.energy.Value < setting.startEnergyNewDay)
        {
            PlayerData.energy.Value = setting.startEnergyNewDay;
            PlayerData.AddCustomTimeTickFromNow((int) BaseCustomTimeTick.CountDownGiveEnergy,
                setting.countDownEnergy.value);
        }

        DailyData.gameControllerChecked = true;
        foreach (var onNewDayClaim in customDailyDataSetting)
            DailyData.customPropertyList[onNewDayClaim.key].Value = onNewDayClaim.amount;
        DailyData.Save();
    }


    public void OnKernelLoadedAndFinishLoadingScreen()
    {
        switch (AutoStart)
        {
            case GameRootStartAction.None:
                ScreenRoot.ShowStartScreen();
                break;
            case GameRootStartAction.StartFirstTime:
                if (!PlayerData.firstTime)
                {
                    PlayerData.firstTime = true;
                    LoadAndStartCurrentGameSave();
                }
                else
                    ScreenRoot.ShowStartScreen();

                break;
            case GameRootStartAction.AlwaysStartGame:
                LoadAndStartCurrentGameSave();
                break;
            case GameRootStartAction.StartWhenLevel0:
                if (Level == 0)
                    LoadAndStartCurrentGameSave();
                else
                    ScreenRoot.ShowStartScreen();
                break;
            case GameRootStartAction.StartWhenLevelBelow:
                if (Level < gameSetting.autoStartWhenLevelBelow)
                    LoadAndStartCurrentGameSave();
                else
                    ScreenRoot.ShowStartScreen();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    protected virtual void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            PlayerData.Save();
    }

    public virtual void SelectLevel(int level)
    {
        StartLevel(level);
    }


    protected int CurrentLevelOfMode;

    public virtual void StartLevel(int level)
    {
        PlayerData.currentLevel.Value = level;
        PlayerData.Save();
        StartPlayerDataLevel();
        CurrentLevelOfMode = level;
    }

    protected List<Product> _equip;

    public virtual void Equip(List<Product> equip)
    {
        _equip = equip;
    }

    protected void InvokeEquip()
    {
        if (_equip != null)
        {
            foreach (var product in _equip)
                if (product.quantity == Quantity.GameAction)
                    SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(product));
        }

        _equip = null;
    }

    protected abstract void StartPlayerDataLevel();


    public void LoadAndStartCurrentGameSave(int playerMode = 0)
    {
        ProcessTutorialStep();
        if (LoadGameSave())
            StartCurrentGameSave(playerMode);
        else
            StartPlayerDataLevel();
    }

    protected virtual void ProcessTutorialStep()
    {
        if (setting.deleteGameSaveWhenTutorialUnfinished && PlayerData.tutorialStep >= 0)
        {
            PlayerData.tutorialStep = 0;
            UIDebugLog.LogError("Delete game save because not finish tutorial");
            DeleteGameSave();
        }
    }

    public abstract void StartLevelDirty(object input);

    public abstract bool IsNotOnAnyAction();

    public abstract void FreeAction();

    protected bool isInGame;

    protected virtual void InitGame()
    {
        CheckKernelLoaded();
        BoosterCount = 0;
        Clear();
        PlayerData.CheckAndTurnOnSave();
        OnStartGame();
        SubjectController.OnGameMainEvent.OnNext(GameMainEvent.StartGame);
        CheckShowBannerAds();
        PlayOnStartMusic();
        DuongQualityChecking.CheckDuplicateCall(nameof(InitGame));
        isInGame = true;
        DisableInput = false;

        SessionData.combo.Value = 0;
        if (gameUi != null && gameUi.ComboProgress != null)
            gameUi.ComboProgress.ResetCoolDown();
        WaveData.Clear();
        ClearUseShopItem(false);
    }

    protected virtual void Combo()
    {
        SessionData.combo.Value++;
        PlayComboSound(SessionData.combo.Value);
        if (gameUi.ComboProgress != null)
            gameUi.ComboProgress.StartCooldown(BaseCurrentGameSetting.cooldownCombo,
                () => { SessionData.combo.Value = 0; });
    }

    protected virtual void PlayComboSound(int combo)
    {
        if (InGameSetting != null)
            InGameSetting.GetSound(combo).PlaySound();
    }

    public virtual void SetCurrentLevelDirty(BaseLevel level)
    {
        
    }

    protected virtual void GameEndHandler()
    {
        isInGame = false;
        musicOnGameEnd.PlayMusic();
    }

    protected virtual void PlayOnStartMusic()
    {
        if (musicOnStartGame.Length > 0)
            musicOnStartGame.TakeRandom().PlayMusic();
    }

    public void CheckShowBannerAds()
    {
        AdsManager.NotShowBanner = !IsValidToShowAds();
        Kernel.Resolve<AdsManager>().CheckShowBanner();
    }

    protected virtual bool InitTut()
    {
        Debug.Log("PlayerData.tutorialStep" + PlayerData.tutorialStep);
        return false;
    }

    public virtual void Handler(ParameterTutorialMiniAction action)
    {
    }

    protected bool IsTutNexted;

    public bool NextTut(int add, ParameterTutorialMiniAction[] actions)
    {
        if (actions != null)
            foreach (var parameterGameAction in actions)
                Handler(parameterGameAction);

        PlayerData.tutorialStep += add;
        IsTutNexted = true;
        return InitTut();
    }

    public virtual bool ForceInitTut(int step)
    {
        PlayerData.tutorialStep = step;
        IsTutNexted = false;
        return InitTut();
    }

    public virtual void StopTut(int indexWhenStop)
    {
        PlayerData.tutorialStep = indexWhenStop;
        IActiveTutorialComponents.StopTut();
        RootView.rootView.onTutorial.BoolValue = false;
        RootView.rootView.gameController.DisableInput = false;
        ScreenRoot.TurnRayCast(true);
        CheckShowBannerAds();
    }

    public virtual void LoadRemoteConfig()
    {
    }

    public virtual void Clear()
    {
        SubjectController.ClearGameDisposables();
        WaveData.Clear();
    }


    public virtual void StartInput(float delay)
    {
        if (delay > 0)
            delay.Timer(Input);
        else
            Input();

        void Input()
        {
            _inputDisposable?.Dispose();
            _inputDisposable = InputHandle().AddToGameDisposable();
        }
    }

    public virtual void StopInput()
    {
        _inputDisposable?.Dispose();
    }

    private IDisposable InputHandle()
    {
        return this.UpdateAsObservable()
            .Where(c => ValidInput())
            .Subscribe(_ =>
            {
                if (Time.frameCount % 15 == 0 && Time.time > 10)
                {
                    foreach (var baseFrameworkViewCheck in binding.continuouslyChecks)
                        baseFrameworkViewCheck.Check();
                    _adsManager.HideNative();
                }

                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                    OnInputUpdate();
            }).AddToGameDisposable();
    }


    protected abstract void OnInputUpdate();

    public virtual void DialogTest(int testEnum, int value)
    {
    }

    public virtual bool ValidInput()
    {
        return !Pause.Value
               && !BoardBusy.Value
               && !DisableInput
               && !RootView.rootView.pauseMoving
               && !ScreenRoot.popupManager.AnyPopupShow
               && !ScreenRoot.dialogController.IsBusy()
               && (setting.validInputs.Length == 0 || setting.validInputs.All(x => x.IsValid()));
    }


    public abstract void Undo();


    public enum BaseGameState
    {
        None,
        Playing,
        Win,
        BoardLose,
        GameOver,
    }

    protected ReactiveProperty<BaseGameState> baseGameStateProperty = new ReactiveProperty<BaseGameState>();

    protected BaseGameState baseGameState
    {
        get => baseGameStateProperty.Value;
        set => baseGameStateProperty.Value = value;
    }

    protected virtual void OnStartGame()
    {
        if (gameUi != null)
            gameUi.OnStartGame();
        startGameCompositeDisposable.Clear();
        if (gameUi != null)
            foreach (var baseRxAnimator in gameUi.BindingUi.onStartGame)
                baseRxAnimator.StartAction();
        foreach (var baseRxAnimator in binding.onStartGame)
            baseRxAnimator.StartAction();
        baseGameState = BaseGameState.Playing;
    }


    public abstract void UpdateActionTime();

    public virtual bool IsOnPlaying() => baseGameState == BaseGameState.Playing;


    public virtual bool CheckBoardWin(bool force = false)
    {
        if (force || IsOnPlaying() && IsBoardWin() && baseGameState != BaseGameState.Win)
        {
            baseGameState = BaseGameState.Win;
            binding.onBoardWin.StartAction();
            SetBoardFinish(true);
            Observable.FromCoroutine(IeFinishingWin).Subscribe()
                .AddToGameDisposable()
                .AddToScreenChangeDisposable();
            return true;
        }

        return false;
    }

    public bool CheckBoardLose(bool force = false)
    {
        if (force || IsOnPlaying() && IsBoardLose())
        {
            baseGameState = BaseGameState.BoardLose;
            SetBoardFinish(false);
            HandlerBoardLose();
            return true;
        }

        return false;
    }

    protected virtual void SetBoardFinish(bool winLose)
    {
    }

    protected abstract bool IsBoardWin();
    protected abstract bool IsBoardLose();


    protected void LevelUp()
    {
        if (!IsInTutorial())
            PlayerData.currentLevel.Value++;
        Debug.Log("Level Up");
    }

    private GameSetting _gameSetting;
    private InGameSetting _inGameSetting;

    private bool _loadingGameSetting;

    protected GameSetting gameSetting //=> Kernel.GetDatabase<GameSetting>();
    {
        get
        {
            if (!_loadingGameSetting)
            {
                _gameSetting = Kernel.GetDatabase<GameSetting>();
                _loadingGameSetting = true;
            }

            return _gameSetting;
        }
    }

    public void ClearGameSetting() => _loadingGameSetting = false;

    public InGameSetting InGameSetting
    {
        get
        {
            if (_inGameSetting == null)
                _inGameSetting = Kernel.GetDatabase<InGameSetting>();
            return _inGameSetting;
        }
    }

    protected virtual void StartCurrentGameSave(int playMode = 0)
    {
        SessionData.playMode.Value = playMode;
        DuongQualityChecking.CheckDuplicateCall(nameof(StartCurrentGameSave));
        UpdateGameSaveDisplay();
        HandlerResume();
        var levelInt = PlayerData.currentLevel.Value;
        ScreenRoot.playScreen.SetupStartLevel(levelInt);
    }

    protected virtual void AfterInitGame()
    {
        CheckShowPackAfterInitGame(PlayerData.currentLevel.Value);
        DuongQualityChecking.CheckDuplicateCall(nameof(AfterInitGame));
    }

    protected virtual void CheckShowPackAfterInitGame(int levelInt)
    {
        foreach (var packShow in gameSetting.packShowAfterInitGame)
            if (IsShopItemValidToShow(packShow.key) && packShow.levelConditionCheck.CheckCondition(levelInt))
            {
                packShow.delayCheck.Timer(() =>
                {
                    if (!ScreenRoot.AnyPopupOrDialogShow())
                    {
                        ScreenRoot.popupManager.Resolve<PopupPack>().ShowPack(packShow.key);
                    }
                });
                break;
            }
    }

    protected abstract void SaveLevel();
    protected abstract void SaveCurrentGameSave();

    protected abstract bool LoadGameSave();
    protected abstract void DeleteGameSave();

    protected abstract void UpdateGameSaveDisplay();

    protected virtual void PrepareReadyToPlay()
    {
    }

    public virtual void LogCustomForTesting()
    {
    }


    /// <summary>
    ///  to choose GameSave property can display/bind to UI, not all of property
    /// </summary>
    /// <returns></returns>
    protected abstract int[] GetGameSaveDisplayPropertyRemap();


    public virtual IntReactiveProperty GetGameSaveProperty(int key)
    {
        return GameSaveDisplayProperty[GameSaveDisplayRemap.IndexOf(key)];
    }

    public virtual bool IsInTutorial()
    {
        return false;
    }

    public enum GameRootStartAction
    {
        None,
        StartFirstTime,
        AlwaysStartGame,
        StartWhenLevel0,
        StartWhenLevelBelow,
    }


    private static readonly Quantity[] earnQuantityLog =
    {
        Quantity.Diamond,
        Quantity.Gold,
        Quantity.Energy,
        Quantity.Spin,
        Quantity.Stock,
        Quantity.EquipData,
        Quantity.CustomTimeTick,
        Quantity.CustomProperty,
    };

    private static readonly Quantity[] spendQuantityLog =
    {
        Quantity.Diamond,
        Quantity.Gold,
        Quantity.Energy,
        Quantity.Spin,
        Quantity.Stock,
        Quantity.EquipData,
//        Quantity.IapBuying,
        Quantity.CustomQuantity,
        Quantity.WatchAds,
    };

    private bool CheckLogged(SonatLogEarnVirtualCurrency log)
    {
        string fullLog =
            log.virtual_currency_name 
            + "_" + log.virtual_currency_type
            + "_" + log.placement
            + "_" + log.spend_item_type
            + "_" + log.spend_item_id
            ;
        var hash = DuongExtensions.GetStringSha256Hash(fullLog);
        var list = PlayerPrefs.GetString("earn_virtual_currency_logged").Split(",").ToList();
        if (list.Contains(hash))
        {
            list.Add(hash);
            PlayerPrefs.SetString("earn_virtual_currency_logged",string.Join(',',list));
            return false;
        }

        return true;
    }
    
    public virtual void CheckLogVirtualCurrency<T, T2>(Trade<T, T2> trade, bool skipEarn = false,
    bool skipSpend = false) where T : Product where T2 : Product
    {
        if (!string.IsNullOrEmpty(trade.BoosterName))
            LogUseBooster(trade.BoosterName);
        
        var earnLog = new SonatLogEarnVirtualCurrency()
        {
            virtual_currency_name = GetItemType(trade.Product),
            virtual_currency_type = GetItemId(trade.Product),
            value = trade.Product.amount,
            placement = GetCurrentPlacement(),
            spend_item_type = GetItemType(trade.Price),
            spend_item_id = GetItemId(trade.Price),
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            mode = Kernel.ParameterValueRetriever.Mode,
        };
        earnLog.is_first_buy = CheckLogged(earnLog);
        
        
        if (earnQuantityLog.Contains(trade.Product.quantity) && !skipEarn)
            earnLog.Post();

        var spendLog = new SonatLogSpendVirtualCurrency()
        {
            virtual_currency_name = GetItemType(trade.Price),
            virtual_currency_type = GetItemId(trade.Price),
            value = trade.Price.amount,
            placement = GetCurrentPlacement(),
            earn_item_type = GetItemType(trade.Product),
            earn_item_id = GetItemId(trade.Product),
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            mode = Kernel.ParameterValueRetriever.Mode,
        };

        if (spendQuantityLog.Contains(trade.Price.quantity) && !skipSpend)
            spendLog.Post();


        var rewardedLog = GetRewardedLog(trade);
        rewardedLog?.Post();

        StupidLogBuyShopItem(trade);
    }

    public void LogShowUi(string showUiName, string openBy, string action)
    {
        new SonatLogShowUi()
        {
            name = showUiName,
            open_by = openBy,
            action = action,
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            placement = GetCurrentPlacement(),
        }.Post();
    }


    public void LogShortcut(string shortcut)
    {
        new SonatLogClickIconShortcut()
        {
            shortcut = shortcut,
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            placement = GetCurrentPlacement(),
            mode = Kernel.ParameterValueRetriever.Mode.ToString(),
        }.Post();
    }

    protected virtual SonatLogVideoRewarded GetRewardedLog<T, T2>(Trade<T, T2> trade)
        where T : Product where T2 : Product
    {
        if (trade.Price.Quantity == Quantity.WatchAds)
        {
            return new SonatLogVideoRewarded()
            {
                placement = GetCurrentPlacement(),
                item_type = GetItemType(trade.Product),
                item_id = GetItemId(trade.Product),
//                value = trade.Product.amount,
                level = Kernel.ParameterValueRetriever.LevelDisplay,
                mode = Kernel.ParameterValueRetriever.Mode,
            };
        }

        return null;
    }

    protected virtual void StupidLogBuyShopItem<T, T2>(Trade<T, T2> trade) where T : Product where T2 : Product
    {
        if (trade.Price.Quantity == Quantity.IapBuying
            || trade.Price.Quantity == Quantity.Gold
            || trade.Price.Quantity == Quantity.Diamond
        )
        {
            var log = new SonatLogBuyShopItem()
            {
                placement = GetCurrentPlacement(),
                item_type = GetItemType(trade.Product),
                item_id = GetItemId(trade.Product),
                value = trade.Product.amount,
                level = Kernel.ParameterValueRetriever.LevelDisplay,
                mode = Kernel.ParameterValueRetriever.Mode,
                currency = GetItemType(trade.Price),
                is_first_buy = PlayerPrefs.GetInt("is_first_buy") == 0
            };

            if (trade.Price.Quantity == Quantity.IapBuying)
                PlayerPrefs.SetInt("is_first_buy", 1);

            if (trade.Price.Quantity == Quantity.IapBuying)
            {
                PlayerData.iapBought[trade.Price.index] = PlayerData.iapBought[trade.Price.index] + 1;
                PlayerData.Save();
            }

            log.Post();
        }
    }


    public virtual string GetLogName(Product product)
    {
        return product.quantity.ToString().ToLower() + "_" + product.index + "_" + product.amount;
    }

    public virtual string GetQuantityName(Product product)
    {
        return product.quantity.ToString().ToLower() + "_" + product.index;
    }

    public virtual InGameNotification GetQuestNotification(int key)
    {
        DailyQuestItem quest = null;
        foreach (var temp in GameSetting.dailyQuestItems)
            if (temp.key == key)
                quest = temp;

        if (quest == null) return null;

        return new InGameNotification()
        {
            name = BuiltInEnumType.QuestEnum.GetName(key),
            message = string.Format("<color=green>Complete:</color> " + quest.Description, quest.Condition)
        };
    }

    // duong_comment_flow_offer_show3 : check offer ready to show in current game logic
    public virtual bool IsOfferOkayToShow(int key)
    {
        return false;
    }

    public virtual void UpdateUserProperty()
    {
    }

    public virtual BasePopup GetPopup(int popupEnum)
    {
        return null;
    }

    public virtual BaseScreen GetScreen(int screenEnum)
    {
        return null;
    }

    public virtual void CalculateUpdateQuest(int add = 0)
    {
        UpdateQuest((int) BaseQuestEnum.FinishLevel, Level + 1 + add);
        UpdateQuest((int) BaseQuestEnum.AcquiredStar, PlayerData.levelProgress.Sum());
    }

    public virtual T GetRootViewProperty<T>(string propertyName)
    {
        var getField = GetType().GetField(propertyName);
        if (getField != null)
            return (T) getField.GetValue(this);

        var getProperty = GetType().GetProperty(propertyName);
        if (getProperty == null)
            return GetType().GetPrivateFieldValue<T>(propertyName, this);

        return (T) getProperty.GetValue(this);
    }

    public virtual IntReactiveProperty GetRootViewProperty(string propertyName)
    {
        var getField = GetType().GetField(propertyName);
        if (getField != null)
            return (IntReactiveProperty) getField.GetValue(this);

        var getProperty = GetType().GetProperty(propertyName);
        if (getProperty == null)
            return GetType().GetPrivateFieldValue<IntReactiveProperty>(propertyName, this);

        return (IntReactiveProperty) getProperty.GetValue(this);
    }

    public virtual FloatReactiveProperty GetRootViewPropertyFloat(string propertyName)
    {
        var getField = GetType().GetField(propertyName);
        if (getField != null)
            return (FloatReactiveProperty) getField.GetValue(this);

        var getProperty = GetType().GetProperty(propertyName);
        if (getProperty == null)
            return GetType().GetPrivateFieldValue<FloatReactiveProperty>(propertyName, this);

        return (FloatReactiveProperty) getProperty.GetValue(this);
    }

    public virtual bool CheckShowNavigationDialog()
    {
        return false;
    }

    public virtual bool CheckNavigation(int nav, bool skipScreenCheck = false)
    {
        return false;
//        return PlayerData.navigationChecker.Contains(nav);
    }

    /// <summary>
    /// for IntRetriever
    /// </summary>
    /// <param name="pName"></param>
    /// <returns></returns>
    public int GetIntCurrentGameSetting(string pName)
    {
#if UNITY_EDITOR
        if (BaseCurrentGameSetting == null)
        {
            Debug.LogError("Not found Current Game Setting");
            return 0;
        }
#endif
        return ReflectionExtension.GetIntValue(BaseCurrentGameSetting, pName);
    }

    /// <summary>
    /// for IntRetriever
    /// </summary>
    /// <param name="pName"></param>
    /// <returns></returns>
    public int[] GetIntArrayCurrentGameSetting(string pName)
    {
        return ReflectionExtension.GetIntValues(BaseCurrentGameSetting, pName);
    }

    public abstract int Level { get; } //=> PlayerData.playTimes;
    public virtual int LevelDisplay => Level + 1;
    public virtual string Mode => "classic";

    public virtual Vector3 GetPos(Point point)
    {
        return point.GetPos();
    }


    protected virtual bool IsValidToShowAds()
    {
//        #if UNITY_EDITOR
//            Debug.Log(RemoteConfigHelper.GetValueInt(RemoteConfigKey.start_level_show_banner));
//        #endif
        return Kernel.GetDatabase<GameSetting>().showBannerCondition
            .Valid(PlayerData.tutorialStep, PlayerData.CurrentLevel);
    }

    public virtual void HandlerShowNotEnough(Quantity currency, Action action = null)
    {
        if (currency == Quantity.Gold ||
            currency == Quantity.Diamond ||
            currency == Quantity.Energy)
        {
            if (action == null)
                action = () =>
                {
                    ScreenRoot.dialogController.CloseAll();
                    ScreenRoot.popupManager.ShowPopup<PopupShop>();
                };
            ScreenRoot.dialogController.ShowMessage(
                string.Format(gameSetting.metaDataSetting.notEnoughMessageFormat, GetQuantityName(currency)), action,
                () => { });
        }
        else
        {
            SubjectController.MessageBroadcastingEvent.OnNext(new BroadcastMessage()
            {
                message = string.Format(gameSetting.metaDataSetting.notEnoughMessageFormat2, GetQuantityName(currency))
            });
        }
    }

    public virtual void HandlerShowNotEnoughMessageOnly(Quantity currency)
    {
        if (currency == Quantity.Gold ||
            currency == Quantity.Diamond ||
            currency == Quantity.Energy)
        {
            ScreenRoot.dialogController.ShowMessage(
                string.Format(gameSetting.metaDataSetting.notEnoughMessageFormat, GetQuantityName(currency)));
        }
        else
        {
            SubjectController.MessageBroadcastingEvent.OnNext(new BroadcastMessage()
            {
                message = string.Format(gameSetting.metaDataSetting.notEnoughMessageFormat2, GetQuantityName(currency))
            });
        }
    }

    public virtual string NotEnoughMessage(Quantity quantity) =>
        string.Format(gameSetting.metaDataSetting.notEnoughMessageFormat, GetQuantityName(quantity));


    protected virtual string GetQuantityName(Quantity quantity)
    {
        foreach (var quantityName in gameSetting.metaDataSetting.quantityNames)
            if ((int) quantity == quantityName.value)
                return quantityName.value2;
        switch (quantity)
        {
            case Quantity.None:
                break;
            case Quantity.Gold:
                return "gold";
            case Quantity.Diamond:
                return "diamond";
            case Quantity.Energy:
                return "energy";
            case Quantity.Spin:
                return "spine";
            case Quantity.Medal:
                break;
            case Quantity.Item:
                break;
            case Quantity.CustomProperty:
                break;
            case Quantity.CustomProduct:
                break;
            case Quantity.WatchAds:
                break;
            case Quantity.IapBuying:
                break;
            case Quantity.Subscription:
                break;
            case Quantity.Achievement:
                break;
            case Quantity.Quest:
                break;
            case Quantity.Free:
                break;
            case Quantity.SystemAction:
                break;
            case Quantity.GameAction:
                break;
            case Quantity.SystemGameAction:
                break;
            case Quantity.TutorialSkip:
                break;
            default:
                return quantity.ToString();
        }

        return quantity.ToString();
    }

    public bool PlayScreenOnStart()
    {
        switch (AutoStart)
        {
            case GameRootStartAction.None:
                return false;
            case GameRootStartAction.StartFirstTime:
                return (!PlayerData.firstTime);
            case GameRootStartAction.AlwaysStartGame:
                return true;
            case GameRootStartAction.StartWhenLevel0:
                return Level == 0;
            default:
                return false;
        }
    }

    public virtual void HandlerTimeTick()
    {
    }

    public virtual void OnPlaySound(int sound)
    {
        if (gameSetting != null)
            foreach (var gameSettingSoundVibrate in gameSetting.SoundVibrates)
                if (gameSettingSoundVibrate.value == sound)
                {
                    DuongVibration.Vibration(gameSettingSoundVibrate.value2);
                    break;
                }
    }

    public void HandlerRestoreNoAds(bool noAds)
    {
        if (!noAds)
            ScreenRoot.dialogController.ShowMessage("You haven't purchased anything, Nothing to restore");
        else if (AdsManager.NoAds)
            ScreenRoot.dialogController.ShowMessage("Nothing to restore");
        else if (!AdsManager.NoAds)
            SubjectController.ProductSystemActionEvent.OnNext(new ProductAction()
            {
                index = (int) ProductSystemAction.NoAds
            });
    }

    public virtual int GetUnlockEquipUnlockState(int unlockEquipDataEnum, int itemIndex)
    {
        return 0;
    }


    public virtual void ClaimEventItem(int eventType, int eventKey, int intAmount)
    {
        StaticDataController.LoadEventPlayerData(eventType, Level, i => OnLoadedEventDay(eventType, eventKey));

        switch ((EventDayType) eventKey)
        {
            case EventDayType.Default:
                break;
            case EventDayType.Special:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventKey), eventKey, null);
        }

        if (StaticDataController.EventDays[eventType] != null)
        {
            StaticDataController.EventDays[eventType].Claim(eventKey, intAmount);
            StaticDataController.EventDays[eventType].Save();
        }
    }


    public abstract int GetGameSaveValue(int key);

    public string GetItemType(Product reward)
    {
        switch (reward.quantity)
        {
            case Quantity.None:
                return reward.quantity.ToString().ToLower();
            case Quantity.Gold:
                return reward.quantity.ToString().ToLower();
            case Quantity.Diamond:
                return reward.quantity.ToString().ToLower();
            case Quantity.Energy:
                return reward.quantity.ToString().ToLower();
            case Quantity.Spin:
                return reward.quantity.ToString().ToLower();
            case Quantity.Medal:
                return reward.quantity.ToString().ToLower();
            case Quantity.Item:
                return reward.quantity.ToString().ToLower();
            case Quantity.CustomProperty:
                return "booster";
            case Quantity.CustomProduct:
                return BuiltInEnumType.CustomProductEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.CustomDailyProperty:
                return BuiltInEnumType.CustomDailyDataProperty.GetName(reward.index).ToSnakeCase();
            case Quantity.CustomTimeTick:
                return BuiltInEnumType.CustomTimeTick.GetName(reward.index).ToSnakeCase();
            case Quantity.EquipData:
                return BuiltInEnumType.UnlockEquipDataEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.EventItem:
                return BuiltInEnumType.EventEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.Stock:
                return BuiltInEnumType.PlayerDataStock.GetName(reward.index).ToSnakeCase();
            case Quantity.WatchAds:
                return reward.quantity.ToString().ToSnakeCase();
            case Quantity.IapBuying:
                return "iap";
            case Quantity.Subscription:
                return BuiltInEnumType.SubscriptionKey.GetName(reward.index).ToSnakeCase();
            case Quantity.Achievement:
                return BuiltInEnumType.AchievementEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.Quest:
                return BuiltInEnumType.QuestEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.Free:
                break;
            case Quantity.Random:
                break;
            case Quantity.SystemAction:
                return BuiltInEnumType.ProductSystemAction.GetName(reward.index).ToSnakeCase();
            case Quantity.GameAction:
                return BuiltInEnumType.ProductGameAction.GetName(reward.index).ToSnakeCase();
            case Quantity.SystemGameAction:
                return BuiltInEnumType.GameActionEvent.GetName(reward.index).ToSnakeCase();
            case Quantity.TutorialSkip:
                break;
            case Quantity.CustomQuantity:
                return "action";
            default:
                throw new ArgumentOutOfRangeException();
        }

        return GetQuantityName(reward.quantity).ToSnakeCase();
    }

    public string GetItemId(Product reward)
    {
        switch (reward.quantity)
        {
            case Quantity.None:
                break;
            case Quantity.Gold:
                return reward.quantity.ToString().ToLower();
            case Quantity.Diamond:
                return reward.quantity.ToString().ToLower();
            case Quantity.Energy:
                return reward.quantity.ToString().ToLower();
            case Quantity.Spin:
                return reward.quantity.ToString().ToLower();
            case Quantity.Medal:
                return reward.quantity.ToString().ToLower();
            case Quantity.Item:
                return reward.quantity.ToString().ToLower();
            case Quantity.CustomProperty:
                return BuiltInEnumType.CustomPlayerDataProperty.GetName(reward.index).ToSnakeCase();
            case Quantity.CustomProduct:
                return BuiltInEnumType.CustomProductEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.CustomDailyProperty:
                return BuiltInEnumType.CustomDailyDataProperty.GetName(reward.index).ToSnakeCase();
            case Quantity.CustomTimeTick:
                return BuiltInEnumType.CustomTimeTick.GetName(reward.index).ToSnakeCase();
            case Quantity.EquipData:
                return reward.parameter.ToString();
            case Quantity.EventItem:
                return BuiltInEnumType.EventEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.Stock:
                return BuiltInEnumType.PlayerDataStock.GetName(reward.index).ToSnakeCase();
            case Quantity.WatchAds:
                return reward.quantity.ToString().ToSnakeCase();
            case Quantity.IapBuying:
                return "iap";
            case Quantity.Subscription:
                return BuiltInEnumType.SubscriptionKey.GetName(reward.index).ToSnakeCase();
            case Quantity.Achievement:
                return BuiltInEnumType.AchievementEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.Quest:
                return BuiltInEnumType.QuestEnum.GetName(reward.index).ToSnakeCase();
            case Quantity.Free:
                return reward.quantity.ToString().ToSnakeCase();
            case Quantity.Random:
                break;
            case Quantity.SystemAction:
                return BuiltInEnumType.ProductSystemAction.GetName(reward.index).ToSnakeCase();
            case Quantity.GameAction:
                return BuiltInEnumType.ProductGameAction.GetName(reward.index).ToSnakeCase();
            case Quantity.SystemGameAction:
                return BuiltInEnumType.GameActionEvent.GetName(reward.index).ToSnakeCase();
            case Quantity.TutorialSkip:
                break;
            case Quantity.CustomQuantity:
                return BuiltInEnumType.CustomQuantity.GetName(reward.index).ToSnakeCase();
            default:
                throw new ArgumentOutOfRangeException();
        }

        return (reward.quantity + "_" + reward.index).ToSnakeCase();
    }

    public string GetBoosterName(Product shopItemProduct)
    {
        if (shopItemProduct.quantity == Quantity.CustomProperty)
            return BuiltInEnumType.CustomPlayerDataProperty.GetName(shopItemProduct.index);
        Debug.LogError("not found");
        return "not_found";
    }

    public string GetCurrentPlacement()
    {
        return BuiltInEnumType.LogPlacement.GetName(CurrentPlacement).ToSnakeCase();
    }

    public ClaimedProduct HandlerClaimCustomQuantity(ClaimedProduct claimedReward)
    {
        return null;
    }

    public int CurrentPlacement
    {
        get
        {
            int place = 0;
            int level = -1;
            for (var i = 0; i < placements.Count; i++)
            {
                if (placements[i].y > level)
                {
                    place = placements[i].x;
                    level = placements[i].y;
                }
            }

            return place;
        }
    }

    private readonly List<Vector2Int> placements = new List<Vector2Int>();

    public void OnPlacement(int placement, int level, bool b)
    {
        if (b)
            placements.AddDistinct(new Vector2Int(placement, level));
        else
            placements.RemoveAll(x => x.x == placement && x.y == level);
    }

    public IntReactiveProperty GetCustomDataProperty(CustomDataEnumShortcut customPropertyEnumShortcut)
    {
        switch (customPropertyEnumShortcut)
        {
            case CustomDataEnumShortcut.pendingShopItemKey:
                return pendingShopItemKey;
            default:
                throw new ArgumentOutOfRangeException(nameof(customPropertyEnumShortcut), customPropertyEnumShortcut, null);
        }
    }

    public virtual bool CheckBuiltInValid(int key)
    {
        if (key == 0) // no ads available
            return !Kernel.Resolve<AdsManager>().IsNoAds();
        return false;
    }

    public virtual int GetDifficulty(int i)
    {
        return  0;
    }
}

public enum CustomDataEnumShortcut
{
    pendingShopItemKey = 0
}

/// <summary>
/// for initializing property of CustomDailyDataProperty on New day
/// </summary>
[Serializable]
public struct DailyCustomDataSetting
{
    [IndexAsEnum(BuiltInEnumType.CustomDailyDataProperty)]
    public int key;

    public int amount;
}