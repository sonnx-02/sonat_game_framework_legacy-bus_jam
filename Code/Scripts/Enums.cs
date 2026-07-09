using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PlaySound
{
    public static void Play(this BaseSoundEnum sound)
    {
        RootView.rootView.audioManager.Play((int)sound);
    }
}


public enum ValidEnum
{
    NotValid,
    Valid,
    Skip
}

public enum SourceDataEnum
{
    PlayerData = 0,
    DailyData = 1,
    WeeklyData = 2,
    SessionData = 3,
    WaveData = 4,
    GameController = 5,
    RootView = 6,
}

public enum DataEnum
{
    CurrentLevel = -1,
    Gold = 0,
    Gem = 1,
    BestScore = 2,
    GameController = 3,
    RootView = 4,
    View = 5,
    GameState = 6,
    PlayMode = 7,
    Energy = 8,
    BUndo = 9,
    BMore3Lot = 10,
    BGoStraight = 11,

    PlayerDataByName = 20,
    DailyDataByName = 21,
    WeeklyDataByName = 22,
    SessionDataByName = 23,
    WaveDataByName = 24,

    GameSaveProperty = 29,
    CustomPlayerDataProperty = 30,
    CustomDailyDataProperty = 31,
    CustomWeeklyDataProperty = 32,
    CustomSessionDataProperty = 33,
    CustomWaveDataProperty = 34,
    
    CustomDataEnumShortcut = 100
}

public enum ArrayIntDataEnum
{
    GameController = 0,
    RootView = 1,
    View = 2,
 
    GameSaveArrayIntProperty = 3,
    CustomPlayerArrayIntProperty = 4,
    CustomDailyArrayIntProperty = 5,
    CustomWeeklyArrayIntProperty = 6,
    CustomSessionArrayIntProperty = 7,
    CustomWaveArrayIntProperty = 8,
    
//    PlayerArrayIntByName = 9,
//    DailyArrayIntByName = 10,
//    WeeklyArrayIntByName = 11,
//    SessionArrayIntByName = 12,
//    WaveArrayIntByName = 13,
}


public enum LocationEnum
{
    Gold,
    Gem,
    Star,
}


public enum BaseSoundEnum
{
    None = 0,
    ButtonClick = 1,
    PopupShow = 2,
    PopupHide = 3,
    GetReward = 4,
}