using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetPlayerPrefs : BaseBaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.PlayerPrefEnum)]
    [SerializeField] private int playerPrefKey = (int) PlayerPrefEnum.played;
    private PlayerPrefInt _key;

    [SerializeField] private int setOn;
    [SerializeField] private int setOff;
    protected override void DoAction()
    {
        base.DoAction();
        if(_key == null)
            _key = new PlayerPrefInt((PlayerPrefEnum)playerPrefKey);
        _key.Value = setOn;
    }

    public override void StartAction(bool value)
    {
        base.StartAction(value);
        if(_key == null)
            _key = new PlayerPrefInt((PlayerPrefEnum)playerPrefKey);
        _key.Value = value ?  setOn : setOff; 
    }
}
