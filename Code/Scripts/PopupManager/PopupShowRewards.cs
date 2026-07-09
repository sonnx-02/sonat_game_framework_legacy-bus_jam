using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class PopupShowRewards : PopupShowRewardBase
{
    [SerializeField] private float delayToClaim = 1;
    [SerializeField] private float delayToClose = 1;

    [SerializeField] private RewardView[] rewardViews;
    [SerializeField] private BaseRxAnimator[] animatorBeforeClaim;
    public override void SetData(ShowRewardInput input, Action actionAfterClaim = null)
    {
        base.SetData(input, actionAfterClaim);
        rewardViews.SetActiveAll(false);
        int count = 0;
        foreach (var tradeProduct in input.trade.Products)
        {
            rewardViews[count].gameObject.SetActive(true);
            rewardViews[count].SetData(tradeProduct);
            count++;
        }

        claimButton.interactable = true;
    }

    public override void Claim(bool byAds)
    {
        base.Claim(byAds);
        claimButton.interactable = false;
        Input.trade.Spend();
        delayToClaim.TimerUnscale(() =>
        {
            PlayerData.Claim(Input.trade.Products);
            PlayerData.Save();
            DailyData.Save();
            Action?.Invoke();
            DuongExtensions.ExecuteInNextFrame(PlayerData.CheckAndTurnOnSave);
        });
        
        if (byAds)
        {
            if(Input.logWhenAdsClaim != null)
                foreach (var log in Input.logWhenAdsClaim)
                    log.Post();
        }
        else
        {
            if(Input.logWhenClaim != null)
                foreach (var log in Input.logWhenClaim)
                    log.Post();
        }

        Observable.WhenAll(animatorBeforeClaim.ToList().Select(x => x.IO_AnimatorIn()).ToArray())
            .Subscribe(
                data =>
                {
                    // nếu có log thì show quảng cáo
                    if (Input.logInterstitial != null && !byAds)
                        (delayToClose + delayToClaim).TimerUnscale(() => Close(() =>
                            Kernel.Resolve<AdsManager>().ShowInterstitial(Input.logInterstitial)));
                    else
                        (delayToClose + delayToClaim).TimerUnscale(Close);
                });
    }
}
