using System.Collections;
using System.Collections.Generic;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum UrlEnum
{
    FanPage,
    Policy,
    RateUs,
    MoreGame,
}

[RequireComponent(typeof(Button))]
public class OpenRemoteSettingUrlButton : BaseButton
{
    public UrlEnum url;

    protected override void Register()
    {
        base.Register();
    }

    public  bool RateClicked
    {
        get => PlayerPrefs.GetInt("rate_clicked") == 1;
        set => PlayerPrefs.SetInt("rate_clicked", value ? 1 : 0);
    }
    
    protected override void OnClick()
    {
        base.OnClick();
        AdsManager.AppLeaving = 30;
        switch (url)
        {
            case UrlEnum.FanPage:
                Application.OpenURL(RemoteConfigKey.fan_page_url.GetValueString());
                break;
            case UrlEnum.Policy:
                Application.OpenURL(RemoteConfigKey.policy_url.GetValueString());
                break;
            case UrlEnum.RateUs:
                RateClicked = true;
                new SonatLogShowRate()
                {
                    open_by = SonatLogShowRate.ShowRateOpenBy.user,
                    action = SonatLogShowRate.ShowRateAction.rate_now
                }.Post();
#if UNITY_ANDROID
            #if AMZ
                Debug.Log("amzn://apps/android?p=" + Application.identifier);
                Application.OpenURL("amzn://apps/android?p=" + Application.identifier);
            #else
                Application.OpenURL("market://details?id=" + Application.identifier);    
            #endif
#else
                Application.OpenURL("itms-apps://itunes.apple.com/app/id"+Kernel.Resolve<AppFlyerController>().appID_IOS);
#endif
                break;
            case UrlEnum.MoreGame:
                Application.OpenURL(RemoteConfigKey.more_game_url.GetValueString());
                break;
        }
    }


    protected override string GetName()
    {
        return $"btn_open_url_{url.ToString()}";
    }
}