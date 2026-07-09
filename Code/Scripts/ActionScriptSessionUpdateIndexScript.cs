using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSessionUpdateIndexScript : BaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    [SerializeField] private IntBool type;
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    [SerializeField] private int value;
    [SerializeField] private IndexBindingScript[] updates;


    protected override void DoAction()
    {
        base.DoAction();
        var indexValue = value.GetSessionDataCustomProperty().Value;
        if (type.value2)
        {
            var typeValue = type.value.GetSessionDataCustomProperty().Value;
            foreach (var indexBindingScript in updates)
                indexBindingScript.OnChangedTypeValue(typeValue,indexValue);
        }
        else
            foreach (var indexBindingScript in updates)
                indexBindingScript.OnChanged(indexValue);
    }

    
}
