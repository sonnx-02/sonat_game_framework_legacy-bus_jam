using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptUpdateSharedData : BaseActionScriptInput
{
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)]
    [SerializeField] private IntInt[] setup;
    [SerializeField] private BaseBaseActionScript[] invokeAfterSet;
    [SerializeField] private UpdateType type;
    [SerializeField]
    private enum UpdateType
    {
        UpdateByRetrieverOrHardSet,
        UpdateAction0ByIndex,
    }
    protected override void DoAction()
    {
        if (type == UpdateType.UpdateByRetrieverOrHardSet)
        {
            for (var i = 0; i < setup.Length; i++)
                SharedData[setup[i].value] = input != null ? input.Values[i] : setup[i].value2;
            invokeAfterSet.StartAction();  
        }
    }

    public override void StartAction(int index)
    {
        base.StartAction(index);
        if (type == UpdateType.UpdateAction0ByIndex)
            SharedData[setup[0].value] = index;
    }
}
