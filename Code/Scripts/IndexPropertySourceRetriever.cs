using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

[Flags]
public enum PropertyBindTime
{
    None = 0,
    AlwaysSubscribe = 1,
    OnEnable  = 2,
    OnUniversalEvent = 4,
    OnGameMainEvents = 8,
}

//[Obsolete("Index...Multiple is more convenient")]
public class IndexPropertySourceRetriever : BaseIndexPropertySourceRetriever
{
    [SerializeField] private IndexPropertySourceRetrieverModel convert;

    public override void LogCustom()
    {
        base.LogCustom();
        convert.bind.bindings = bindings;
        convert.bind.changeScripts = changeScripts;
        convert.bind.toggleBindings = toggleBindings;
        convert.bind.tweenScripts = tweenScripts;
        convert.bind.whenOff = whenOff;
        convert.bind.whenOn = whenOn;
        convert.bind.toggleOnEnableBindings = toggleOnEnableBindings;
//        convert.Set(bind,bindInUniversalEvents,gameMainEvents);
    }


    [SerializeField] private PropertyBindTime bind = PropertyBindTime.AlwaysSubscribe;
    [SerializeField] private int[] bindInUniversalEvents;
    [SerializeField] private GameMainEvent[] gameMainEvents;

    [SerializeField] private IndexBindingScriptBase[] bindings;
    [SerializeField] private IndexPropertyChangeScriptBase[] changeScripts;
    [SerializeField] private ToggleScript[] toggleBindings;
    [SerializeField] private TweenDisplayScript[] tweenScripts;
    [SerializeField] private ToggleScript[] toggleOnEnableBindings;
    [SerializeField] private GameObject[] whenOn;
    [SerializeField] private GameObject[] whenOff;

    [MyButtonInt(nameof(TestChange),nameof(Log))] [SerializeField]
    private int testChanged;

    #if UNITY_EDITOR
    [LabelHeader("To add UniversalEvent as int",0,1,1)] [SerializeField]  private int testLabel16;
    [MyButton(nameof(AddUniversal))] [SerializeField]
    private int forAddUniversalEvent;
    [SerializeField] private BuiltInEnumType enumTypeOfUniversalEvent; // for add universal event
    [SerializeField] [IndexAsEnumFromProperty(nameof(enumTypeOfUniversalEvent))]
    private int indexOfUniversalEvent; // for universal event
    protected void AddUniversal()
    {
        bindInUniversalEvents = bindInUniversalEvents.ArrayAdd(enumTypeOfUniversalEvent.ToUniversalKey(indexOfUniversalEvent)).ToArray();
    }
    #endif

    
    public void TestChange(int value)
    {
        Property.Value = value;
    }
    
    public void Log(int value)
    {
        Debug.Log(Property.Value);
    }

    #if UNITY_EDITOR
    [SerializeField] private bool log;
    #endif
    protected override void Register()
    {
        base.Register();
        
        #if UNITY_EDITOR
            if(Property == null)
                Debug.LogError("null on "+gameObject,gameObject);
            if (log)
                Property.Subscribe(data => Debug.Log(data));
        #endif
        
        if( !disable && (bind & PropertyBindTime.AlwaysSubscribe) != 0)
            Property.Subscribe(UpdateData);
    }

    private void OnEnable()
    {
        if ( !disable &&  ((bind & PropertyBindTime.OnEnable) != 0 || (bind & PropertyBindTime.AlwaysSubscribe) != 0))
            UpdateData(Property.Value);
    }

    protected virtual void UpdateData(int val)
    {
        if(disable)
            return;
        
        toggleOnEnableBindings.OnChanged(Property.BoolValue);
        foreach (var indexBindingScript in bindings)
        {
            indexBindingScript.OnChanged(val);
         
        }
        foreach (var indexBindingScript in toggleBindings)
            indexBindingScript.OnChanged(val);
        foreach (var tweenDisplayScript in tweenScripts)
            tweenDisplayScript.TweenTo(val);
        
        whenOn.SetActiveAll(val > 0);
        whenOff.SetActiveAll(val == 0);
    }

//    [SerializeField] private bool logDebug;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
#if UNITY_EDITOR
        if (bindings.Any(x => x == null))
            Debug.LogError("null on" + gameObject, gameObject);
        if (toggleBindings.Any(x => x == null))
            Debug.LogError("null on" + gameObject, gameObject);
        if (tweenScripts.Any(x => x == null))
            Debug.LogError("null on" + gameObject, gameObject);
#endif

        if(disable)
            return;
        foreach (var sc in changeScripts)
            sc.SetProperty(Property);

        if ((bind & PropertyBindTime.AlwaysSubscribe) != 0)
        {
            SubjectController.UniversalEvent.Subscribe(CheckUniversalEvent);
        }
        
        if ((bind & PropertyBindTime.OnGameMainEvents) != 0)
        {
            SubjectController.OnGameMainEvent.Subscribe(CheckGameMainEvent);
        }
    }

    private void CheckGameMainEvent(GameMainEvent eventKey)
    {
        if(disable)
            return;
        foreach (var bindInUniversalEvent in gameMainEvents)
            if (bindInUniversalEvent == eventKey)
            {
                UpdateData(Property.Value);
                return;
            }
    }

    private void CheckUniversalEvent(int eventKey)
    {
        if(disable)
            return;
        foreach (var bindInUniversalEvent in bindInUniversalEvents)
            if (bindInUniversalEvent == eventKey)
            {
                UpdateData(Property.Value);
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