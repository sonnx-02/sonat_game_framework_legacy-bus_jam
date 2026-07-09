using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingBuiltInEnumTypeRemap: IndexBindingScript
{
    [SerializeField] private BuiltInEnumType enumType;
    [IndexByBuiltInEnum(nameof(enumType))]
    public int key; 
    
    [IndexByBuiltInEnum(nameof(enumType))]
    public int[] remaps; 
    [SerializeField] private ToggleScript[] toggleRemaps; 
    [SerializeField] private GameObject[] setActiveRemaps; 
    [SerializeField] private IndexBindingScript[] remapBindings;
    [SerializeField] private int defaultValue = 0;


    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var find = -1;
        for (var i = 0; i < remaps.Length; i++)
            if (remaps[i] == computedValue)
            {
                find = i;
                remapBindings.OnChanged(i);
                break;
            }
        for (var i1 = 0; i1 < toggleRemaps.Length; i1++)
            if(toggleRemaps[i1] != null) toggleRemaps[i1].OnChanged(i1 == find);
        for (var i1 = 0; i1 < setActiveRemaps.Length; i1++)
            if(setActiveRemaps[i1] != null)  setActiveRemaps[i1].SetActive(i1 == find);
        remapBindings.OnChanged(find);
    }
}
