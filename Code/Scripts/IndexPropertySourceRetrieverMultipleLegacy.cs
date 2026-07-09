using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UniRx;




[Obsolete("It is for old version")]
public class IndexPropertySourceRetrieverMultipleLegacy : BaseFrameworkView
{
    [SerializeField] private DataEnumByName dataEnum;

    [SerializeField]
    [ConditionalField(nameof(dataEnum),
        new object[]
        {
            DataEnumByName.View,
        })]
    private MonoBehaviour view;

    [SerializeField] private IndexSourceBindingItem[] bindings;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();

        foreach (var item in bindings)
        {
            var property = CalProperty(item.propertyName);
            if (property == null)
                Debug.Log("not found " + item.propertyName, gameObject);

            property.Subscribe(data =>
            {
                foreach (var indexBindingScript in item.bindings)
                    indexBindingScript.OnChanged(data);

                foreach (var indexBindingScript in item.toggleBindings)
                    indexBindingScript.OnChanged(data);
            });

            foreach (var sc in item.changeScripts)
                sc.SetProperty(property);
        }
    }
    
    private IntReactiveProperty CalProperty(string propertyName)
    {
        switch (dataEnum)
        {
            case DataEnumByName.GameController:
                return RootView.rootView.gameController.GetRootViewProperty(propertyName);
            case DataEnumByName.RootView:
                return RootView.rootView.GetProperty(propertyName);
            case DataEnumByName.PlayerDataByName:
                return PlayerData.GetPropertyByName(propertyName);
            case DataEnumByName.DailyDataByName:
                return DailyData.GetPropertyByName(propertyName);
            case DataEnumByName.WeeklyDataByName:
                return WeeklyData.GetType().GetIntReactiveProperty(propertyName, WeeklyData);
            case DataEnumByName.WaveDataByName:
                return WaveData.GetType().GetIntReactiveProperty(propertyName, WaveData);
            case DataEnumByName.SessionDataByName:
                return SessionData.GetType().GetIntReactiveProperty(propertyName, SessionData);
            case DataEnumByName.View:
                return GetRootViewProperty(propertyName);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual IntReactiveProperty GetRootViewProperty(string propertyName)
    {
        var getField = view.GetType().GetField(propertyName);
        if (getField != null)
            return (IntReactiveProperty) getField.GetValue(view);

        var getProperty = view.GetType().GetProperty(propertyName);
        if (getProperty != null)
            return (IntReactiveProperty) getProperty.GetValue(view);

        return null;
    }

    private enum DataEnumByName
    {
        GameController = 0,
        RootView = 1,
        View = 2,
        PlayerDataByName = 3,
        DailyDataByName = 4,
        WeeklyDataByName = 5,
        SessionDataByName = 6,
        WaveDataByName = 7,
    }
    
    [System.Serializable]
    public class IndexSourceBindingItem
    {
        public string propertyName;
        public IndexBindingScriptBase[] bindings;
        public IndexPropertyChangeScriptBase[] changeScripts;
        public ToggleScript[] toggleBindings;
    }
}
