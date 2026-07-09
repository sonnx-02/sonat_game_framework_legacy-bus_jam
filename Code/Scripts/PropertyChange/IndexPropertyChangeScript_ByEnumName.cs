using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexPropertyChangeScript_ByEnumName : IndexPropertyChangeScriptBase
{
#if UNITY_EDITOR
    [LabelHeader("must re-select to load", 0, 1, 1)] [SerializeField]
    private int testHeader;
#endif

    
    [IndexByBuiltInEnum(nameof(enumTypeName),EnumFindBy.Name)] // for display
    [SerializeField]
    private int value = 0;

    public string enumTypeName;
    
    protected override int Value
    {
        get => value;
        set => this.value = value;
    }

    protected override string GetName()
    {
        return "index_property_change_" + new EnumList(enumTypeName).GetRawName(value).ToLower();
    }

    
}