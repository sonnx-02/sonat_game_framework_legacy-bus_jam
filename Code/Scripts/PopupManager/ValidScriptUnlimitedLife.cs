using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidScriptUnlimitedLife : ValidScriptBase
{
    [IndexAsEnum(BuiltInEnumType.CustomTimeTick)] [SerializeField]
    private int customTimeTickKey = (int) BaseCustomTimeTick.CountDownUnlimitedLives;

    
    [SerializeField] private bool energyAvailableIncluded = true;
    protected override bool IsThisValid()
    {
        return PlayerData.customTimeTick[customTimeTickKey] > DateTime.Now.Ticks ||  energyAvailableIncluded && PlayerData.energy.Value > 0;
    }
}
