using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public static class BaseActionScriptListExtensions
{
    public static void Prepare<T>(this IEnumerable<T> dict) where T : BaseBaseActionScript
    {
        foreach (var keyValuePair in dict)
            keyValuePair.Prepare();
    }

    public static void StartAction<T>(this IEnumerable<T> dict) where T : BaseBaseActionScript
    {
        foreach (var keyValuePair in dict)
            keyValuePair.StartAction();
    }
    
 
    public static void StartAction<T>(this IEnumerable<T> dict,int index) where T : BaseBaseActionScript
    {
        foreach (var keyValuePair in dict)
            keyValuePair.StartAction(index);
    }
    
    public static void StartAction<T>(this IEnumerable<T> dict,string index) where T : BaseBaseActionScript
    {
        foreach (var keyValuePair in dict)
            keyValuePair.StartAction(index);
    }
    
    public static void StartAction<T>(this IEnumerable<T> dict,bool value) where T : BaseBaseActionScript
    {
        foreach (var keyValuePair in dict)
            keyValuePair.StartAction(value);
    }

    
    public static void StartAction<T>(this IEnumerable<T> dict,Product product) where T : BaseBaseActionScript
    {
        foreach (var keyValuePair in dict)
            keyValuePair.StartAction(product);
    }
}


public enum ActionScriptTriggerType
{
    None,
    ByIndex,
    Toggle,
}

public abstract class BaseBaseActionScriptStartOnEnable : BaseBaseActionScript
{
    [SerializeField] private bool startOnEnable;
    protected virtual void OnEnable()
    {
        if(startOnEnable)
            StartAction();
    }
}

[Serializable] public class ListActionScript
{
    public List<BaseBaseActionScript> scripts;
}

public abstract class BaseBaseActionScript : BaseFrameworkView, IActionScript
{
    [MyButton(nameof(StartAction))] [SerializeField]
    private int testStartAction;
    [MyButtonInt(nameof(StartActionInt))] [SerializeField]
    private int testStartActionIndex;
    [MyButton(nameof(StartActionOn),nameof(StartActionOff))] [SerializeField]
    private int testStartActionToggle;

    public void StartActionInt(int i) => StartAction(i);
    public void StartActionOn() => StartAction(true);
    public void StartActionOff() => StartAction(false);
    
    [SerializeField] private ValidScriptBase validateScript;
    [LabelColor(1,0,0)]
    [SerializeField] protected bool disableAction;
    protected virtual bool IsValidToDoAction() => !disableAction && (validateScript == null || validateScript.IsValid());
    public bool IsBusy { get; set; }

    protected virtual void DoAction()
    {
    }

    public virtual void StartAction()
    {
        #if UNITY_EDITOR
        if (disableAction)
            Debug.LogError($"disable {this.name} on "+gameObject,gameObject);
        #endif
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(ActionScriptTriggerType type,int value)
    {
        switch (type)
        {
            case ActionScriptTriggerType.None:
                StartAction();
                break;
            case ActionScriptTriggerType.ByIndex:
                StartAction(value);
                break;
            case ActionScriptTriggerType.Toggle:
                StartAction(value != 0);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public virtual void StartAction(int index)
    {
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(bool value)
    {
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(string value)
    {
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(int[] arr)
    {
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(IntInt keyPair)
    {
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(int value1,int value2)
    {
        if(IsValidToDoAction())
            DoAction();
    }
    
    public virtual void StartAction(Product product)
    {
        if(IsValidToDoAction())
            DoAction();
    }


    public virtual void StartWithDelay(int index)
    {
        if(IsValidToDoAction())
            DoAction();
    }

    public virtual void Prepare()
    {
        
    }
}