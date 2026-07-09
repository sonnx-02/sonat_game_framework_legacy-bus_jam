using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuiltInList
{
    public BuiltInEnumType type;
    [IndexByBuiltInEnum(nameof(type))]
    public int[] keys;
}

public class IndexBindingBuiltInEnumRemap : IndexBindingScript
{
    [SerializeField] private BuiltInList builtInList;
    [SerializeField] private IndexBindingScriptBase[] remaps;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        for (int i = 0; i < builtInList.keys.Length; i++)
        {
            if(builtInList.keys[i] == computedValue)
                remaps.OnChanged(i);
        }
    }
}
