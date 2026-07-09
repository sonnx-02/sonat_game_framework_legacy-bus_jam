using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetSharedData : BaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
    private IntInt[] defaultSet; // preset
    [Header("which value change by StartAction(int index)")]
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
    private int change; // preset

    public override void StartAction(int index)
    {
        base.StartAction(index);
        SharedData[change] = index;
    }

    protected override void DoAction()
    {
        base.DoAction();
        foreach (var intInt in defaultSet)
            SharedData[intInt.value] = intInt.value2;
    }
}
