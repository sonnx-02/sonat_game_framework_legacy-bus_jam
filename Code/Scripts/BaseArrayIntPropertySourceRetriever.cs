using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[Serializable]
public class ArrayIntPropertySourceRetrieverModelBase
{
    [SerializeField] protected ArrayIntDataEnum dataEnum;

    [SerializeField]
    [ConditionalField(nameof(dataEnum),
        new object[]
        {
            ArrayIntDataEnum.GameController,
            ArrayIntDataEnum.RootView,
            ArrayIntDataEnum.View,
//            ArrayIntDataEnum.PlayerArrayIntByName,
//            ArrayIntDataEnum.DailyArrayIntByName,
//            ArrayIntDataEnum.WeeklyArrayIntByName,
//            ArrayIntDataEnum.WaveArrayIntByName,
//            ArrayIntDataEnum.SessionArrayIntByName,
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
            ArrayIntDataEnum.CustomPlayerArrayIntProperty,
            ArrayIntDataEnum.CustomDailyArrayIntProperty,
            ArrayIntDataEnum.CustomWeeklyArrayIntProperty,
            ArrayIntDataEnum.CustomSessionArrayIntProperty,
            ArrayIntDataEnum.CustomWaveArrayIntProperty,
            ArrayIntDataEnum.GameSaveArrayIntProperty,
        },
        new[]
        {
            nameof(BuiltInEnumType.CustomPlayerArrayIntProperty),
            nameof(BuiltInEnumType.CustomDailyArrayIntProperty),
            nameof(BuiltInEnumType.CustomWeeklyArrayIntProperty),
            nameof(BuiltInEnumType.CustomSessionArrayIntProperty),
            nameof(BuiltInEnumType.CustomWaveArrayIntProperty),
            nameof(BuiltInEnumType.GameSaveArrayIntKey),
        })]
    private int customPropertyEnum;


    protected ArrayIntReactiveProperty property;

    public void Log() => Debug.Log(Property.Value);

    public ArrayIntReactiveProperty Property => property ?? (property = CalProperty());

    public void SetProperty(ArrayIntReactiveProperty p)
    {
        property = p;
    }

    private ArrayIntReactiveProperty CalProperty()
    {
        switch (dataEnum)
        {
            case ArrayIntDataEnum.GameController:
                return RootView.rootView.gameController.GetRootViewProperty<ArrayIntReactiveProperty>(propertyName);
            case ArrayIntDataEnum.RootView:
                return RootView.rootView.GetProperty<ArrayIntReactiveProperty>(propertyName);
            case ArrayIntDataEnum.View:
                return GetPropertyFromView<ArrayIntReactiveProperty>(view);
            case ArrayIntDataEnum.GameSaveArrayIntProperty:
                return null;
//                return RootView.rootView.gameController.GetGameSaveProperty(customPropertyEnum);
            case ArrayIntDataEnum.CustomPlayerArrayIntProperty:
                return BaseFrameworkView.PlayerData.GetCustomArrayIntProperty(customPropertyEnum);
            case ArrayIntDataEnum.CustomDailyArrayIntProperty:
                return BaseFrameworkView.DailyData.GetCustomArrayIntProperty(customPropertyEnum);
            case ArrayIntDataEnum.CustomWeeklyArrayIntProperty:
                return BaseFrameworkView.WeeklyData.GetCustomArrayIntProperty(customPropertyEnum);
            case ArrayIntDataEnum.CustomSessionArrayIntProperty:
                return BaseFrameworkView.SessionData.GetCustomArrayIntProperty(customPropertyEnum);
            case ArrayIntDataEnum.CustomWaveArrayIntProperty:
                return BaseFrameworkView.WaveData.GetCustomArrayIntProperty(customPropertyEnum);
//            case ArrayIntDataEnum.PlayerArrayIntByName:
//                break;
//            case ArrayIntDataEnum.DailyArrayIntByName:
//                break;
//            case ArrayIntDataEnum.WeeklyArrayIntByName:
//                break;
//            case ArrayIntDataEnum.SessionArrayIntByName:
//                break;
//            case ArrayIntDataEnum.WaveArrayIntByName:
//                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual T GetPropertyFromView<T>(MonoBehaviour v)
    {
        var getField = v.GetType().GetField(propertyName);
        if (getField != null)
            return (T) getField.GetValue(v);

        var getProperty = v.GetType().GetProperty(propertyName);
        return (T) getProperty.GetValue(v);
    }
}

[Serializable]
public abstract class BaseArrayIntPropertySourceRetriever : BaseFrameworkView
{
//    [SerializeField] private IndexPropertySourceRetrieverModelBase model;
    public ArrayIntReactiveProperty GetProperty() => Property;
    protected ArrayIntReactiveProperty property;
    protected abstract ArrayIntReactiveProperty Property { get; }

    public void SetProperty(ArrayIntReactiveProperty p)
    {
        property = p;
    }
}
