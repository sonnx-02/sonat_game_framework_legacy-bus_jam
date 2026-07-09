using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public abstract class BaseFloatPropertySource : BaseFrameworkView
{
    public FloatReactiveProperty GetProperty() => Property;
    protected FloatReactiveProperty property;
    protected abstract FloatReactiveProperty Property { get; }

    public void SetProperty(FloatReactiveProperty p)
    {
        property = p;
    }
}

public abstract class BaseFloatPropertySourceRetriever : BaseFloatPropertySource
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

    protected override FloatReactiveProperty Property => property ??= CalProperty();

    private FloatReactiveProperty CalProperty()
    {
        switch (dataEnum)
        {
//            case DataEnum.CurrentLevel:
//                return PlayerData.currentLevel;
//            case DataEnum.Gem:
//                return PlayerData.gem;
//            case DataEnum.BestScore:
//                return PlayerData.bestScore;
//            case DataEnum.Gold:
//                return PlayerData.gold;
            case DataEnum.GameController:
                return BaseGameController.GetRootViewPropertyFloat(propertyName);
            case DataEnum.RootView:
                return RootView.rootView.GetPropertyFloat(propertyName);
            case DataEnum.PlayerDataByName:
                return PlayerData.GetFloatPropertyByName(propertyName);
//            case DataEnum.DailyDataByName:
//                return DailyData.GetPropertyByName(propertyName);
//            case DataEnum.WeeklyDataByName:
//                return WeeklyData.GetType().GetFloatReactiveProperty(propertyName, WeeklyData);
//            case DataEnum.WaveDataByName:
//                return WaveData.GetType().GetFloatReactiveProperty(propertyName, WaveData);
//            case DataEnum.SessionDataByName:
//                return SessionData.GetType().GetFloatReactiveProperty(propertyName, SessionData);
//            case DataEnum.CustomPlayerDataProperty:
//                return PlayerData.GetCustomProperty(customPropertyEnum);
//            case DataEnum.CustomDailyDataProperty:
//                return DailyData.GetCustomProperty(customPropertyEnum);
//            case DataEnum.CustomWeeklyDataProperty:
//                return WeeklyData.GetCustomProperty(customPropertyEnum);
//            case DataEnum.CustomWaveDataProperty:
//                return WaveData.GetCustomProperty(customPropertyEnum);
//            case DataEnum.CustomSessionDataProperty:
//                return SessionData.GetCustomProperty(customPropertyEnum);
            case DataEnum.View:
                return GetRootViewProperty();
//            case DataEnum.GameSaveProperty:
//                return RootView.rootView.gameController.GetGameSaveProperty(customPropertyEnum);
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    protected virtual FloatReactiveProperty GetRootViewProperty()
    {
        var getField = view.GetType().GetField(propertyName);
        if (getField != null)
            return (FloatReactiveProperty) getField.GetValue(view);

        var getProperty = view.GetType().GetProperty(propertyName);
        if (getProperty != null)
            return (FloatReactiveProperty) getProperty.GetValue(view);
        return null;
    }


    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(GetProperty());
    }
}