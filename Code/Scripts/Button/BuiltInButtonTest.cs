using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sonat;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Rendering;

public enum BuiltInFunctionTest
{
    FillTestAds,
    EnableNoAds,
    DisableNoAds,
    ShowAdsIds,
    ShowStatistics,
    Vibrate,
    AdmobInspector,
    AddSessionDataValue,
    ResetGDPR,
    SetTestDevice,
    LogCustom,
    DeleteAllDataQuit,
    ConsentStatus,
    TurnOffAllShadow,
    LoadGdpr,
}

[RequireComponent(typeof(Button))]
public class BuiltInButtonTest : BaseButton
{
    [SerializeField] private BuiltInFunctionTest Enum;
    [SerializeField] private int value;
    [SerializeField] private int value2;

    protected override void OnClick()
    {
        Handler(Enum, value, value2);
    }

    public void Handler(BuiltInFunctionTest function, int val, int val2)
    {
        switch (function)
        {
            case BuiltInFunctionTest.FillTestAds:
                Kernel.Resolve<AdsManager>().EnableTestAdsThenClearAds();
                break;
            case BuiltInFunctionTest.EnableNoAds:
                Kernel.Resolve<AdsManager>().EnableNoAds();
                break;
            case BuiltInFunctionTest.DisableNoAds:
                Kernel.Resolve<AdsManager>().DisableNoAds();
                break;
            case BuiltInFunctionTest.ShowAdsIds:
                Kernel.Resolve<AdsManager>().ShowAdsIds();
                break;
            case BuiltInFunctionTest.ShowStatistics:
                ShowStatistic();
                break;
            case BuiltInFunctionTest.Vibrate:
                DuongVibration.Vibration(val);
                break;
            case BuiltInFunctionTest.AdmobInspector:
                Kernel.Resolve<AdsManager>().ShowDebugInspector();
                break;
            case BuiltInFunctionTest.AddSessionDataValue:
                SessionData.customPropertyList[val].Value += val2;
                break;
            case BuiltInFunctionTest.ResetGDPR:
                Kernel.Resolve<AdsManager>().Resolve<AdsInstanceAdmob>().ResetGDPR();
                PlayerPrefs.DeleteKey(PlayerPrefEnum.cache_consent_admob.ToString());
                break;
            case BuiltInFunctionTest.SetTestDevice:
                SonatExtenstions.SetTestDeviceAdmob(val == 0);
                break;
            case BuiltInFunctionTest.LogCustom:
                BaseGameController.LogCustomForTesting();
                Log();
                break;
            case BuiltInFunctionTest.DeleteAllDataQuit:
                BaseLoadReactiveData.IgnoreSave = true;
                ReactivePlayerData.DeleteFile();
                ReactivePlayerDailyData.DeleteFile();
                PlayerPrefs.DeleteAll();
                Application.Quit();
                break;
            case BuiltInFunctionTest.ConsentStatus:
#if use_admob
                UIDebugLog.Log($"AdmobConsentHelper.CanShowAds()" + AdmobConsentHelper.CanShowAds());
#endif
                break;
            case BuiltInFunctionTest.TurnOffAllShadow:
                foreach (var r in FindObjectsOfType<Renderer>(true))
                    r.shadowCastingMode = ShadowCastingMode.Off;
                break;
            case BuiltInFunctionTest.LoadGdpr:
                Kernel.ResolveDirty<AdsManager>().LoadGdpr();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void Log()
    {
        UIDebugLog.Log($"AdsManager.BannerHeight {AdsManager.BannerHeight}" +
                       $" HaveNotch{HaveNotch()}  Screen.height{Screen.height} ");
    }

    public static bool HaveNotch()
    {
        return Screen.safeArea.height < Screen.height;
    }

    public static void ShowStatistic()
    {
        UIDebugLog.Log("refreshRate:" + Screen.currentResolution.refreshRate);
        UIDebugLog.Log("Screen.safeArea.height:" + Screen.safeArea.height);
        UIDebugLog.Log("Screen.height:" + Screen.height);
    }

    // Update is called once per frame

    protected override string GetName()
    {
        return "btn_" + Enum;
    }
}