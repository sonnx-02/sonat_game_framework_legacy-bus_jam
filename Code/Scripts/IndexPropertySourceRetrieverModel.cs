using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

[Serializable]
public class IndexPropertySourceRetrieverModelBind
{
    public IndexBindingScriptBase[] bindings;
    public IndexPropertyChangeScriptBase[] changeScripts;
    public ToggleScript[] toggleBindings;
    public TweenDisplayScript[] tweenScripts;
    public ToggleScript[] toggleOnEnableBindings;
    public IndexActive[] actives;

//    [HideInInspector]
    public GameObject[] whenOn;

//    [HideInInspector]
    public GameObject[] whenOff;
}


[Serializable]
public class IndexPropertySourceRetrieverModelBase
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
    private BaseView view;

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
            DataEnum.CustomDataEnumShortcut,
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
            nameof(BuiltInEnumType.CustomDataEnumShortcut),
        })]
    private int customPropertyEnum;


    protected IntReactiveProperty property;

    public void Log() => Debug.Log(Property.Value);

    public IntReactiveProperty Property => property ?? (property = CalProperty());

    public void SetProperty(IntReactiveProperty p)
    {
        property = p;
    }

    private IntReactiveProperty CalProperty()
    {
        switch (dataEnum)
        {
            case DataEnum.CurrentLevel:
                return BaseFrameworkView.PlayerData.currentLevel;
            case DataEnum.Gem:
                return BaseFrameworkView.PlayerData.gem;
            case DataEnum.BestScore:
                return BaseFrameworkView.PlayerData.bestScore;
            case DataEnum.Gold:
                return BaseFrameworkView.PlayerData.gold;
            case DataEnum.Energy:
                return BaseFrameworkView.PlayerData.energy;
            case DataEnum.BUndo:
                return BaseFrameworkView.PlayerData.bUndo;
            case DataEnum.BMore3Lot:
                return BaseFrameworkView.PlayerData.bMore3Slot;
            case DataEnum.BGoStraight:
                return BaseFrameworkView.PlayerData.bGoStraight;
            case DataEnum.GameController:
                return RootView.rootView.gameController.GetRootViewProperty(propertyName);
            case DataEnum.RootView:
                return RootView.rootView.GetProperty(propertyName);
            case DataEnum.PlayerDataByName:
                return BaseFrameworkView.PlayerData.GetPropertyByName(propertyName);
            case DataEnum.DailyDataByName:
                return BaseFrameworkView.DailyData.GetPropertyByName(propertyName);
            case DataEnum.WeeklyDataByName:
                return BaseFrameworkView.WeeklyData.GetType()
                    .GetIntReactiveProperty(propertyName, BaseFrameworkView.WeeklyData);
            case DataEnum.WaveDataByName:
                return BaseFrameworkView.WaveData.GetType()
                    .GetIntReactiveProperty(propertyName, BaseFrameworkView.WaveData);
            case DataEnum.SessionDataByName:
                return BaseFrameworkView.SessionData.GetType()
                    .GetIntReactiveProperty(propertyName, BaseFrameworkView.SessionData);
            case DataEnum.CustomPlayerDataProperty:
                return BaseFrameworkView.PlayerData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomDailyDataProperty:
                return BaseFrameworkView.DailyData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomWeeklyDataProperty:
                return BaseFrameworkView.WeeklyData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomWaveDataProperty:
                return BaseFrameworkView.WaveData.GetCustomProperty(customPropertyEnum);
            case DataEnum.CustomSessionDataProperty:
                return BaseFrameworkView.SessionData.GetCustomProperty(customPropertyEnum);
            case DataEnum.View:
                return GetRootViewProperty();
            case DataEnum.GameSaveProperty:
                return RootView.rootView.gameController.GetGameSaveProperty(customPropertyEnum);
            case DataEnum.GameState:
                return RootView.rootView.gameController.gameState;
            case DataEnum.PlayMode:
                return BaseFrameworkView.SessionData.playMode;
            case DataEnum.CustomDataEnumShortcut:
                return RootView.rootView.gameController.GetCustomDataProperty((CustomDataEnumShortcut)customPropertyEnum);
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    protected virtual IntReactiveProperty GetRootViewProperty()
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

[Serializable]
public class IndexPropertySourceRetrieverModel : IndexPropertySourceRetrieverModelBase
{
    public IndexPropertySourceRetrieverModelBind bind;

    public void SetChangeScriptSource()
    {
        foreach (var sc in bind.changeScripts)
            sc.SetProperty(Property);
    }

    public void Update()
    {
        UpdateData(Property.Value);
    }

    protected virtual void UpdateData(int val)
    {
        bind.toggleOnEnableBindings.OnChanged(Property.BoolValue);
        bind.bindings.OnChanged(val);
        bind.toggleBindings.OnChanged(val != 0);
        foreach (var tweenDisplayScript in bind.tweenScripts)
            tweenDisplayScript.TweenTo(val);
        foreach (var indexActive in bind.actives)
            indexActive.OnChanged(val);
        bind.whenOn.SetActiveAll(val > 0);
        bind.whenOff.SetActiveAll(val == 0);
    }

    public void Check(GameObject source)
    {
#if UNITY_EDITOR
        if (bind.bindings.Any(x => x == null))
            Debug.LogError("null on" + source, source);
#endif
    }

    public void Subscribe()
    {
        Property.Subscribe(UpdateData);
    }

    public bool Validate()
    {
//        if (bind.whenOn.Length > 0 || bind.whenOff.Length > 0)
//            Debug.LogError("not using anymore");

        foreach (var gameObject in bind.actives)
            if (gameObject.target == null)
            {
                Debug.Log("null");
                return false;
            }

        foreach (var gameObject in bind.bindings)
            if (gameObject == null)
            {
                Debug.Log("null");
                return false;
            }

        foreach (var gameObject in bind.toggleBindings)
            if (gameObject == null)
            {
                Debug.Log("null");
                return false;
            }

        foreach (var gameObject in bind.tweenScripts)
            if (gameObject == null)
            {
                Debug.Log("null");
                return false;
            }

        return true;
    }
}