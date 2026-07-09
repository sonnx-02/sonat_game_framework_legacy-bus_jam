using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniRx;

[System.Serializable]
public class IntRetrieverItem
{
    [SerializeField] private DataEnumByName dataEnum;

    [SerializeField]
    [ConditionalField(nameof(dataEnum),
        new object[]
        {
            DataEnumByName.View,
        })]
    private BaseView view;

    [SerializeField] private string propertyName;
    
    [IndexAsEnumMultiple(new object[]
        {
            DataEnumByName.FrameworkSharedData,
            DataEnumByName.GameSettingSharedData,
        },
        new[]
        {
            nameof(BuiltInEnumType.FrameworkSharedData),
            nameof(BuiltInEnumType.GameSettingSharedData),
        },
        nameof(dataEnum))]
    [SerializeField] private IntInt keyAndAdjust;
    [HideInInspector] [SerializeField] private int adjust;

    

    public int GetValue()
    {
        var adj = keyAndAdjust.value2;
        if (adjust != 0)
        {
            adj = adjust;
#if UNITY_EDITOR
            Debug.LogError("adjust no more use");
#endif
        }

        switch (dataEnum)
        {
            case DataEnumByName.GameController:
                return RootView.rootView.gameController.GetIntValue(propertyName) + adj;
            case DataEnumByName.RootView:
                return RootView.rootView.GetIntValue(propertyName) + adj;
            case DataEnumByName.View:
                return view.GetIntValue(propertyName) + adj;
            case DataEnumByName.CurrentGameSetting:
                return RootView.rootView.gameController.GetIntCurrentGameSetting(propertyName) + adj;
            case DataEnumByName.GameSetting:
                return Kernel.GetDatabase<GameSetting>().GetIntValue(propertyName) + adj;
            case DataEnumByName.CurrentLevel:
                return BaseFrameworkView.PlayerData.CurrentLevel + adj;
            case DataEnumByName.GameSettingSharedData:
                return Kernel.GetDatabase<GameSetting>().sharedSetting[keyAndAdjust.value] + adj;
            case DataEnumByName.FrameworkSharedData:
                return BaseFrameworkView.SharedData[keyAndAdjust.value] + adj;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public int[] GetValues()
    {
        switch (dataEnum)
        {
            case DataEnumByName.GameController:
                return RootView.rootView.gameController.GetIntValues(propertyName);
            case DataEnumByName.RootView:
                return RootView.rootView.GetIntValues(propertyName);
            case DataEnumByName.View:
                return view.GetIntValues(propertyName);
            case DataEnumByName.CurrentGameSetting:
                return RootView.rootView.gameController.GetIntArrayCurrentGameSetting(propertyName);
            case DataEnumByName.GameSetting:
                return Kernel.GetDatabase<GameSetting>().GetIntValues(propertyName);
            case DataEnumByName.CurrentLevel:
                return new[] {BaseFrameworkView.PlayerData.CurrentLevel};
            case DataEnumByName.GameSettingSharedData:
                return Kernel.GetDatabase<GameSetting>().sharedSetting;
            case DataEnumByName.FrameworkSharedData:
                return BaseFrameworkView.SharedData;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum DataEnumByName
{
    View = 0,
    GameController = 1,
    RootView = 2,
    CurrentGameSetting = 3,
    GameSetting = 4,
    CurrentLevel = 5,
    GameSettingSharedData = 6,
    FrameworkSharedData = 7,
}


/// <summary>
/// to many integer properties from one source
/// </summary>
public class IntValueRetriever : IntValueRetrieverBase
{
    public override int this[int index] => items[index].GetValue();

    [SerializeField] private IntRetrieverItem[] items;

    public override int[] Values
    {
        get
        {
#if UNITY_EDITOR
            if (test)
                return testData;
#endif
            return items.Select(x => x.GetValue()).ToArray();
        }
    }

    public override int[] GetArrayInt(int index)
    {
        return items[index].GetValues();
    }

    public override int Length => items.Length;

#if UNITY_EDITOR
    [SerializeField] private bool test;
    [SerializeField] private int[] testData;
#endif

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        Debug.Log(this[value]);
    }
}