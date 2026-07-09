using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


public class IndexPropertySourceRetrieverMultiple : BaseFrameworkView
{
    [ArrayElementTitle("dataEnum")]
    public IndexPropertySourceRetrieverModel[] models;

    [SerializeField] private PropertyBindTime bind = PropertyBindTime.AlwaysSubscribe;
    [SerializeField] private GameMainEvent[] gameMainEvents;
    [SerializeField] private int[] bindInUniversalEvents;


    [MyButtonInt(nameof(Log),nameof(ListenChanged))] [SerializeField]
    private int testLog;

    private void Log(int value)
    {
        Debug.Log(models[value].Property.Value);
    }
    private void ListenChanged(int value)
    {
        models[value].Property.Subscribe(data => Debug.LogError(data));
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        models[0].Property.Value = value;
    }

    protected override void Register()
    {
        base.Register();
        if ((bind & PropertyBindTime.AlwaysSubscribe) != 0)
            for (var index = 0; index < models.Length; index++)
            {
                var model = models[index];
#if UNITY_EDITOR
                if (!model.Validate())
                    Debug.LogError("model " + index + " null on" + gameObject, gameObject);
#endif
                model.Subscribe();
            }
        
        foreach (var indexPropertySourceRetrieverModel in models)
            indexPropertySourceRetrieverModel.Check(gameObject);
    }

    private void OnEnable()
    {
        if ((bind & PropertyBindTime.OnEnable) != 0 || (bind & PropertyBindTime.AlwaysSubscribe) != 0)
            foreach (var model in models)
                model.Update();
    }


//    [SerializeField] private bool logDebug;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();

        foreach (var model in models)
            model.SetChangeScriptSource();
        
        if ((bind & PropertyBindTime.AlwaysSubscribe) != 0)
            SubjectController.UniversalEvent.Subscribe(CheckUniversalEvent);

        if ((bind & PropertyBindTime.OnGameMainEvents) != 0)
            SubjectController.OnGameMainEvent.Subscribe(CheckGameMainEvent);
    }

    private void CheckGameMainEvent(GameMainEvent eventKey)
    {
        foreach (var bindInUniversalEvent in gameMainEvents)
            if (bindInUniversalEvent == eventKey)
            {
                foreach (var model in models)
                    model.Update();
                return;
            }
    }

    private void CheckUniversalEvent(int eventKey)
    {
        foreach (var bindInUniversalEvent in bindInUniversalEvents)
            if (bindInUniversalEvent == eventKey)
            {
                foreach (var model in models)
                    model.Update();
                return;
            }
    }

//    protected override string GetName()
//    {
//        if (dataEnum == DataEnum.CustomProperty)
//            return base.GetName() + "_" +
//                   ProjectSettings.GetKey(BuiltInEnumType.CustomPlayerDataProperty.ToString(), customPropertyEnum);
//        return base.GetName();
//    }
}