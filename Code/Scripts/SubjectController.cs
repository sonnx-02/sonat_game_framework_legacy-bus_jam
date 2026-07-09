using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public enum WaveEvent
{
    PlayerDestroyed,
    FinishWave,
    Firework,
}

public enum GameActionEvent
{
    None = -1,
    Pause = 0,
    Resume = 1,
    Quit = 2,
    Retry = 3,
    NextGame = 4,
    PreviousGame = 5,
    Surrender = 6,
    Revive = 7,
    Undo = 8,
    Hint = 9,
    ForceGameWin = 10,
    ForceBoardLose = 11,
    SaveGame = 12,
    SavePlayerData = 13,
    ForceRetry = 14,
    Continue = 15,
    DeleteGameSave = 16,
    UpdateDisplayGameData = 17,
    ClearMoves = 18,
    AddSave = 19,
    SkipTutorial = 20,
    PrepareToReadyToPlay = 21,
    SkipLevel = 22,
//    BuilderNext = 23,
}

public enum UpdateQuantityEvent
{
    Update,
    Setup
}

public enum CustomUpdateEvent
{
    None = 0,
    PlayerProfile = 1,
    LeaderBoard = 2,
    Avatar = 3,
    DailyQuest = 4,
    Achievement = 5,
    Event = 6,
}

[Serializable]
public struct BroadcastMessage
{
    public int index;
    public string message;
}

[Serializable]
public struct TextUpdateData
{
    public int key;
    public string message;

    public TextUpdateData(int k, string mess)
    {
        key = k;
        message = mess;
    }
}

[Serializable]
public struct IndexToggle
{
    public int key;
    public bool value;

    public IndexToggle(int index, bool value)
    {
        this.key = index;
        this.value = value;
    }
}

[Serializable]
public struct IndexValue
{
    public int index;
    public float value;

    public IndexValue(int index, int value)
    {
        this.index = index;
        this.value = value;
    }
}


[Serializable]
public class InGameNotification
{
    public string name;
    public string message;
}

public enum GameMainEvent
{
    StartGame,
    QuitGame,
    GoToHome,
    Win,
    Lose,
    Pause,
    Resume,
}

public enum EquipDataUpdateType
{
    Undefined,
    NewlyBought,
    Switch
}

[Serializable]
public class EquipDataUpdate
{
    public int key;
    public int index;
    public EquipDataUpdateType updateType;

    public EquipDataUpdate(int key, int index, EquipDataUpdateType updateType)
    {
        this.key = key;
        this.index = index;
        this.updateType = updateType;
    }
}


public static class SubjectController
{
    public static readonly Subject<GameMainEvent> OnGameMainEvent = new Subject<GameMainEvent>();

//    public static readonly Subject<SubcriptionStatus> OnSubscription = new Subject<SubcriptionStatus>();
//    public static readonly Subject<bool> OnClearScreen = new Subject<bool>();
    public static readonly Subject<ProductAction> ProductGameActionEvent = new Subject<ProductAction>();
    public static readonly Subject<ProductAction> ProductSystemActionEvent = new Subject<ProductAction>();
    public static readonly Subject<PointParameter> OnPointParameterCallBack = new Subject<PointParameter>(); // Value AfterInCallBackEnum
    public static readonly Subject<GameActionEvent> GameActionEvent = new Subject<GameActionEvent>();
    public static readonly Subject<NotifyEnum> NotifyEvent = new Subject<NotifyEnum>();
    public static readonly Subject<bool> NotifyCustomGameCondition = new Subject<bool>();
    public static readonly Subject<bool> NotifyCustomProductPrice = new Subject<bool>();
    public static readonly Subject<bool> CheckDailyQuestCompleteEvent = new Subject<bool>();

//    public static readonly Subject<StoreProductKey> OnShopItemBought = new Subject<StoreProductKey>();
    public static readonly Subject<WaveEvent> OnWaveEvent = new Subject<WaveEvent>();

    public static readonly Subject<bool> OnAdsItemKeyDataUpdated = new Subject<bool>();

//    public static readonly Subject<CustomAdsCondition> OnCustomAdsConditionUpdated = new Subject<CustomAdsCondition>();
//    public static readonly Subject<UpdateQuantityEvent> OnQuantityStatusChanged = new Subject<UpdateQuantityEvent>();
    public static readonly Subject<bool> ShopPriceStockChangedEvent = new Subject<bool>();
    public static readonly Subject<bool> FireBaseRemoteReadyEvent = new Subject<bool>();
    public static readonly Subject<bool> CheckWin = new Subject<bool>();
    public static readonly Subject<bool> OnExchanged = new Subject<bool>(); // in app key
    public static readonly Subject<int> OnAdsRewarded = new Subject<int>();
    public static readonly Subject<int> OnShopItemBought = new Subject<int>(); // in app key
    public static readonly Subject<int> InAppPurchasedEvent = new Subject<int>(); // in app key
    public static readonly Subject<int> StartLevel = new Subject<int>();
    public static readonly Subject<int> CustomEvent = new Subject<int>();
    public static readonly Subject<int> OfferActivated = new Subject<int>();
    public static readonly Subject<int> SubscriptionActivated = new Subject<int>();
    public static readonly Subject<int> GlobalError = new Subject<int>();
   
    // example GameActionEvent : x* 10000 + (int)[GameActionEvent] // x is BuiltIntEnum.GameActionEvent as int // it's is a little bit dirty but its work
    public static readonly Subject<int> UniversalEvent = new Subject<int>(); 


    public static readonly Subject<IndexToggle> ToggleEvent = new Subject<IndexToggle>();
    public static readonly Subject<BroadcastMessage> MessageBroadcastingEvent = new Subject<BroadcastMessage>();
    public static readonly Subject<TextUpdateData> TextUpdater = new Subject<TextUpdateData>();
    public static readonly Subject<CustomUpdateEvent> CustomUpdatedEvent = new Subject<CustomUpdateEvent>();
    public static readonly Subject<BaseDialog> OnDialogClosed = new Subject<BaseDialog>();
    public static readonly Subject<IndexToggle> ToggleLink = new Subject<IndexToggle>();
    public static readonly Subject<bool> UpdateUserProperty = new Subject<bool>();
    public static readonly Subject<InGameNotification> InGameNotification = new Subject<InGameNotification>();
    public static readonly Subject<bool> OnDailyDataReset = new Subject<bool>();
  
    public static readonly Subject<bool> PopupChanged = new Subject<bool>();
    public static readonly Subject<BasePopup> OnPopupShowed = new Subject<BasePopup>();
    public static readonly Subject<BasePopup> OnPopupClosed = new Subject<BasePopup>();
    public static readonly Subject<EquipDataUpdate> OnEquipDataUpdate = new Subject<EquipDataUpdate>();
    public static readonly Subject<Vector2Int> BuilderAddStock = new Subject<Vector2Int>();
    public static readonly Subject<Product> ProductEvent = new Subject<Product>();
    
//    public static readonly Subject<int> RegisterAction = new Subject<int>();

    public static readonly Subject<BaseScreen> ScreenShowed = new Subject<BaseScreen>();
    public static readonly Subject<BaseScreen> ScreenClosed = new Subject<BaseScreen>();
    private static readonly CompositeDisposable ScreenDisposables = new CompositeDisposable();
    private static readonly CompositeDisposable GameDisposables = new CompositeDisposable();


    private static readonly Dictionary<int, Action> DictionaryRegisterAction = new Dictionary<int, Action>();

    public static void RegisterAction(int key, Action action)
    {
        DictionaryRegisterAction[key] = action;
    }

    public static void InvokeRegisteredAction(int key)
    {
        if (DictionaryRegisterAction.ContainsKey(key))
            DictionaryRegisterAction[key].Invoke();
    }

    public static void FireEventToUpdateAdsAndQuest()
    {
        OnAdsItemKeyDataUpdated.OnNext(true);
        NotifyCustomGameCondition.OnNext(true);
        CheckDailyQuestCompleteEvent.OnNext(true);
        NotifyEvent.OnNext(NotifyEnum.All);
    }

    public static IDisposable AddToScreenChangeDisposable(this IDisposable dispose)
    {
        return dispose.AddTo(ScreenDisposables);
    }

    public static IDisposable AddToGameDisposable(this IDisposable dispose)
    {
        return dispose.AddTo(GameDisposables);
    }

    public static void ClearGameDisposables()
    {
        GameDisposables.Clear();
    }

    public static void ClearScreenChangeDisposables()
    {
        ScreenDisposables.Clear();
    }

    public static void OnNext(this GameActionEvent gameActionEvent)
    {
        GameActionEvent.OnNext(gameActionEvent);
    }
}