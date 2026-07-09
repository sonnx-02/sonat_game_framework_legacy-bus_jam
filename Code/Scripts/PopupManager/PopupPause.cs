using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopupPause : BasePopup
{ 
//    public Button backToHome;

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        BaseGameController.UpdateActionTime();
    }

    protected override void Register()
    {
        base.Register();
//        BackToHome.onClick.AddListener(() =>
//        {
//            AdsManager.Instance.ShowInterstitial("back_to_home",false);
//            Observable.EveryUpdate()
//                .Where(_ => !AdsManager.AdsShowed)
//                .Take(1)
//                .Subscribe(data =>
//                {
//                    RootView.rootView.audioManager.AudioClickButton();
//                    Locator.ScreenRoot.ShowScreenAsObservable(ScreenEnum.Menu,true).Subscribe();
//                })
//                .AddToWaveDisposable()
//                .AddToScreenChangeDisposable();
//        });
    }
}
