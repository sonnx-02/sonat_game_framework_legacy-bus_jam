using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Obsolete("this is obsolete use IndexPropertySourceRetriever")]
public abstract class BindingSourceRootView : BaseFrameworkView
{
    [SerializeField] protected string propertyName;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
      
    }

    [MyButton(nameof(CheckExist))] [SerializeField]
    private int testCheckExist;
    public void CheckExist()
    {
        var getProperty = typeof(RootView).GetField(propertyName);
        if (getProperty == null)
        {
            Debug.LogError("Error : Kernel doesn't contain  " + propertyName, gameObject);
        }

        if (getProperty.FieldType.IsAssignableFrom(typeof(ReactiveProperty<>)))
        {
            Debug.LogError("Error : " + propertyName + " is not a Property", gameObject);
        }
        
        Debug.Log(propertyName +" exists");
    }

   
    protected object GetPropertyByName()
    {
        var getProperty = typeof(RootView).GetField(propertyName);
        if (getProperty == null)
        {
            Debug.LogError("Error : Kernel doesn't contain  " + propertyName, gameObject);
            return null;
        }

        if (getProperty.FieldType.IsAssignableFrom(typeof(ReactiveProperty<>)))
        {
            Debug.LogError("Error : " + propertyName + " is not a Property", gameObject);
            return null;
        }
        return getProperty.GetValue(RootView.rootView);
    }
}
