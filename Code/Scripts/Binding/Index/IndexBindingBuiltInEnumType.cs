using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingBuiltInEnumType : IndexBindingScript
{
    [SerializeField] private BuiltInEnumType enumType;
    [IndexByBuiltInEnum(nameof(enumType))]
    public int key; 
    
    [DisplayAsDisable]
    [IndexByBuiltInEnum(nameof(enumType))]
    public int current; 
    [IndexByBuiltInEnum(nameof(enumType))]
    public int[] othersKey; 
    
    [SerializeField] private ToggleScript[] onValid;
    [SerializeField] private bool setActiveThis = true;
    [SerializeField] private GameObject[] actives;
    [SerializeField] private GameObject[] activesReverse;
    [SerializeField] private bool reverse;

    [SerializeField] private bool log;

    public override void OnChanged(int index)
    {
        base.OnChanged(index);
        current = index;
    }

    protected override void OnValueChanged(int computedValue)
    {
        
        base.OnValueChanged(computedValue);
        bool result = GetValid(computedValue);
        if (reverse)
            result = !result;
        if (setActiveThis)
            gameObject.SetActive(result);
        onValid.OnChanged(result);
        actives.SetActiveAll(result);
        activesReverse.SetActiveAll(!result);
    }

    private bool GetValid(int index)
    {
        foreach (var other in othersKey)
        {
            if (other == index)
                return true;
        }

        return key == index;
    }

    public override void LogCustomInt(int a)
    {
        base.LogCustomInt(key);
        if (a > 0)
        {
            OnChanged(key);
        }
        else
        {
            OnChanged(key-1);
        }
    }
}
