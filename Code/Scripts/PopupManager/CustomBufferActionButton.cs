using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBufferActionButton  : BaseButton
{
    [SerializeField] private bool showInters = false;
    [SerializeField] [ConditionalField(nameof(showInters))] private string showIntersLog;
    [SerializeField] [ConditionalField(nameof(showInters))] private bool conditionLevel = true;
    [SerializeField] [ConditionalField(nameof(showInters))] private bool handlerAfterInters;
    [SerializeField] private BaseBaseActionScript[] actions;
    [SerializeField] private bool checkLevel;
    protected override void OnClick()
    {
        base.OnClick();
        DoOnClick();
    }
    
    protected virtual void DoOnClick()
    {
        // ____ move to inherit class
//        if (!_nextGameRated.BoolValue && PlayerData.currentLevel.Value == RemoteConfigKey.game_play_level_pass_to_rate.GetValueInt())
//        {
//            Pause.Value = true;
//            _nextGameRated.BoolValue = true;
//            ScreenRoot.popupManager.ShowPopup<PopupRateUs>(rateUsPopupLevel).SetActionWhenClose(Action);
//        }
//        else if (popupNoAds && !Kernel.Resolve<AdsManager>().IsNoAds() && DuongExtensions.IsListContains(level_show_no_ads_pack.Value,LevelInt,true))
//        {
//            Pause.Value = true;
//            ScreenRoot.popupManager.ShowPopup<PopupNoAds>(rateUsPopupLevel).SetActionWhenClose(Action);
//        }
//        else
//        {
            if (showInters && !SessionData.skipIntersOnce)
            {
                    var log = showIntersLog.CreateDefaultLogInterstitial();
                    if (handlerAfterInters)
                        Kernel.Resolve<AdsManager>().ShowInterstitial(log, false, conditionLevel, Action);
                    else
                        Kernel.Resolve<AdsManager>().ShowInterstitial(log, false, conditionLevel);
            }
            else if(!handlerAfterInters || !showInters)
                Action();
            else if(showInters && handlerAfterInters && SessionData.skipIntersOnce)
                Action();
            
            SessionData.skipIntersOnce = false;
//        }  
    }

    protected virtual void Action()
    {
#if Bus_Jam_Tool
        actions[1].StartAction();
#else
        if (!checkLevel)
        {
            actions.StartAction();
            return;
        }

        if (PlayerData.CurrentLevel < GameDefine.AutoGoHomeLevel && checkLevel)
        {
            actions[0].StartAction();
        }
        else
        {
            actions[1].StartAction();
        }
#endif
    }
}
