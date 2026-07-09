using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PopupWifiHandler : BaseFrameworkViewCheck
{
    private IDisposable _checkForTurnOff;
    
    public override void Check()
    {
        if(!Kernel.IsReady())
            return;
        if(!RemoteConfigKey.internet_connection.GetValueBoolean())
            return;
        LogCustomInt(!Kernel.IsInternetConnection() ? 1 : 0);
    }
    
    private int _last = 0;
    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        
        if(!Kernel.IsReady())
            return;
        
//        if(Time.time < 15)
//            return;
        
        if(PlayerData.tutorialStep != -1)
            return;
        
        if(PlayerData.playTimes < (RemoteConfigKey.internet_connection_level).GetValueInt())
            return;
        
        if(_last == value)
            return;
        
        if(ScreenRoot.dialogController.CurrentDialog != null && value == 1)
            return;
        
        if (value > 0)
        {
            var dialog = ScreenRoot.dialogController.Resolve<DialogWifi>();
            dialog.Show();
            ScreenRoot.TurnRayCast(false);
            _last = value;
//            Time.timeScale = 0;
            BaseGameController.FreeAction();
            _checkForTurnOff?.Dispose();
            _checkForTurnOff = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(data =>
                {
#if UNITY_EDITOR
                    Debug.Log("Check wifi "+data);
#endif
                    if (Kernel.IsInternetConnection() && data > 0)
                    {
                        LogCustomInt(0);
                        _checkForTurnOff?.Dispose();
                    }
                });
        }
        else
        {
//            Time.timeScale = 1;
            if (ScreenRoot.dialogController.CurrentDialog is DialogWifi)
            {
                _checkForTurnOff?.Dispose();
                ScreenRoot.TurnRayCast(true);
                ScreenRoot.dialogController.CloseCurrentAsObservable(true).Subscribe();
                _last = value;
            }
        }
    }
}
