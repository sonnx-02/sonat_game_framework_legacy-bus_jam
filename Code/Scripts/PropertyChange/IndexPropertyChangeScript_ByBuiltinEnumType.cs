using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexPropertyChangeScript_ByBuiltinEnumType : IndexPropertyChangeScriptBase
{
#if UNITY_EDITOR
    [LabelHeader("must re-select to load", 0, 1, 1)] [SerializeField]
    private int testHeader;
#endif
    [IndexByBuiltInEnum(nameof(builtInEnumType))] // for display
    [SerializeField]
    private int value = 0;

    public BuiltInEnumType builtInEnumType = BuiltInEnumType.QuestTabEnum;
    private IndexPropertyChangeScriptBase _indexPropertyChangeScriptBaseImplementation;


    protected override int Value
    {
        get => value;
        set => this.value = value;
    }
    
    public override void Prepare()
    {
        _indexPropertyChangeScriptBaseImplementation.Prepare();
    }

    protected override string GetName()
    {
        return "index_property_change_" + new EnumList(builtInEnumType.ToString()).GetRawName(value).ToLower();
    }
}