using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidScriptPlayerPref : ValidScriptBase
{
    [IndexAsEnum(BuiltInEnumType.PlayerPrefEnum)]
    [SerializeField] private int playerPrefKey = (int) PlayerPrefEnum.played;
    private PlayerPrefInt _key;
    [SerializeField] private int compare;
    protected override bool IsThisValid()
    {
        if(_key == null)
            _key = new PlayerPrefInt((PlayerPrefEnum)playerPrefKey);
        return _key.Value == compare;
    }
}
