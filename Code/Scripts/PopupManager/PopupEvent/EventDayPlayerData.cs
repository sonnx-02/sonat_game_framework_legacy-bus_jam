using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
public class EventDayPlayerData : EventDescriptor
{
    public bool inform;
    public int currentStage; // to play
    public int progress; // item collected
    public bool[] claimed; // reward claimed
    public int EventKey => eventKey;
    public string filePath;

    public EventDayPlayerData(EventDescriptor descriptor)
    {
        eventKey = descriptor.eventKey;
        startEpochDate = descriptor.startEpochDate;
        activeDays = descriptor.activeDays;
        rewards = descriptor.rewards.Select(x => x.Clone()).ToArray();
        eventName = descriptor.eventName;
    }

    private static string FilePath => $"{Application.persistentDataPath}/{nameof(EventDayPlayerData)}.json";

    public void Save()
    {
        File.WriteAllText( string.IsNullOrEmpty(filePath) ? FilePath : filePath, JsonUtility.ToJson(this));
    }
    
    public static void DeleteAllFile()
    {
        File.Delete($"{Application.persistentDataPath}/Current{nameof(EventDayPlayerData)}.json");
        File.Delete($"{Application.persistentDataPath}/Special{nameof(EventDayPlayerData)}.json");
        File.Delete($"{Application.persistentDataPath}/{nameof(EventDayPlayerData)}.json");
    }

    public static EventDayPlayerData Load(string fileName)
    {
        var filePath = $"{Application.persistentDataPath}/{fileName}.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            EventDayPlayerData data = JsonUtility.FromJson<EventDayPlayerData>(json);
            if (data != null && data.Validate())
            {
                data.filePath = filePath;
                return data;
            }
        }
        
//        var oldPath = $"{Application.persistentDataPath}/CurrentEventDayPlayerData.json";
//        if (File.Exists( oldPath)) // because of rename
//        {
//            string json = File.ReadAllText(oldPath);
//            EventDayPlayerData data = JsonUtility.FromJson<EventDayPlayerData>(json);
//            if (data != null && data.Validate())
//            {
//                File.Delete(oldPath);
//                return data;
//            }
//        }

        return null;
    }

    private bool CheckUpdateVersion()
    {
        // legacy
        if (rewards == null || rewards.Length == 0)
        {
            var ev = Kernel.GetDatabase<GameSetting>().events;
            foreach (var descriptor in ev)
                if (descriptor.eventKey == eventKey)
                {
                    activeDays = descriptor.activeDays;
                    rewards = descriptor.rewards.Select(x => x.Clone()).ToArray();
                    eventName = descriptor.eventName;
                    return true;
                }
        }

        return false;
    }

    public bool Validate()
    {
        if (rewards == null || rewards.Length == 0)
        {
            if (!CheckUpdateVersion())
                return false;
        }

        if (claimed == null || claimed.Length != rewards.Length)
            claimed = new bool[rewards.Length];


        return !IsExpired();
    }

    private bool IsExpired()
    {
        return !DuongExtensions.IsInDate(DuongExtensions.GetEpochDayOfYear(), startEpochDate, activeDays);
    }


    public void DeleteFile()
    {
        File.Delete(filePath);
    }
    
  

    public int GetCurrentTargetIndex(out ProgressState currentReady)
    {
        currentReady = ProgressState.NotActive;
        for (int i = 0; i < rewards.Length; i++)
        {
            if (progress < rewards[i].parameter)
            {
                currentReady = ProgressState.InProgress;
                return i;
            }

            if (progress >= rewards[i].parameter && !claimed[i])
            {
                if (!claimed[i])
                    currentReady = ProgressState.Ready;
                return i;
            }
        }

        currentReady = ProgressState.CompleteAll;
        return rewards.Length - 1;
    }

    public void Claim(int i, int intAmount)
    {
        progress += intAmount;
    }
}

