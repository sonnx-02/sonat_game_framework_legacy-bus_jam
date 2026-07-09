using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ReactivePlayerWeeklyData : BaseLoadReactiveData
{
    public IntegerSerializableDictionary adsRewardCounter = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary adsRewardTaken = new IntegerSerializableDictionary();
    public LongSerializableDictionary adsTimeTick = new LongSerializableDictionary();
    public IntegerSerializableDictionary shopPriceStock = new IntegerSerializableDictionary();
    public static int LastEpochWeek
    {
        get => PlayerPrefs.GetInt(nameof(LastEpochWeek));
        set => PlayerPrefs.SetInt(nameof(LastEpochWeek), value);
    }

    private static string FilePath => string.Format("{0}/{1}", Application.persistentDataPath, "PlayerWeeklyData.json");

    public static void DeleteFile()
    {
        File.Delete(FilePath);
    }


    public static bool IsNewWeek()
    {
        return !(File.Exists(FilePath) && DuongExtensions.GetEpochWeek() == LastEpochWeek);
    }

    public static ReactivePlayerWeeklyData Load()
    {
        if (!IsNewWeek())
        {
            string json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<ReactivePlayerWeeklyData>(json);
            data.Validate();
            return data;
        }

        LastEpochWeek = DuongExtensions.GetEpochWeek();
        var weeklyData = new ReactivePlayerWeeklyData();
        weeklyData.Validate();
        return weeklyData;
    }

    public void Save()
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(this));
    }
}