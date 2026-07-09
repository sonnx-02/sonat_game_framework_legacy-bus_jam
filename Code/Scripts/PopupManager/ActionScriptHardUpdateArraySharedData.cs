using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptHardUpdateArraySharedData : BaseBaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)]
    [SerializeField] private IntInt[] setup;
    public override void StartAction(int[] arr)
    {
        base.StartAction(arr);
        for (var i = 0; i < setup.Length; i++)
        {
            SharedData[setup[i].value] = arr[setup[i].value2];
        }
    }
}
