using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptUpdateSharedProductAmount : BaseActionScriptInput
{
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedObject)]
    [SerializeField] private int key;
    [SerializeField] private Product toUpdate;
    // Start is called before the first frame update

    public override void StartAction()
    {
        base.StartAction();
        var clone = toUpdate.Clone();
        clone.amount = input.Values[0];
        SharedObject[key] = clone;
    }
}
