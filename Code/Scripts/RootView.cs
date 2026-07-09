using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable InconsistentNaming


public class RootView : Kernel
{
    public static RootView rootView;
    public static readonly IntReactiveProperty testEnable = new IntReactiveProperty();
    public BaseGameController gameController;
    public ScreenRoot screenRoot;
    public AudioManager audioManager;
    public PoolManager poolManager;
    public bool pauseMoving;
    public IntReactiveProperty onTutorial = new IntReactiveProperty();
    public IntReactiveProperty gameState = new IntReactiveProperty();
    private GameSetting _gameSetting;
    private ShopDatabase _shopDatabase;

    public GameSetting GameSetting
    {
        get
        {
            if (_gameSetting == null)
                _gameSetting = GetDatabase<GameSetting>();
            return _gameSetting;
        }
    }

    public ShopDatabase ShopDatabase
    {
        get
        {
            if (_shopDatabase == null)
                _shopDatabase = GetDatabase<ShopDatabase>();
            return _shopDatabase;
        }
    }

    public override void Awake()
    {
        rootView = this;
        base.Awake();
        gameController.CheckRegister();
        audioManager.CheckRegister();
        poolManager.CheckRegister();
        screenRoot.Setup();
        gameController.CheckShowBannerAds();
        Resolve<FireBaseController>().RemoteConfigReady.Action += gameController.OnRemoteConfigFetched;
        gameController.OnRemoteConfigFetched(false);

    }
    
    protected override void LoadParameterValueRetriever()
    {
        ParameterValueRetriever = gameController;
    }


    private static IEventAggregator _eventAggregator;

    public static IEventAggregator EventAggregator
    {
        get { return _eventAggregator ??= new EventAggregator(); }
        set => _eventAggregator = value;
    }
    
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();

        SubjectController.UpdateUserProperty.Subscribe(data => gameController.UpdateUserProperty());
        DuongKernel.EventKernelLoaded.OnNext(true);
        BaseFrameworkView.PlayerData.openTimes++;

        Resolve<BasePurchaser>().onInAppPurchased.Action += data =>
        {
            BaseFrameworkView.PlayerData.Save();
            BaseFrameworkView.DailyData.Save();
        };

        ResolveDirty<AdsManager>().onVideoRewarded.Action += () =>
        {
            1f.Timer(() =>
            {
                BaseFrameworkView.FillQuest((int) BaseQuestEnum.WatchAds,1);
                BaseFrameworkView.PlayerData.Save();
                BaseFrameworkView.DailyData.Save();
            });
        };

        BaseFrameworkView.DailyData.dailyQuestChecker.DataUpdated.Subscribe(data =>
        {
            SubjectController.CheckDailyQuestCompleteEvent.OnNext(true);
        });
        BaseFrameworkView.DailyData.dailyQuestData.DataUpdated.Subscribe(index =>
        {
            if (!BaseFrameworkView.DailyData.dailyQuestNewlyFinished[index])
                if (GameSetting.IsQuestFinish(index))
                {
                    BaseFrameworkView.DailyData.dailyQuestNewlyFinished[index] = true;
                    SubjectController.InGameNotification.OnNext(gameController.GetQuestNotification(index));
                }
            SubjectController.CheckDailyQuestCompleteEvent.OnNext(true);
        });
    }

    public void LogShopItem(int shopItemKey, IList<ShopPrice> prices)
    {
        LogShopItem("ShopItem_"+BuiltInEnumType.ShopItemKey.GetName(shopItemKey),prices);
    }

    [SerializeField] private bool duongLogVirtualCurrency;

    private void LogShopItem(string shopItemName, IList<ShopPrice> prices)
    {
        var price = prices[0];
        var shopKeyName = shopItemName;
        var fireBaseController = Resolve<FireBaseController>();
        fireBaseController.LogEvent("in_game_purchase",
            new LogParameter("name", shopKeyName));

        if (duongLogVirtualCurrency)
            if (price.quantity == Quantity.Diamond || price.quantity == Quantity.Gold)
                fireBaseController.EventSpendVirtualCurrency(shopKeyName, price.amount,
                    price.quantity.ToString().ToLower());
        Debug.Log("duong : LogShopItem:" + shopKeyName);
    }

 
    public virtual T GetProperty<T>(string propertyName)
    {
        var getField = GetType().GetField(propertyName);
        if (getField != null)
            return (T) getField.GetValue(this);

        var getProperty = GetType().GetProperty(propertyName);
        if (getProperty == null)
            return GetType().GetPrivateFieldValue<T>(propertyName, this);

        return (T) getProperty.GetValue(this);
    }

    public IntReactiveProperty GetProperty(string propertyName)
    {
        var getField = GetType().GetField(propertyName);
        if (getField != null)
        {
            if (getField.FieldType.IsAssignableFrom(typeof(ReactiveProperty<>)))
            {
                Debug.LogError("Error : " + propertyName + " is not a Property", gameObject);
                return null;
            }
            return getField.GetValue(this) as IntReactiveProperty;
        }

        
        var getProperty = GetType().GetProperty(propertyName);
        if (getProperty == null)
        {
            Debug.LogError("Error : " + propertyName + " is not a Property/Field", gameObject);

            return null;
        }
       
        if (getProperty.PropertyType.IsAssignableFrom(typeof(ReactiveProperty<>)))
        {
            Debug.LogError("Error : " + propertyName + " is not a Property", gameObject);
            return null;
        }
        return getProperty.GetValue(this) as IntReactiveProperty;
    }
    
    public FloatReactiveProperty GetPropertyFloat(string propertyName)
    {
        var getField = GetType().GetField(propertyName);
        if (getField != null)
        {
            if (getField.FieldType.IsAssignableFrom(typeof(ReactiveProperty<>)))
            {
                Debug.LogError("Error : " + propertyName + " is not a Property", gameObject);
                return null;
            }
            return getField.GetValue(this) as FloatReactiveProperty;
        }

        
        var getProperty = GetType().GetProperty(propertyName);
        if (getProperty == null)
        {
            Debug.LogError("Error : " + propertyName + " is not a Property/Field", gameObject);

            return null;
        }
       
        if (getProperty.PropertyType.IsAssignableFrom(typeof(ReactiveProperty<>)))
        {
            Debug.LogError("Error : " + propertyName + " is not a Property", gameObject);
            return null;
        }
        return getProperty.GetValue(this) as FloatReactiveProperty;
    }

    protected override void OnUpdateRemote(Type type)
    {
        base.OnUpdateRemote(type);
        if (type == typeof(GameSetting))
            gameController.ClearGameSetting();
    }


#if UNITY_EDITOR
    [MyButtonString(nameof(ShowRemoteConfigKeyInt))]
    public string testRemote;

    public void ShowRemoteConfigKeyInt(string key)
    {
        Debug.Log(key +" has key= "+RemoteConfigHelper.HasKey(key));
        Debug.Log(key +" has value= "+ RemoteConfigHelper.GetValueInt(key));
    }
#endif
  
    
}