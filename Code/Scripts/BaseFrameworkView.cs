using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public enum ReactiveDataEnum
{
    PlayerData,
    DailyData,
    WeeklyData,
    SessionData,
    WaveData,
}

public abstract class BaseFrameworkView : BaseView
{
    public static int[] SharedData = new int[100];
    public static object[] SharedObject = new object[100];
    
    public Product SharedObjectProduct1
    {
        get => (Product) SharedObject[0];
        set => SharedObject[0] = value;
    }
    public Product SharedObjectProduct2
    {
        get => (Product) SharedObject[0];
        set => SharedObject[0] = value;
    }
    public Product SharedObjectProduct3
    {
        get => (Product) SharedObject[0];
        set => SharedObject[0] = value;
    }
    
    public Product SharedProduct
    {
        get =>
            new Product()
            {
                quantity = (Quantity) SharedData[(int) BaseFrameworkSharedData.ProductQuantity],
                index = SharedData[(int) BaseFrameworkSharedData.ProductIndex],
                amount = SharedData[(int) BaseFrameworkSharedData.ProductAmount],
                parameter = SharedData[(int) BaseFrameworkSharedData.ProductParameter1],
                parameter2 = SharedData[(int) BaseFrameworkSharedData.ProductParameter2],
            };
        set => SetSharedProductData(value.Quantity, value.index, value.amount, value.parameter, value.parameter2);
    }

    private static void SetSharedProductData(int quantity,int index,int amount,int parameter1,int parameter2 = 0
        ,int quantityKey = (int)BaseFrameworkSharedData.ProductQuantity
        ,int indexKey = (int)BaseFrameworkSharedData.ProductIndex
        ,int amountKey = (int)BaseFrameworkSharedData.ProductAmount
        ,int parameterKey = (int)BaseFrameworkSharedData.ProductParameter1
        ,int parameter2Key = (int)BaseFrameworkSharedData.ProductParameter2
        )
    {
        SharedData[quantityKey] = quantity;
        SharedData[indexKey] = index;
        SharedData[amountKey] = amount;
        SharedData[parameterKey] = parameter1;
        SharedData[parameter2Key] = parameter2;
    }
    
    private static void SetSharedProductData(Quantity quantity,int index,int amount,int parameter1,int parameter2 = 0
        ,int quantityKey = (int)BaseFrameworkSharedData.ProductQuantity
        ,int indexKey = (int)BaseFrameworkSharedData.ProductIndex
        ,int amountKey = (int)BaseFrameworkSharedData.ProductAmount
        ,int parameterKey = (int)BaseFrameworkSharedData.ProductParameter1
        ,int parameter2Key = (int)BaseFrameworkSharedData.ProductParameter2
    )
    {
        SetSharedProductData((int) quantity, index, amount, parameter1, parameter2, quantityKey, indexKey, amountKey,
            parameterKey, parameter2Key);
    }
    
    private static void SetSharedProductData(Product product
        ,int quantityKey = (int)BaseFrameworkSharedData.ProductQuantity
        ,int indexKey = (int)BaseFrameworkSharedData.ProductQuantity
        ,int amountKey = (int)BaseFrameworkSharedData.ProductQuantity
        ,int parameterKey = (int)BaseFrameworkSharedData.ProductParameter1
        ,int parameter2Key = (int)BaseFrameworkSharedData.ProductParameter2
    )
    {
        SetSharedProductData((int) product.Quantity, product.index, product.amount, product.parameter, product.parameter2, quantityKey, indexKey, amountKey,
            parameterKey, parameter2Key);
    }
    
#if UNITY_EDITOR
    [SerializeField] private TestFunction testFunction = new TestFunction()
    {
        method = "functionName"
    };
    
    [MyButtonInt(nameof(LogCustom), nameof(LogCustomInt), nameof(Rename))] [SerializeField]
    protected int logAndRename;
  
#endif

    public static BaseGameController BaseGameController => RootView.rootView.gameController;
    protected static int LevelInt => BaseGameController.Level;

    public virtual void LogCustom()
    {
    }

    public static void MyDebugBreak()
    {
        if(TestingSettings.DebugBreak)
            Debug.Break();
    }
    
    public static void MyDebugBreak1()
    {
        if(TestingSettings.DebugBreak1)
            Debug.Break();
    }

    public virtual void LogCustomInt(int value)
    {
    }

    public virtual void Rename()
    {
        gameObject.name = GetName();
    }

    public static void FillQuest(int key, int value)
    {
        DailyData.dailyQuestData[key] += value;
        PlayerData.achievementData[key] += value;
        SubjectController.NotifyEvent.OnNext(NotifyEnum.Achievement);
        SubjectController.NotifyEvent.OnNext(NotifyEnum.DailyQuest);
    }
    
    public static void UpdateQuest(int key, int value)
    {
        DailyData.dailyQuestData[key] = value;
        PlayerData.achievementData[key] = value;
        SubjectController.NotifyEvent.OnNext(NotifyEnum.Achievement);
        SubjectController.NotifyEvent.OnNext(NotifyEnum.DailyQuest);
    }

    protected virtual string GetName() => GetType().Name;

    // rootview
    protected static PoolManager PoolManager => RootView.rootView.poolManager;
    protected static ScreenRoot ScreenRoot => RootView.rootView.screenRoot;

    protected static GameSetting GameSetting => RootView.rootView.GameSetting;

//    protected static RewardAssetDatabase rewardAssetDatabase;
    // database

    // reactive data
    private static ReactivePlayerData _reactivePlayerData;
    private static ReactivePlayerDailyData _reactivePlayerDailyData;
    private static ReactivePlayerWeeklyData _reactivePlayerWeeklyData;
    private static ReactiveSessionData _sessionData;
    private static ReactiveWaveData _waveData;

    public static ReactiveSessionData SessionData
    {
        get
        {
            if (_sessionData == null)
            {
                _sessionData = new ReactiveSessionData();
                _sessionData.Clear();
            }

            return _sessionData;
        }
    }


    public static ReactivePlayerData PlayerData
    {
        get
        {
            if (_reactivePlayerData == null)
                _reactivePlayerData = ReactivePlayerData.Load();
            return _reactivePlayerData;
        }
    }

    public static ReactivePlayerDailyData DailyData
    {
        get
        {
            if (_reactivePlayerDailyData == null)
            {
                _reactivePlayerDailyData = ReactivePlayerDailyData.Load();
                var gs = Kernel.GetDatabase<GameSetting>();
                _reactivePlayerDailyData.dailyQuestData.DataUpdated.Subscribe(key =>
                {
                    _reactivePlayerDailyData.CheckQuestFinish(key);
                    if (gs.AnyDailyQuestFinish())
                        SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
                });
            }

            return _reactivePlayerDailyData;
        }
    }

    public static ReactivePlayerWeeklyData WeeklyData
    {
        get
        {
            if (_reactivePlayerWeeklyData == null)
                _reactivePlayerWeeklyData = ReactivePlayerWeeklyData.Load();
            return _reactivePlayerWeeklyData;
        }
    }

    public static ReactiveWaveData WaveData
    {
        get
        {
            if (_waveData == null)
                _waveData = new ReactiveWaveData();
            return _waveData;
        }
    }


    protected static readonly Subject<TweenCommand> CoinTween = new Subject<TweenCommand>();
    protected static readonly Subject<TweenCommand> GemTween = new Subject<TweenCommand>();
    protected static readonly Subject<TweenCommand> ScoreTween = new Subject<TweenCommand>();

    protected void TweenCoin(TextTweenType type, int value)
    {
        CoinTween.OnNext(new TweenCommand()
        {
            TweenType = type,
            Value = value
        });
    }

    protected void TweenGem(TextTweenType type, int value)
    {
        GemTween.OnNext(new TweenCommand()
        {
            TweenType = type,
            Value = value
        });
    }

    protected void TweenScore(TextTweenType type, int value)
    {
        ScoreTween.OnNext(new TweenCommand()
        {
            TweenType = type,
            Value = value
        });
    }

    protected List<ClaimedProduct> TakeSubscriptionReward(int key)
    {
        var subscriptionItem = RootView.rootView.ShopDatabase.GetSubscription(key);
        var claimed = PlayerData.Claim(subscriptionItem.rewards);
        DailyData.subscriptionTake[key] = true;
        SubjectController.SubscriptionActivated.OnNext(key);
        return claimed;
    }
    
    protected bool IsSubscriptionActive(int key) => PlayerData.subscription.Exist(key) && PlayerData.subscription[key] > 0;
    protected bool IsSubscriptionReadyTake(int key) => IsSubscriptionActive(key) && DailyData.subscriptionTake[key] == false;

    public static IntReactiveProperty GetPropertyByName(string name, ReactiveDataEnum reactiveDataEnum)
    {
        FieldInfo getProperty;
        switch (reactiveDataEnum)
        {
            case ReactiveDataEnum.PlayerData:
                getProperty = typeof(ReactivePlayerData).GetField(name);
                return (IntReactiveProperty) getProperty.GetValue(PlayerData);
            case ReactiveDataEnum.DailyData:
                getProperty = typeof(ReactivePlayerDailyData).GetField(name);
                return (IntReactiveProperty) getProperty.GetValue(DailyData);
            case ReactiveDataEnum.WeeklyData:
                getProperty = typeof(ReactivePlayerWeeklyData).GetField(name);
                return (IntReactiveProperty) getProperty.GetValue(WeeklyData);
            case ReactiveDataEnum.SessionData:
                getProperty = typeof(ReactiveSessionData).GetField(name);
                return (IntReactiveProperty) getProperty.GetValue(SessionData);
            case ReactiveDataEnum.WaveData:
                getProperty = typeof(ReactiveWaveData).GetField(name);
                return (IntReactiveProperty) getProperty.GetValue(WaveData);
            default:
                throw new ArgumentOutOfRangeException(nameof(reactiveDataEnum), reactiveDataEnum, null);
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu(nameof(CopyComponentValue))]
    private  void CopyComponentValue()
    {
        Debug.Log(UnityEditor.EditorJsonUtility.ToJson(this));
        System.IO.File.WriteAllText("D:/serialzied.json",UnityEditor.EditorJsonUtility.ToJson(this));
    }
    
    [ContextMenu(nameof(PasteComponentValue))]
    private  void PasteComponentValue()
    {
        var text =  System.IO.File.ReadAllText("D:/serialzied.json");
        Debug.Log(text);
        UnityEditor.EditorJsonUtility.FromJsonOverwrite(text,this);
    }
    #endif
}