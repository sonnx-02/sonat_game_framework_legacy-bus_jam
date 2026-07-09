using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[Serializable]
public class GameEventBinding
{
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    public IntBool[] eventDaySessionKey;

//    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
//    public IntBool specialEventDaySessionKey;
}

[Serializable]
public class ArrayIntDataHandler
{
    [SerializeField] private int key;
    [SerializeField] private BaseActionScript[] actions;
    [SerializeField] private IndexBindingScriptBase[] bindings;

    public void Process(int[] data)
    {
        if (key >= 0 && key < data.Length)
        {
            actions.StartAction(data[key]);
            bindings.OnChanged(data[key]);
        }
    }
}


[Serializable]
public class GameDataProcessing
{
    [SerializeField] private ArrayIntDataHandler[] finishGameHandler;

    public void HandlerFinishGameData(int[] data)
    {
        foreach (var arrayIntDataHandler in finishGameHandler)
        {
            arrayIntDataHandler.Process(data);
        }
    }
}

public abstract partial class BaseGameController
{

    protected readonly ReactiveProperty<BaseBaseShopItemView> PendingShopItem = new ReactiveProperty<BaseBaseShopItemView>();
    protected readonly ReactiveProperty<BaseBaseShopItemView> ToBuyShopItem = new ReactiveProperty<BaseBaseShopItemView>();
    [HideInInspector]
    public IntReactiveProperty pendingShopItemKey = new IntReactiveProperty();
    
    
    private bool _rateUsShowed;
    protected virtual void HandlerGameActionRetry()
    {
        if (PlayerData.playTimes == RemoteConfigKey.game_play_level_pass_to_rate.GetValueInt() && !_rateUsShowed)
        {
            Pause.Value = true;
            _rateUsShowed = true;
            Clear();
            ScreenRoot.popupManager.ShowPopup<PopupRateUs>()
                .SetActionWhenClose(() => { SubjectController.GameActionEvent.OnNext(GameActionEvent.Retry); });
        }
        else
        {
            Retry();
        }
    }
    
    protected virtual void GameActionEventHandler(GameActionEvent action)
    {
        switch (action)
        {
            case GameActionEvent.Pause:
                HandlerPause();
                break;
            case GameActionEvent.Resume:
                HandlerResume();
                break;
            case GameActionEvent.Quit:
                HandlerQuit();
                break;
            case GameActionEvent.Retry:
                HandlerGameActionRetry();
                break;
            case GameActionEvent.NextGame:
                HandlerNextGame();
                break;
            case GameActionEvent.PreviousGame:
                PlayerData.currentLevel.Value--;
                StartPlayerDataLevel();
                break;
            case GameActionEvent.Surrender:
                HandlerSurrender();
                break;
            case GameActionEvent.Revive:
                HandlerRevive();
                break;
            case GameActionEvent.Undo:
                HandlerUndo();
                break;
            case GameActionEvent.Hint:
                HandlerHint();
                break;
            case GameActionEvent.ForceGameWin:
                CheckBoardWin(true);
                break;
            case GameActionEvent.ForceBoardLose:
                CheckBoardLose(true);
                break;
            case GameActionEvent.SaveGame:
                SaveCurrentGameSave();
                break;
            case GameActionEvent.ForceRetry:
                Retry();
                break;
            case GameActionEvent.SavePlayerData:
                SavePlayerData();
                break;
            case GameActionEvent.Continue:
                HandlerContinue();
                break;
            case GameActionEvent.UpdateDisplayGameData:
                UpdateGameSaveDisplay();
                break;
            case GameActionEvent.ClearMoves:
                ClearMoves();
                break;
            case GameActionEvent.AddSave:
                AddSave();
                break;
            case GameActionEvent.DeleteGameSave:
                DeleteGameSave();
                break;
            case GameActionEvent.SkipTutorial:
                OnSkipTutorial();
                break;
            case GameActionEvent.None:
                break;
            case GameActionEvent.PrepareToReadyToPlay:
                PrepareReadyToPlay();
                break;
            case GameActionEvent.SkipLevel:
                SkipLevel();
                break;
//            case GameActionEvent.BuilderNext:
//                BuilderNext();
//                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }
    
    public virtual void CalculateSessionData()
    {
    }

    public virtual void SkipLevel()
    {
        
    }

   
    // _________________________________
    
    protected virtual void HandlerWin()
    {
        baseGameState = BaseGameState.Win;
        startGameCompositeDisposable.Clear();
        StopInput();
        SubjectController.ClearGameDisposables();
        DuongExtensions.ExecuteInNextFrame(HandlerWinAfter);
        GameEndHandler();
    }
    
    protected virtual void HandlerWinAfter()
    {
        CalculateUpdateQuest();
        Pause.Value = true;
        ScreenRoot.popupManager.ShowPopup<PopupWin>();
    }
    
    protected virtual void HandlerBoardLose()
    {
        baseGameState = BaseGameState.BoardLose;
        StopInput();
        foreach (var onGameOverAnim in binding.onGameOver)
            onGameOverAnim.StartAction();
        if(gameUi != null)
            foreach (var baseRxAnimator in gameUi.BindingUi.onGameOver)
                baseRxAnimator.StartAction();
        Observable.FromCoroutine(IeFinishingBoardLose)
            .Subscribe();
    }
    
    
    protected virtual IEnumerator IeFinishingWin()
    {
        Pause.Value = true;
        while (ScreenRoot.popupManager.AnyPopupShow)
            yield return null;
        if (!ScreenRoot.isPlayScreen)
            yield break;
        ScreenRoot.TurnRayCast(false);
        foreach (var bindingOnWinAnim in binding.onWin)
        {
            if (bindingOnWinAnim == null)
            {
                Debug.LogError("bindingOnWinAnim null");
                continue;
            }
            bindingOnWinAnim.StartAction();
        }
        if(gameUi != null)
            foreach (var baseRxAnimator in gameUi.BindingUi.onWin)
                baseRxAnimator.StartAction();

        var parameter = GetFinishGameParameter();
        dataProcessing.HandlerFinishGameData(parameter);
        
        if (gameUi != null && gameUi.FinishingObservableWin != null)
        {
            yield return gameUi.FinishingObservableWin.Observable(parameter).ToYieldInstruction();
            gameUi.FinishingObservableWin.gameObject.SetActive(false);
        }

        var delay = BaseCurrentGameSetting.delayToShowPopupWin;
        if (delay > 0)
            yield return new WaitForSecondsRealtime(delay);
        ScreenRoot.TurnRayCast(true);
        SetBoardFinish(true);
        HandlerWin();
        Debug.Log("force win");
    }
    
    protected virtual IEnumerator IeFinishingBoardLose()
    {
        ScreenRoot.TurnRayCast(false);
        if (gameUi != null && gameUi.FinishingObservableLose != null)
        {
            yield return gameUi.FinishingObservableLose.Ie(GetLoseGameParameter());
            ScreenRoot.TurnRayCast(true);
            HandlerGameOver(); 
        }
        else
        {
            yield return null;
            ScreenRoot.TurnRayCast(true);
            HandlerGameOver(); 
        }
    }

    protected virtual int[] GetFinishGameParameter() => null;
    protected virtual int[] GetLoseGameParameter() => null;
    
    protected virtual void HandlerGameOver()
    {
        GameEndHandler();
        baseGameState = BaseGameState.GameOver;
        startGameCompositeDisposable.Clear();
        SubjectController.ClearGameDisposables();
        DeleteGameSave();
        HandlerGameOverAfter();
    }
    
    protected virtual void HandlerGameOverAfter()
    {
        Pause.Value = true;
        ScreenRoot.popupManager.ShowPopup<PopupGameOver>();
    }
    
    protected virtual void HandlerContinue()
    {
        baseGameState = BaseGameState.Playing;
        StartInput(0.1f);
    }
    
    // _________________________________

    protected virtual void HandlerNextGame()
    {
        LevelUp();
        StartPlayerDataLevel();
        PlayerData.won = false;
        // show rate popup as retry should be editor-wise than hardcode
//        if (PlayerData.currentLevel.Value == RemoteConfigKey.game_play_level_pass_to_rate.GetValueInt())
//        {
//            Pause.Value = true;
//            ScreenRoot.popupManager.ShowPopup<PopupRateUs>().SetCloseAsRetry();
//        }
//        else
//        {
//            StartPlayerDataLevel();
//            Kernel.instance.Resolve<AdsManager>().ShowInterstitial("next_game", true);
//        }
    }


    protected virtual void OnSkipTutorial()
    {
        StopTut(-1);
        Retry();
    }



  

    private void SavePlayerData()
    {
        PlayerData.Save();
        DailyData.Save();
    }


    protected abstract void AddSave();

    protected abstract void ClearMoves();


    public virtual void StartUseShopItem(BaseBaseShopItemView data)
    {
        PendingShopItem.Value = data;
    }

    public virtual void StartUseShopItemBeforeBuy(BaseBaseShopItemView data)
    {
        if (ToBuyShopItem.Value != null)
        {
            ClearUseShopItem(false, false);
        }

        ToBuyShopItem.Value = data;
    }

    public virtual void ClearUseShopItem(bool success,bool spend = true)
    {
        if (success && PendingShopItem.Value != null)
        {
            if (PendingShopItem.Value.GetShopItem().ignoreSave)
            {
                PlayerData.IgnoreSaveUntilTurnOn();
            }

            if (spend)
            {
                if (PendingShopItem.Value.IsBoosterItem)
                {
                    PlayerData.UseItem(PendingShopItem.Value.Key());
                }
                else
                {
                    PendingShopItem.Value.TryToSpend();
                }
            }
        }

        if (success && ToBuyShopItem.Value != null)
        {
            if (ToBuyShopItem.Value.GetShopItem().ignoreSave)
            {
                PlayerData.IgnoreSaveUntilTurnOn();
            }

            if (spend)
            {
                if (ToBuyShopItem.Value.IsBoosterItem)
                {
                    PlayerData.UseItem(ToBuyShopItem.Value.Key());
                }
                else
                {
                    ToBuyShopItem.Value.TryToSpend();
                }
            }
        }

        PendingShopItem.Value = null;
        ToBuyShopItem.Value = null;
    }

    public virtual void HandlerAction(GameActionEvent action)
    {
    }


    public abstract bool GetCustomCondition(int customGameCondition);

    public abstract bool DoGoStraight();

    public abstract Product GetCustomProduct(Product reward);
  
    protected abstract void ProductGameActionEventHandler(ProductAction product);

    protected virtual void OnSubscriptionActivate(int subscriptionKey)
    {
        
    }

    public abstract void HandlerButton(int key,PointParameter parameter);


    public virtual bool IsShopItemValidToShow(int key)
    {
        if (SessionData.shopItemShowTimes[key] > 0)
            return false;
            
        if (key == 0)
            return !AdsManager.NoAds;
   
        return true;
    }

    public virtual void HandlerHint()
    {
    }

    public virtual void HandlerUndo()
    {
        Undo();
    }

    public virtual void HandlerRevive()
    {
        
    }

    public virtual void HandlerSurrender()
    {
        HandlerGameOver();
    }
    
    
    protected virtual void HandlerPause()
    {
        if (!Pause.Value)
        {
            Time.timeScale = 0;
            Pause.Value = true;
            ScreenRoot.popupManager.ShowPopup<PopupPause>();
        }
    }

    public virtual void UnPause()
    {
        if (Pause.Value)
        {
            Time.timeScale = 0;
            Pause.Value = false;
        }
    }

    protected virtual void HandlerResume()
    {
        ScreenRoot.popupManager.CloseAllPopupAsObservable().Subscribe(_ =>
        {
            Pause.Value = false;
            Time.timeScale = 1;
        });
    }

    protected virtual void HandlerQuit()
    {
        StopInput();
        SubjectController.ClearGameDisposables();
    }

    protected virtual void Retry()
    {
        DeleteGameSave();
        binding.onQuitGame.StartAction();
        ScreenRoot.popupManager.CloseCurrentPopupAsObservable()
            .Subscribe(data => { StartPlayerDataLevel(); });
    }


    public virtual bool CustomNoAds() => false;


    public virtual void OnDailyDataCustomPropertyCreated(ref IntReactiveProperty[] customPropertyList)
    {
    }


    public virtual void SkipTutorial(int index, int amount)
    {
        PlayerData.tutorialStep += amount;
    }


    public virtual void OnShopStockFilled(ShopPrice shopItem)
    {
    }

    public virtual void HandlerButtonPlay(bool retry = false)
    {
        LoadAndStartCurrentGameSave();
    }

    public virtual void QuitGameHandler()
    {
        binding.onQuitGame.StartAction();
        GameEndHandler();
    }

    public virtual ValidEnum CheckValidTutorialButton(int tutorialButtonEnum)
    {
        return ValidEnum.NotValid;
    }

    public virtual RectTransform GetGameLocationEnum(int location)
    {
        return null;
    }
    
    public virtual Vector3 GetWorldPos(int location)
    {
        return Vector3.zero;
    }

    public virtual string PlayModeName => "classic";

    protected int BoosterCount = 0;
    public virtual void LogUseBooster(string boosterName,int phase = 0)
    {
        BoosterCount++;
        new Sonat.SonatLogUseBooster()
        {
            name = boosterName,
            level = PlayerData.DisplayCurrentLevelInt,
            mode = PlayModeName,
            phase = phase
        }.Post();
    }


    public virtual void OnLoadedEventDay(int eventType,int eventKey)
    {
        if (eventType >= 0 && eventBinding.eventDaySessionKey.Length > eventType && eventBinding.eventDaySessionKey[eventType].value2)
            eventBinding.eventDaySessionKey[eventType].value.GetSessionDataCustomProperty().Value = eventKey;
//        else
//            Debug.LogError("not handler");
        //    SessionData.customPropertyList[(int)CustomSessionDataProperty.EventKey].Value = eventKey;
    }
    

    [SerializeField] private BuilderRoot builderRoot;
    public BuilderRoot BuilderRoot => builderRoot;
    
    public void AddStock(int i, int value)
    {
        if (builderRoot != null)
        {
            builderRoot.CheckRegister();
            builderRoot.AddStock(i, value);
        }
        else
        {
            Debug.LogError("not found builder root");
        }
    }

    public virtual bool GcCheck(int key, BaseCheckScript script,object parameter = null)
    {
        return false;
    }

    public virtual void HandlerBuiltInAction(int key)
    {
        switch ((BaseBuiltInAction)key)
        {
            case BaseBuiltInAction.StartGame:
                break;
            case BaseBuiltInAction.FocusUnfinishedBuilder:
                if(builderRoot != null)
                    builderRoot.FocusUnfinishedStage();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, null);
        }
    }
}