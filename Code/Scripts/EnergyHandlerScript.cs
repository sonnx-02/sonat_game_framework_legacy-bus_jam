using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyHandlerScript : BaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.PlayerPrefEnum)]
    [SerializeField] private int playerPrefKey = (int) PlayerPrefEnum.played;
    private PlayerPrefInt _key;
    
    public void Handler(bool isWin)
    {
//        base.StartAction(isWin);
        if(_key == null)
            _key = new PlayerPrefInt((PlayerPrefEnum)playerPrefKey);
      
        if (!isWin && _key.BoolValue && PlayerData.energy.Value > 0
                           && PlayerData.currentLevel.Value >= RemoteConfigKey.start_level_consume_energy.GetValueInt()
                           && RemoteConfigKey.enable_energy.GetValueBoolean()
                           &&  !PlayerData.IsInRange((int) BaseCustomTimeTick.CountDownUnlimitedLives)
                           )
        {
            if(PlayerData.energy.Value > 0)
                PlayerData.energy.Value--;
        }
        _key.BoolValue = false;
    }

    [SerializeField] private bool defaultToggle;

    protected override void DoAction()
    {
        base.DoAction();
        Handler(defaultToggle);
    }
}
