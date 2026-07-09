using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DeactiveIfNoAds : BaseFrameworkView
{
    // Start is called before the first frame update

    [SerializeField] private bool continueChecking = true;
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private bool deactivate = true;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(Kernel.Resolve<AdsManager>().IsNoAds() && deactivate)
            gameObject.SetActive(false);
        toggles.OnChanged(Kernel.Resolve<AdsManager>().IsNoAds());
        if (continueChecking)
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(20)
                .Subscribe(data =>
                {
                    if (Kernel.Resolve<AdsManager>().IsNoAds() && deactivate)
                        gameObject.SetActive(false);
                    
                    toggles.OnChanged(Kernel.Resolve<AdsManager>().IsNoAds());
                });
    }

    private void OnEnable()
    {
        Check();
        
    }

    public void Check()
    {
        if (Kernel.Resolve<AdsManager>().IsNoAds() && deactivate)
            gameObject.SetActive(false);
        toggles.OnChanged(Kernel.Resolve<AdsManager>().IsNoAds());
        SubjectController.OnShopItemBought.TakeUntilDisable(this)
            .Subscribe(data =>
            {
                if (Kernel.Resolve<AdsManager>().IsNoAds() && deactivate)
                    gameObject.SetActive(false);
                toggles.OnChanged(Kernel.Resolve<AdsManager>().IsNoAds());
            });
    }
}
