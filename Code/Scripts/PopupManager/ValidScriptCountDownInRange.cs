using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidScriptCountDownInRange : ValidScriptBase
{
    [IndexAsEnum(BuiltInEnumType.CustomTimeTick)] [SerializeField]
    private int customTimeTickKey = (int) BaseCustomTimeTick.CountDownUnlimitedLives;
    protected override bool IsThisValid()
    {
        return PlayerData.customTimeTick[customTimeTickKey] > DateTime.Now.Ticks;
    }
}
