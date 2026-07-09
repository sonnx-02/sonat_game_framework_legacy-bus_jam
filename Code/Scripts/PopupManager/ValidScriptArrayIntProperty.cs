using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ValidScriptArrayIntProperty : ValidScriptBase
{
    [SerializeField] private MonoBehaviour view;
    [SerializeField]  private string propertyName;
    [SerializeField] private int index;
    
    protected IntReactiveProperty property;

    public void Log() => Debug.Log(GetRootViewProperty()[0]);

    
    protected override bool IsThisValid()
    {
        return GetRootViewProperty()[index] > 0;
    }

    protected virtual int[] GetRootViewProperty()
    {
        var getField = view.GetType().GetField(propertyName);
        if (getField != null)
            return (int[]) getField.GetValue(view);

        var getProperty = view.GetType().GetProperty(propertyName);
        if (getProperty != null)
            return (int[]) getProperty.GetValue(view);
        return null;
    }
}
