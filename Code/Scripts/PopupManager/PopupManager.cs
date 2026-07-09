using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class PopupManagerLevel
{
    public ToggleScript[] toggles;
    public Transform container;
    public BasePopup CurrentPopup { get; set; }
    public readonly CompositeDisposable PopupDisposable = new CompositeDisposable();
}

public class PopupManager : BaseView
{
    // ReSharper disable once NotAccessedField.Local 
    [MyButton(nameof(Load),nameof(Test))] [SerializeField] private bool test;
    
    public PopupManagerLevel[] levels;
    private BasePopup[] _popups;

    public ReactiveCollection<BasePopup> onShowingPopups = new ReactiveCollection<BasePopup>();

    public bool AnyPopupShow
    {
        get => anyPopupProperty.Value;
        private set => anyPopupProperty.Value = value;
    }
    
    public BoolReactiveProperty anyPopupProperty = new BoolReactiveProperty();

    private readonly Dictionary<Type, BasePopup> _dict = new Dictionary<Type, BasePopup>();

    
    
    private void Load()
    {
        _popups = GetComponentsInChildren<BasePopup>(true);
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
    
    private void Test()
    {
    }
    
    protected override void Register()
    {
        base.Register();
        _popups = GetComponentsInChildren<BasePopup>(true);
        foreach (var basePopup in _popups)
        {
            Register(basePopup);
            basePopup.transform.Release();
        }

        onShowingPopups.ObserveAdd().Subscribe(popup =>
        {
            UIDebugLog.Log("opened "+popup.Value);
            if (popup.Value.PauseMusic)
            {
                RootView.rootView.audioManager.StopResumeMusic(true); 
            }
        });
        
        onShowingPopups.ObserveRemove().Subscribe(popup =>
        {
            UIDebugLog.Log("closed "+popup.Value);
            if (popup.Value.PauseMusic)
            {
                RootView.rootView.audioManager.StopResumeMusic(false); 
            }
        });
        
        onShowingPopups.ObserveCountChanged().Subscribe(count =>
        {
            UIDebugLog.Log("numbers of popup: "+count);
        });
    }

    private void Register(BasePopup serviceInstance)
    {
        _dict[serviceInstance.GetType()] = serviceInstance;
        if (_popups.Any(x => x == null))
        {
            #if UNITY_EDITOR
            Debug.LogError("some popup null");
            Debug.Break();
            #endif
            Load();
        }
    }

    public T Resolve<T>() where T : BasePopup
    {
   
        if (!_dict.ContainsKey(typeof(T)))
        {
#if UNITY_EDITOR
            Debug.LogError("not found Popup : "+typeof(T));
          //  Debug.Break();
#endif
            return null;
        }

        return (T) _dict[typeof(T)];
    }

    private BasePopup Resolve(string popupName)
    {
        foreach (var keyValuePair in _dict)
            if (keyValuePair.Key.Name == popupName)
                return keyValuePair.Value;

        return null;
    }

    public void CheckAnyPopupShow()
    {
        AnyPopupShow = IsAnyPopupShowDirty();
        SubjectController.PopupChanged.OnNext(AnyPopupShow);
    }

    private BasePopup _lastPopup;

    public IObservable<Unit> CloseCurrentPopupAsObservable()
    {
        for (var i = levels.Length - 1; i >= 0; i--)
        {
            var level = levels[i];
            if (level.CurrentPopup != null)
            {
                onShowingPopups.Remove(level.CurrentPopup);
                return level.CurrentPopup.ObservableAnim_Out(level, true);
            }
        }

        return Observable.FromCoroutine(Empty);
    }
    
    public void CloseCurrentInstant()
    {
        for (var i = levels.Length - 1; i >= 0; i--)
        {
            var level = levels[i];
            if (level.CurrentPopup != null)
            {
                onShowingPopups.Remove(level.CurrentPopup);
                 level.CurrentPopup.CloseInstantThis(level, true);
            }
        }
    }
    
    
    public IObservable<Unit> CloseAllPopupAsObservable()
    {
        onShowingPopups.Clear();
        _lastPopup = null;
        IObservable<Unit> last = null;
        for (var i = levels.Length - 1; i >= 0; i--)
        {
            var level = levels[i];
            if (level.CurrentPopup != null)
            {
                if (last != null)
                    last.Subscribe();
                last = level.CurrentPopup.ObservableAnim_Out(level, true); //231229
            }
        }

        if (last != null)
            return last;

        return Observable.FromCoroutine(Empty);
    }

    private IEnumerator Empty()
    {
        if (false)
            // ReSharper disable once HeuristicUnreachableCode
            yield return null;
    }

    public void CloseAllPopUpInstant()
    {
        onShowingPopups.Clear();
        for (var i = levels.Length - 1; i >= 0; i--)
        {
            var level = levels[i];
            foreach (var toggleScript in level.toggles)
                toggleScript.OnChanged(false);
//            foreach (var rxAnimator in level.anims)
//                rxAnimator.ResetAnimation();
            level.CurrentPopup = null;
            RootView.rootView.screenRoot.CheckLogScreen();
        }

        foreach (var basePopup in _popups)
        {
            basePopup.Deactivate();
            basePopup.transform.Release();
        }

        AnyPopupShow = false;
    }

    public BasePopup ShowPopup(int popupEnum, int popupLevel, WhenPopupClose whenClose = WhenPopupClose.Close, CustomParameter parameter = null)
    {
        if (popupEnum < 0) // if < 0 it is close popup
        {
            CloseCurrentPopupAsObservable().Subscribe();
            return null;
        }

        return ShowPopup(GetPopup(popupEnum), popupLevel, whenClose, parameter);
    }

    public T ShowPopup<T>(int popupLevel = 0, WhenPopupClose whenClose = WhenPopupClose.Close,
        CustomParameter parameter = null) where T : BasePopup
    {
        var popup = Resolve<T>();
        if (popup != null)
        {
            int level = (popupLevel == -1) ? GetCurrentLevel() : popupLevel;
            DoShowPopup(popup, level, whenClose, parameter);
            return popup;
        }

        return null;
    }

    int GetCurrentLevel()
    {
        if (GetCurrentPopup() != null)
            return GetCurrentPopup().Level;
        return 0;
    }
    
    public T ShowPopupNext<T>(WhenPopupClose whenClose = WhenPopupClose.Close,
        CustomParameter parameter = null) where T : BasePopup
    {
        var popup = Resolve<T>();
        if (popup != null)
        {
            DoShowPopup(popup, GetCurrentPopup() == null ? 0 : GetCurrentPopup().Level+1, whenClose, parameter);
            return popup;
        }

        return null;
    }
    
    public BasePopup ShowPopupNext(int popupEnum, WhenPopupClose whenClose = WhenPopupClose.Close,
        CustomParameter parameter = null)
    {
        if (popupEnum < 0)
        {
            CloseCurrentPopupAsObservable().Subscribe();
            return null;
        }

        return ShowPopup(GetPopup(popupEnum), GetCurrentPopup() == null ? 0 : 1, whenClose, parameter);
    }

    public BasePopup ShowPopupNext(BasePopup popup, WhenPopupClose whenClose = WhenPopupClose.Close,
        CustomParameter parameter = null)
    {
        DoShowPopup(popup, GetCurrentPopup() == null ? 0 : 1, whenClose, parameter);
        return popup;
    }

    public BasePopup ShowPopup(BasePopup newPopup, int popupLevel = 0, WhenPopupClose whenClose = WhenPopupClose.Close,
        CustomParameter parameter = null)
    {
        DoShowPopup(newPopup, popupLevel, whenClose, parameter);
        return newPopup;
    }

//#if UNITY_EDITOR
    private readonly Dictionary<Type, float> _timing = new Dictionary<Type, float>();
//#endif
    private bool CheckDuplicateCall(Type type)
    {
//#if UNITY_EDITOR
        if (_timing.ContainsKey(type) &&  Time.unscaledTime - _timing[type]  <= 1f)
        {
            Debug.LogError("Duplicate invoke " + type);
            Debug.Break();
            return true;
            Kernel.Resolve<FireBaseController>().LogEvent("Error log duplicate invoke");
        }
        else
        {
            _timing[type] = Time.unscaledTime;
            return false;
        }
//#endif

        // ReSharper disable once HeuristicUnreachableCode
        return false;
    }

    private void DoShowPopup(BasePopup newPopup, int popupLevel, WhenPopupClose whenClose = WhenPopupClose.Close,
        CustomParameter parameter = null)
    {
        if (!CheckDuplicateCall(newPopup.GetType()))
        {
            var cPopupLevel = Mathf.Clamp(popupLevel, 0, levels.Length - 1);
//            Debug.LogError("beware of reopen last popup");
            newPopup.SetParameter(parameter);
            var level = levels[cPopupLevel];
            level.PopupDisposable.Clear();
            newPopup.OnShowHandler(whenClose);
            ShowPopupAsObservable(newPopup, cPopupLevel)?.Subscribe(_ => { }).AddTo(level.PopupDisposable);
        }
    }

    public BasePopup ShowPopup(string popupClassName, int popupLevel = 0,
        WhenPopupClose whenClose = WhenPopupClose.Close, CustomParameter parameter = null)
    {
        var popup = Resolve(popupClassName);
        return ShowPopup(popup, popupLevel, whenClose, parameter);
    }

    public IObservable<Unit> ShowPopupAsObservable<T>(int popupLevel) where T : BasePopup
    {
        var newPopup = Resolve<T>();
        return ShowPopupAsObservable(newPopup, popupLevel);
    }

    public IObservable<Unit> ShowPopupAsObservable(BasePopup newPopup, int popupLevel)
    {
        onShowingPopups.AddDistinct(newPopup);
        newPopup.Level = popupLevel;
        var level = levels[popupLevel];
        if (newPopup == level.CurrentPopup)
            return null;
        newPopup.transform.SetParentAndExpand(level.container, false, true);
        if (level.CurrentPopup != null)
        {
            onShowingPopups.Remove(level.CurrentPopup);
            _lastPopup = level.CurrentPopup;
            var anim = level.CurrentPopup.ObservableAnim_Out(level, false)
                .SelectMany(newPopup.ObservableAnim_In(level,newPopup.ignoreBg));

            level.CurrentPopup = newPopup;
            RootView.rootView.screenRoot.CheckLogScreen();
            CheckAnyPopupShow();
            return anim;
        }

        _lastPopup = null;
        level.CurrentPopup = newPopup;
        RootView.rootView.screenRoot.CheckLogScreen();
        CheckAnyPopupShow();
        return newPopup.ObservableAnim_In(level,newPopup.ignoreBg);
    }


    private bool IsAnyPopupShowDirty()
    {
        return GetCurrentPopup() != null;
    }

    public BasePopup GetCurrentPopup()
    {
        for (var i = levels.Length - 1; i >= 0; i--)
        {
            var level = levels[i];
            if (level.CurrentPopup != null)
                return level.CurrentPopup;
        }

        return null;
    }

    public void ReopenLastPopup()
    {
        UIDebugLog.Log(nameof(ReopenLastPopup), false,LogType.PopupAndScreen);
        if (_lastPopup != null)
            _lastPopup.Show();
    }
    
     public BasePopup GetPopup(string screenName)
    {
        foreach (var keyValuePair in _dict)
            if (keyValuePair.Key.Name == screenName)
                return keyValuePair.Value;
        return null;
    }
    
     public bool Exist(int popupEnum)
     {
         return GetPopup(popupEnum) != null;
     }
     
    public BasePopup GetPopup(int popupEnum)
    {
        var popup = RootView.rootView.gameController.GetPopup(popupEnum);
        if (popup != null)
            return popup;

        switch ((InScreenPopupEnum) popupEnum)
        {
            case InScreenPopupEnum.Policy:
                return Resolve<PopupPolicy>();
            case InScreenPopupEnum.RateUs:
                return Resolve<PopupRateUs>();
            case InScreenPopupEnum.Shop:
                return Resolve<PopupShop>();
            case InScreenPopupEnum.DailyQuest:
                return Resolve<PopupDailyQuest>();
            case InScreenPopupEnum.Quest:
                return Resolve<PopupQuest>();
            case InScreenPopupEnum.DailyReward:
                return Resolve<PopupDailyReward>();
            case InScreenPopupEnum.Profile:
                return Resolve<PopupProfile>();
            case InScreenPopupEnum.LeaderBoard:
                return Resolve<PopupLeaderBoard_Fake>();
            case InScreenPopupEnum.Achievement:
                return Resolve<PopupAchievement>();
            case InScreenPopupEnum.Setting:
                return Resolve<PopupSetting>();
            case InScreenPopupEnum.Pause:
                return Resolve<PopupPause>();
            case InScreenPopupEnum.SkinPopup:
                return Resolve<PopupSkin>();
            case InScreenPopupEnum.Spin:
                return Resolve<PopupSpin>();
            case InScreenPopupEnum.ReceiveReward:
                return Resolve<PopupReceiveReward>();
            case InScreenPopupEnum.OpenBox:
                return Resolve<PopupOpenBox>();
            case InScreenPopupEnum.ThankYouRateUs:
                return Resolve<PopupThankYouRateUs>();
            case InScreenPopupEnum.GameOver:
                return Resolve<PopupGameOver>();
            case InScreenPopupEnum.VIP:
                return Resolve<PopupVip>();
            case InScreenPopupEnum.Test:
                return Resolve<PopupTest>();
            case InScreenPopupEnum.CustomByIndex:
                return Resolve<PopupCustomByIndex>();
            case InScreenPopupEnum.BoosterEquip:
                return Resolve<PopupStartLevel>();
            case InScreenPopupEnum.Pack:
                return Resolve<PopupPack>();
            case InScreenPopupEnum.QuitGame:
                return Resolve<PopupQuitGame>();
            case InScreenPopupEnum.Close:
                return null;
            case InScreenPopupEnum.NoAds:
                return Resolve<PopupNoAds>();
            case InScreenPopupEnum.ComingSoon:
                return Resolve<PopupComingSoon>();
            case InScreenPopupEnum.Challenger:
                return Resolve<PopupChallenger>();
            case InScreenPopupEnum.Inform:
                return Resolve<PopupInform>();
            case InScreenPopupEnum.Congratulation:
                return Resolve<PopupCongratulation>();
            case InScreenPopupEnum.PopupScreen:
                return Resolve<PopupScreen>();
            case InScreenPopupEnum.Win:
                return Resolve<PopupWin>();
            case InScreenPopupEnum.Tutorial:
                return Resolve<PopupTutorial>();
            case InScreenPopupEnum.NoAds24h:
                return Resolve<PopupNoAds24h>();
            default:
                throw new ArgumentOutOfRangeException();
        } 
    }

    public bool HasTopBar()
    {
        var popup = GetCurrentPopup();
        return popup != null && popup.HasTopBar();
    }


    public bool IsNativePopupOnShow()
    {
        for (var i = levels.Length - 1; i >= 0; i--)
        {
            var level = levels[i];
            if (level.CurrentPopup != null && level.CurrentPopup.nativeAds)
                return true;
        }
        return false;
    }
}