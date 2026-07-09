using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStringSharedSessionData : BaseGetStringScript
{
    [IndexAsEnum(BuiltInEnumType.SessionSharedString)] [SerializeField]
    private int key;

    public override string GetString()
    {
        return SessionData.sharedString[key];
    }
}
