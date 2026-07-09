using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckScriptGameController : BaseCheckScript
{
    [IndexAsEnum(BuiltInEnumType.GcCheckerEnum)]
    [SerializeField] private int key;

    [SerializeField] private Transform target;

    public Transform Target => target == null ? transform : target;
    public override bool Check()
    {
        var backCheck = base.Check();
        return backCheck && BaseGameController.GcCheck(key, this);
    }
}
