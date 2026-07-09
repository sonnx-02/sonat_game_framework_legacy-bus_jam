using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SortingOrderItem
{
    [IndexAsEnum(BuiltInEnumType.SortingEnum)]
    public int key;
    public string layer;
    public int order;
}

[Serializable]
public class GameSetting_Time
{
    public float delayStartGame;
    public float delayStartInput;

    [ArrayLabelIndexAsEnum(BuiltInEnumType.CustomTimeSettingEnum)]
    public float[] customs;
}


[Serializable]
public class CustomDailyGift
{
    [FormerlySerializedAs("giftKey")] [IndexAsEnum(BuiltInEnumType.CustomDailyDataProperty)]
    public int key;
    public Product[] rewards;
}


public enum GameTime
{
    OnStartGame,
    OnHomeScreen,
    BeforePopupLose,
}

public enum LevelCondition
{
    AfterLevel,
    EachLevel,
    EachLevelAfter,
    SpecificLevels,
    ExceptSpecificLevels,
}

[Serializable]
public class LevelConditionCheck
{
    [SerializeField] private LevelCondition condition;
    [SerializeField] private int value;
    [SerializeField] private int value2;
    [SerializeField] private int[] specifics;

    public bool CheckCondition(int levelInt)
    {
        return CheckCondition(condition, levelInt);
    }

    private bool CheckCondition(LevelCondition other, int levelInt)
    {
        switch (other)
        {
            case LevelCondition.AfterLevel:
               return value >= levelInt;
            case LevelCondition.EachLevel:
                return value >= levelInt;
            case LevelCondition.SpecificLevels:
               foreach (var specific in specifics)
                   if (specific == levelInt)
                       return true;
               break;
            case LevelCondition.ExceptSpecificLevels:
                return !CheckCondition(LevelCondition.SpecificLevels, levelInt);
            case LevelCondition.EachLevelAfter:
                return (levelInt - value) % value2 == 0;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }
}

[Serializable]
public class ShowBannerCondition
{
    public ShowBannerConditionEnum conditionEnum = ShowBannerConditionEnum.TutorialStep;
    public int tutorialStep = -1;
    public PlayerPrefRemoteInt levelCondition = new PlayerPrefRemoteInt(RemoteConfigKey.start_level_show_banner,1);

    public bool Valid(int tutorialStepValue, int levelValue)
    {
        #if UNITY_EDITOR
        if(TestingSettings.DebugBreak)
            Debug.Log(levelCondition.Value);
        #endif
        switch (conditionEnum)
        {
            case ShowBannerConditionEnum.None:
                return true;
            case ShowBannerConditionEnum.TutorialStep:
                return tutorialStepValue <= tutorialStep;
            case ShowBannerConditionEnum.Level:
                return levelValue >= levelCondition.Value;
            case ShowBannerConditionEnum.TutorialAndLevel:
                return tutorialStepValue <= tutorialStep && levelValue >= levelCondition.Value;
            case ShowBannerConditionEnum.TutorialOrLevel:
                return tutorialStepValue <= tutorialStep || levelValue >= levelCondition.Value;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return true;
    }

    public enum ShowBannerConditionEnum
    {
        Manual = -1,
        None = 0,
        TutorialStep = 1,
        Level = 2,
        TutorialAndLevel = 3,
        TutorialOrLevel = 4,
    }
}



[Serializable]
public class PackShowSetting
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int key;
    public GameTime time;
    public LevelConditionCheck levelConditionCheck;
    public float delayCheck;
}

[Serializable]
public class EventDescriptor
{
    [IndexAsEnum(BuiltInEnumType.EventEnum)]
    public int eventKey;
    public int startEpochDate;
    public int activeDays;
    public Product[] rewards;// parameter is condition
    public string eventName;
}

[Serializable]
public class MetaDataSetting
{
    [IndexAsEnum(BuiltInEnumType.Quantity)]
    public IntString[] quantityNames;
    public string notEnoughMessageFormat ="You don't have enough {0}. Do you want to buy more?";
    public string notEnoughMessageFormat2 ="Not enough {0}";
}


[CreateAssetMenu(fileName = nameof(GameSetting) + ".asset", menuName = "Duong/" + nameof(GameSetting))]
public class GameSetting : ScriptableObject
{
    [ContextMenu("test")]
    void test()
    {
        for (var i = 0; i < events.Length; i++)
        {
            events[i].startEpochDate /= 1000000;
        }
    }

    [ArrayElementIndexTitleAttribute2(BuiltInEnumType.GameSettingSharedData)]
    public int[] sharedSetting;

    public MetaDataSetting metaDataSetting;
    public GameSetting_Time time;
    public ShowBannerCondition showBannerCondition;
    
#if UNITY_EDITOR
    [SerializeField] [LabelHeader("Product Parameter is condition", 1, 0, 1)]  private bool header1;
#endif
    [ArrayElementIndexTitleAttribute(nameof(DailyQuestItem.key), BuiltInEnumType.QuestEnum)]
    public DailyQuestItem[] dailyQuestItems;

    [ArrayElementIndexTitleAttribute(nameof(AchievementItem.key), BuiltInEnumType.QuestEnum)]
    public AchievementItem[] achievementItems;

    public Product[] dailyCheckInRewards; // gem
    public Product[] onlineRewards;
   
    [ArrayElementIndexTitleAttribute(nameof(CustomDailyGift.key), BuiltInEnumType.CustomSessionDataProperty)]
    public CustomDailyGift[] customDailyGifts;
        
//    public float moveSpeed = 1;
//    public Ease moveEase = Ease.OutBack;
    public int openTimesToOfferShow;
    
    public int playTimesToOfferShow;

//    public Point secondsToShowAdBreakRandom = new Point(90,180);
    public Product adBreakReward;
//    public Vector2Int size = new Vector2Int(10, 10);

    [ArrayElementIndexTitle(nameof(SortingOrderItem.key), BuiltInEnumType.SortingEnum)]
    public SortingOrderItem[] sortingOrders;

    public PackShowSetting[] packShowAfterInitGame;
  
    [IndexAsEnum(BuiltInEnumType.EventEnum)]
    public int[] existEvents;
    [ArrayElementIndexTitle(nameof(EventDescriptor.eventKey), BuiltInEnumType.EventEnum)]
    public EventDescriptor[] events;

    [IndexAsEnum(BuiltInEnumType.EventEnum)]
    public int[] existEvents2;
    [ArrayElementIndexTitle(nameof(EventDescriptor.eventKey), BuiltInEnumType.EventEnum)]
    public EventDescriptor[] events2;
    
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private IntInt[] soundVibrates;
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private  IntInt[] soundVibratesIos;

    #if UNITY_ANDROID
    public IntInt[] SoundVibrates => soundVibrates;
#else
    public IntInt[] SoundVibrates => soundVibratesIos;
    #endif


    public SortingOrderItem GetSortingOrder(int key)
    {
        foreach (var sortingOrderItem in sortingOrders)
            if (sortingOrderItem.key == key)
                return sortingOrderItem;

        return null;
    }

    public Vector2Int GetSortingOrderData(int key)
    {
        foreach (var sortingOrderItem in sortingOrders)
            if (sortingOrderItem.key == key)
                return new Vector2Int(sortingOrderItem.order, SortingLayer.NameToID(sortingOrderItem.layer));

        return Vector2Int.zero;
    }

#if UNITY_EDITOR
    public bool alwaysIsNewDayTestEditor;
#else
    public bool alwaysIsNewDay => false;
#endif

    public bool AnyDailyQuestFinish()
    {
        foreach (var dailyQuestItem in dailyQuestItems)
        {
            if (dailyQuestItem.GetCurrentState() == QuestState.Complete)
                return true;
        }

        return false;
    }


    public bool IsQuestFinish(int key)
    {
        foreach (var dailyQuestItem in dailyQuestItems)
        {
            if (key == dailyQuestItem.key)
            {
                var state = dailyQuestItem.GetCurrentState();
                if (state == QuestState.Complete)
                    return true;
            }
        }

        return false;
    }

    [MyButton(nameof(PrintJson))] [SerializeField]
    private int testShow;

    public int autoStartWhenLevelBelow = 10;
    public int warningTime = 30;
    public int countDownFrozen = 3;
    public int countDownTied = 3;

	public void PrintJson()
    {
        Debug.Log(JsonUtility.ToJson(this));
    }

    public CustomDailyGift GetCustomDailyGift(int key)
    {
        foreach (var customDailyGift in customDailyGifts)
        {
            if (customDailyGift.key == key)
                return customDailyGift;
        }

        return null;
    }
}