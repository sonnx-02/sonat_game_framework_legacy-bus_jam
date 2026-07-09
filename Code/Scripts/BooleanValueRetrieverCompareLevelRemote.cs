using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BooleanValueRetrieverCompareLevelRemote : BooleanValueRetrieverMathCompare
{
    [IndexAsEnum(BuiltInEnumType.RemoteConfigKey)] [SerializeField]
    private int key;

    [SerializeField] private string keyString;
    protected override int CompareValue1 => PlayerData.CurrentLevel;
    protected override int CompareValue2
    {
        get
        {
            if (!string.IsNullOrEmpty(keyString))
                return (int) RemoteConfigHelper.GetValueInt(keyString);
            return (int) ((RemoteConfigKey) key).GetValueInt();
        }
    }
}