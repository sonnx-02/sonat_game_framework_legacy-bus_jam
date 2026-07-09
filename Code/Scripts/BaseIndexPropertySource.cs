using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public abstract class BaseIndexPropertySource : BaseFrameworkView
{
    [LabelColor(1,0,0)]
    [SerializeField] protected bool disable;
    public IntReactiveProperty GetProperty() => Property;
    protected IntReactiveProperty property;
    protected abstract IntReactiveProperty Property { get; }

    public void SetProperty(IntReactiveProperty p)
    {
        property = p;
    }
}

public abstract class BaseIndexPropertySourceRetriever : BaseIndexPropertySource
{
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
            DataEnum.PlayMode,
        },
        new[]
        {
            nameof(BuiltInEnumType.CustomPlayerDataProperty),
            nameof(BuiltInEnumType.CustomDailyDataProperty),
            nameof(BuiltInEnumType.CustomWeeklyDataProperty),
            nameof(BuiltInEnumType.CustomSessionDataProperty),
            nameof(BuiltInEnumType.CustomWaveDataProperty),
            nameof(BuiltInEnumType.GameSaveKey),
            nameof(BuiltInEnumType.PlayMode),
        })]
    private int customPropertyEnum;

    protected override IntReactiveProperty Property => property ??= CalProperty();

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
            case DataEnum.BUndo:
                return PlayerData.bUndo;
            case DataEnum.BMore3Lot:
                return PlayerData.bMore3Slot;
            case DataEnum.BGoStraight:
                return PlayerData.bGoStraight;
            case DataEnum.GameController:
                return BaseGameController.GetRootViewProperty(propertyName);
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
                return GetPropertyFromView(view);
            case DataEnum.GameSaveProperty:
                return BaseGameController.GetGameSaveProperty(customPropertyEnum);
            case DataEnum.GameState:
                return BaseGameController.gameState;
            case DataEnum.PlayMode:
                return SessionData.playMode;
            case DataEnum.Energy:
                return PlayerData.energy;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    protected virtual IntReactiveProperty GetPropertyFromView(MonoBehaviour v)
    {
        var getField = v.GetType().GetField(propertyName);
        if (getField != null)
            return (IntReactiveProperty) getField.GetValue(v);

        var getProperty = v.GetType().GetProperty(propertyName);
        if (getProperty != null)
            return (IntReactiveProperty) getProperty.GetValue(v);
        return null;
    }


    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(GetProperty());
    }
}