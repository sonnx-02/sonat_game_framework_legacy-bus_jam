using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DependencyOpenBox : BasePopupDependency
{
    [SerializeField] private Product[] price;
    [SerializeField] private Product[] product;
    [SerializeField] private Product[] productByAds;
    [SerializeField] private RewardView productView;
    [SerializeField] private RewardView productByAdsView;
    [SerializeField] private Button claimButton;
    [SerializeField] private StandaloneAdsButtonView adsRewardButton;
    [SerializeField] private BaseRxAnimator[] animatorBeforeClaim;
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int soundClick;

    [SerializeField] private ToggleScript[] toggleOpened;

    [SerializeField] private float delayToClaim = 1;
    [SerializeField] private float delayToClose = 1;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (productView != null)
            productView.SetData(product[0]);
        if (productByAdsView != null)
            productByAdsView.SetData(productByAds[0]);
        
        claimButton.onClick.AddListener(() =>
        {
            soundClick.PlaySound();
            Claim(false);
        });
        
        if (adsRewardButton != null)
        {
            var onRewardAction = new BaseAction()
            {
                Action = ClaimByAds
            };
            adsRewardButton.SetAction(onRewardAction,productByAds[0]);
        }
        
    }

    private void ClaimByAds()
    {
        Claim(true);
    }
    
    private void Claim(bool byAds)
    {
        var toClaim = byAds ? productByAds : product;
        PlayerData.Claim(price);
        toggleOpened.OnChanged(true);

        delayToClaim.Timer(() =>
        {
                PlayerData.Claim(price);
                PlayerData.Claim(toClaim); 
        });

        var parallel = Observable.WhenAll(animatorBeforeClaim.ToList().Select(x => x.IO_AnimatorIn()).ToArray())
            .Subscribe(
                data =>
                {
                    delayToClose.Timer(() =>
                    {
                        ParentPopup.Close();
                    });
                });
    }

    public override void OnShow()
    {
        base.OnShow();
        toggleOpened.OnChanged(false);
    }
}
