using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Obsolete("this is obsolete use IndexPropertySourceRetriever")]
public class ToggleBindingSourceRootView : BindingSourceRootView
{
    [SerializeField] private ToggleScript[] scripts;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
            {
                Debug.LogError("err :scripts null",gameObject);
                break;
            }
        
        if(GetProperty() != null)
            GetProperty().Subscribe(scripts.OnChanged).DisposeWith(this);
        else
        {
            GetPropertyInt().Subscribe(data => scripts.OnChanged(data == 1));
        }
    }

    private BoolReactiveProperty GetProperty()
    {
        if (!(GetPropertyByName() is BoolReactiveProperty))
            return null;
        var p = (BoolReactiveProperty) GetPropertyByName();
//        if(p == null)
//            Debug.LogError("not found"+propertyName,gameObject);

        return p;
    }
    
    private IntReactiveProperty GetPropertyInt()
    {
        if (!(GetPropertyByName() is IntReactiveProperty))
            return null;
        
        var p = (IntReactiveProperty)GetPropertyByName();
//        if(p == null)
//            Debug.LogError("not found"+propertyName,gameObject);

        return p;
    }
    
    [MyButton(nameof(AddSetActive))] [SerializeField]
    private int abc123b;
    public void AddSetActive()
    {
        var add =  gameObject.AddComponent<ToggleScriptSetActive>();
        scripts = new ToggleScript[] {add};
    }

}
