using System;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

[Serializable]
public class ReactivePlayerDailyData : BaseLoadReactiveData
{
    public bool dailyGiftChecked;
    public int onlineGiftStep;
    public IntegerSerializableDictionary dailyQuestData = new IntegerSerializableDictionary();
    public BooleanSerializableDictionary dailyQuestChecker = new BooleanSerializableDictionary();
    public BooleanSerializableDictionary dailyQuestNewlyFinished = new BooleanSerializableDictionary();
    public IntegerSerializableDictionary adsRewardCounter = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary adsRewardTaken = new IntegerSerializableDictionary();
    public LongSerializableDictionary adsTimeTick = new LongSerializableDictionary();
    public IntegerSerializableDictionary shopPriceStock = new IntegerSerializableDictionary();
    public BooleanSerializableDictionary offerChecker = new BooleanSerializableDictionary();
    public IntegerSerializableDictionary shopItemShowTimes = new IntegerSerializableDictionary();
    public BooleanSerializableDictionary offerBought = new BooleanSerializableDictionary();
    public BooleanSerializableDictionary subscriptionTake = new BooleanSerializableDictionary();
    public BooleanSerializableDictionary gameChecker = new BooleanSerializableDictionary();

    public List<int> checks = new List<int>();
    public bool gameControllerChecked;


    protected override void NewCustomPropertyList()
    {
        base.NewCustomPropertyList();
        RootView.rootView.gameController.OnDailyDataCustomPropertyCreated(ref customPropertyList);
    }

    public override void Clear()
    {
        base.Clear();
        gameControllerChecked = false;
        dailyGiftChecked = false;
        onlineGiftStep = 0;
        subscriptionTake.ClearDict();
        dailyQuestData.ClearDict();
        offerChecker.ClearDict();
        offerBought.ClearDict();
        dailyQuestChecker.ClearDict();
        dailyQuestNewlyFinished.ClearDict();
        adsRewardCounter.ClearDict();
        adsRewardTaken.ClearDict();
        adsTimeTick.ClearDict();
        shopPriceStock.ClearDict();
        shopItemShowTimes.ClearDict();
        SubjectController.FireEventToUpdateAdsAndQuest();
        SubjectController.OnDailyDataReset.OnNext(true);
        BaseFrameworkView.PlayerData.days++;
        UIDebugLog.Log("Clear "+GetType());
    }
    
    private static int LastEpochDate
    {
        get => PlayerPrefs.GetInt(nameof(LastEpochDate));
        set => PlayerPrefs.SetInt(nameof(LastEpochDate), value);
    }

    private static string FilePath => string.Format("{0}/{1}", Application.persistentDataPath, "PlayerDailyData.json");

    public void CheckNewDay()
    {
        Save();
        if (IsNewDay())
        {
            BaseFrameworkView.PlayerData.OnNewDay();
            Clear();
        }
    }
#if UNITY_EDITOR
    private static bool newDayOnce;
#endif

    public static bool ForceNewDay
    {
        get => PlayerPrefs.GetInt("ForeNewDay") == 1;
        set => PlayerPrefs.SetInt("ForeNewDay",value ? 1 : 0);
    }

#if UNITY_EDITOR
    private static bool _logged;
#endif
    private static bool IsNewDay()
    {
        #if UNITY_EDITOR
        if (TestingSettings.AlwaysNewDay && !newDayOnce)
        {
            if (!_logged)
            {
                _logged = true;
                Debug.LogError("TestingSettings.AlwaysNewDay");
            }
            
            newDayOnce = true;
            return true;
        }
        #endif

        if (ForceNewDay)
        {
            ForceNewDay = false;
            return true;
        }
        
        return !(File.Exists(FilePath)  && SdkExtenstions.GetEpochDate() == LastEpochDate);
    }

    public static void DeleteFile()
    {
        File.Delete(FilePath);
    }

    public static ReactivePlayerDailyData Load()
    {
        var dailyData = GetData();
        dailyData.Save();
        dailyData.Validate();
        return dailyData;
    }

    private static ReactivePlayerDailyData GetData()
    {
        if (!IsNewDay())
        {
            if (!File.Exists(FilePath))
            {
                return NewData();
            }

            
            string json = File.ReadAllText(FilePath);
            
            if (string.IsNullOrEmpty(json))
                return NewData();
            
            try
            {
                return JsonUtility.FromJson<ReactivePlayerDailyData>(json);
            }
            catch (Exception e)
            {
                return NewData();
            }
            
         
        }

        return NewData();
    }

    private static ReactivePlayerDailyData NewData()
    {
        BaseFrameworkView.PlayerData.OnNewDay();
        LastEpochDate = SdkExtenstions.GetEpochDate();
        return new ReactivePlayerDailyData();
    }

    public override void Validate()
    {
        base.Validate();
        if (dailyQuestData == null)
            dailyQuestData = new IntegerSerializableDictionary();
        if (dailyQuestChecker == null)
            dailyQuestChecker = new BooleanSerializableDictionary();
        if (adsRewardCounter == null)
            adsRewardCounter = new IntegerSerializableDictionary();
        if (adsRewardTaken == null)
            adsRewardTaken = new IntegerSerializableDictionary();
        if (adsTimeTick == null)
            adsTimeTick = new LongSerializableDictionary();
        if (shopPriceStock == null)
            shopPriceStock = new IntegerSerializableDictionary();
        if (offerChecker == null)
            offerChecker = new BooleanSerializableDictionary();
        if (shopItemShowTimes == null)
            shopItemShowTimes = new IntegerSerializableDictionary();
    }

    public void Save()
    {
        if (!IgnoreSave)
        {
            var saveStr = JsonUtility.ToJson(this);
            File.WriteAllText(FilePath, saveStr);
        }
    }

    public bool FinishOnlineGift()
    {
        return onlineGiftStep >= Kernel.GetDatabase<GameSetting>().onlineRewards.Length;
    }

    public void CheckQuestFinish(int key)
    {
        var finish = Kernel.GetDatabase<GameSetting>().IsQuestFinish(key);
        if (Kernel.GetDatabase<GameSetting>().IsQuestFinish(key) && !checks.Contains(key))
        {
            checks.Add(key);
            BaseFrameworkView.FillQuest((int)BaseQuestEnum.CompleteQuest,1);
        }
    }
}