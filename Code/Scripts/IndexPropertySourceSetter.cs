using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[Serializable]
public class IndexPropertySourceSetter
{
    private IntReactiveProperty _property;

    [SerializeField] protected DataEnum dataEnum;

    [SerializeField]
    [ConditionalField(nameof(dataEnum),
        new object[]
        {
            DataEnum.GameController,
            DataEnum.RootView,
            DataEnum.View,
            DataEnum.PlayerDataByName,
            DataEnum.DailyDataByName,
            DataEnum.WeeklyDataByName,
            DataEnum.SessionDataByName,
            DataEnum.WaveDataByName,
        })]
    private string propertyName;

    [SerializeField]
    [ConditionalField(nameof(dataEnum),
        new object[]
        {
            DataEnum.View,
        })]
    private MonoBehaviour view;

    [SerializeField]
    [IndexAsEnumCondition(nameof(dataEnum),
        new object[]
        {
            DataEnum.CustomPlayerDataProperty,
            DataEnum.CustomDailyDataProperty,
            DataEnum.CustomWeeklyDataProperty,
            DataEnum.CustomSessionDataProperty,
            DataEnum.CustomWaveDataProperty,
            DataEnum.GameSaveProperty,
        },
        new[]
        {
            nameof(BuiltInEnumType.CustomPlayerDataProperty),
            nameof(BuiltInEnumType.CustomDailyDataProperty),
            nameof(BuiltInEnumType.CustomWeeklyDataProperty),
            nameof(BuiltInEnumType.CustomSessionDataProperty),
            nameof(BuiltInEnumType.CustomWaveDataProperty),
            nameof(BuiltInEnumType.GameSaveKey),
        })]
    private int customPropertyEnum;

    [SerializeField] int setValue;

    private IntReactiveProperty Property => _property ??= CalProperty();
    private ReactivePlayerData PlayerData => BaseFrameworkView.PlayerData;
    private ReactiveWaveData WaveData => BaseFrameworkView.WaveData;
    private ReactiveSessionData SessionData => BaseFrameworkView.SessionData;
    private ReactivePlayerDailyData DailyData => BaseFrameworkView.DailyData;
    private ReactivePlayerWeeklyData WeeklyData => BaseFrameworkView.WeeklyData;
    private IntReactiveProperty CalProperty()
    {
        switch (dataEnum)
        {
            case DataEnum.CurrentLevel:
                return PlayerData.currentLevel;
            case DataEnum.Gem:
                return PlayerData.gem;
            case DataEnum.BestScore:
                return PlayerData.bestScore;
            case DataEnum.Gold:
                return PlayerData.gold;
            case DataEnum.Energy:
                return BaseFrameworkView.PlayerData.energy;
            case DataEnum.BUndo:
                return PlayerData.bUndo;
            case DataEnum.BMore3Lot:
                return PlayerData.bMore3Slot;
            case DataEnum.BGoStraight:
                return PlayerData.bGoStraight;
            case DataEnum.GameController:
                return RootView.rootView.gameController.GetRootViewProperty(propertyName);
            case DataEnum.RootView:
                return RootView.rootView.GetProperty(propertyName);
            case DataEnum.PlayerDataByName:
                return PlayerData.GetPropertyByName(propertyName);
            case DataEnum.DailyDataByName:
                return DailyData.GetPropertyByName(propertyName);
            case DataEnum.WeeklyDataByName:
                return WeeklyData.GetType().GetIntReactiveProperty(propertyName, WeeklyData);
            case DataEnum.WaveDataByName:
                return WaveData.GetType().GetIntReactiveProperty(propertyName, WaveData);
            case DataEnum.SessionDataByName:
                return SessionData.GetType().GetIntReactiveProperty(propertyName, SessionData);
            case DataEnum.CustomPlayerDataProperty:
                return PlayerData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomDailyDataProperty:
                return DailyData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomWeeklyDataProperty:
                return WeeklyData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomWaveDataProperty:
                return WaveData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomSessionDataProperty:
                return SessionData.GetCustomProperty(customPropertyEnum);
            case DataEnum.View:
                return GetRootViewProperty();
            case DataEnum.GameSaveProperty:
                return RootView.rootView.gameController.GetGameSaveProperty(customPropertyEnum);
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    public void SetValue()
    {
        Property.Value = setValue;
    }

    
    public void SetValue(int value)
    {
        Property.Value = value;
    }

    private IntReactiveProperty GetRootViewProperty()
    {
        var getField = view.GetType().GetField(propertyName);
        if (getField != null)
            return (IntReactiveProperty) getField.GetValue(view);

        var getProperty = view.GetType().GetProperty(propertyName);
        if (getProperty != null)
            return (IntReactiveProperty) getProperty.GetValue(view);

        return null;
    }
}