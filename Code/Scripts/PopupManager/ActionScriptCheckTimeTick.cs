using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptCheckTimeTick : BaseActionScript
{
    [SerializeField] private SourceDataEnum source = SourceDataEnum.PlayerData;
    [IndexAsEnum(BuiltInEnumType.CustomTimeTick)]
    [SerializeField] private int timeTickKey;
    [SerializeField] private ToggleScript[] toggles; 
    [SerializeField] private BaseBaseActionScript[] whenTrue; 
    protected override void DoAction()
    {
        base.DoAction();
        switch (source)
        {
            case SourceDataEnum.PlayerData:
                var calculated = PlayerData.IsInRange(timeTickKey);
                toggles.OnChanged(calculated);
                if(calculated)
                    foreach (var baseActionScript in whenTrue)
                        baseActionScript.StartAction();
                break;
            case SourceDataEnum.DailyData:
                break;
            case SourceDataEnum.WeeklyData:
                break;
            case SourceDataEnum.SessionData:
                break;
            case SourceDataEnum.WaveData:
                break;
            case SourceDataEnum.GameController:
                break;
            case SourceDataEnum.RootView:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    
}
