using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class PopupWin : BasePopup
{
    [SerializeField] private TweenDisplayText earnedCoin;
    [SerializeField] private TweenDisplayText coinText;
    [SerializeField] private TweenDisplayText gemText;

    [SerializeField] private GameObject randomXObj;
    [SerializeField] private GameObject buttonAds;
    [SerializeField] private CanvasGroup buttonNormal;
    [SerializeField] private RectTransform unlockFeature;
    [SerializeField] private RectTransform bottomButtons;
    [SerializeField] Vector2[] UnlockFeaturePos;
    [SerializeField] Vector2[] bottomButtonsPos;

    public UIPopupWinRandom winRandom;


    [MyButtonInt(nameof(TestShowScore))] [SerializeField]
    private int abc123;

    public void TestShowScore(int n)
    {
        ScreenRoot.popupManager.ShowPopup<PopupWin>()
            .ShowScore(n);
    }

    public BaseTweenScript[] tweenOnCoinUp;

    public void ShowScore(int score)
    {
        foreach (var baseTweenScript in tweenOnCoinUp)
            baseTweenScript.ResetTween();

        gemText.SetTo(PlayerData.gem.Value);
        coinText.TweenTo(PlayerData.gold.Value);
//        earnedCoin.SetTo(0);
//        earnedCoin.TweenTo(score);
//        
        earnedCoin.SetTo(score);
        Observable.Timer(TimeSpan.FromSeconds(1f))
            .Subscribe(data =>
            {
                earnedCoin.TweenTo(0);
                coinText.TweenTo(PlayerData.gold.Value + score);
                foreach (var baseTweenScript in tweenOnCoinUp)
                {
                    baseTweenScript.Tween();
                }
            });
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);

        //bool levelAds = buttonAds.activeSelf;
        bool levelAds = PlayerData.CurrentLevel >= RemoteConfigKey.start_level_show_interstitial.GetValueInt();

        if (unlockFeature != null)
        {
            unlockFeature.anchoredPosition = levelAds ? UnlockFeaturePos[0] : UnlockFeaturePos[1];
            bottomButtons.anchoredPosition = levelAds ? bottomButtonsPos[0] : bottomButtonsPos[1];
            randomXObj.SetActive(levelAds);

            buttonNormal.alpha = levelAds ? 0 : 1;
            buttonNormal.interactable = levelAds ? false : true;

            if (levelAds)
            {
                DOVirtual.DelayedCall(4.5f, () =>
                {
                    buttonNormal.DOFade(1, 0.5f);
                    buttonNormal.interactable = true;
                });

                //winRandom.StartRamdomRun();
            }
            else
            {
                buttonNormal.DOFade(1, 0.5f);
                buttonNormal.interactable = true;
            }
        }
    }

    public void OnXRewardAdsClick()
    {
        //winRandom.StopRandomRun();
    }
}