using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOpenOffer : GetShopItemView
{
    [SerializeField] private Button btn;
    [SerializeField] private CountDownView countDownView;

     
    private Offer _offer;
    public Offer Offer => _offer ?? (_offer = RootView.rootView.ShopDatabase.GetOffer(key));
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        btn.onClick.AddListener(() =>
        {
            RootView.rootView.screenRoot.popupManager.Resolve<PopupOffer>().CheckShow(Key());
        });
    }

    public void CheckActive()
    {
        if (Offer.IsOfferActive())
        {
            gameObject.SetActive(true);
            if (countDownView != null && gameObject.activeInHierarchy)
            {
                countDownView.StartCountDown(DateTime.FromBinary(Offer.TimeTick[key]), () =>
                {
                    gameObject.SetActive(false);
                });
            }
        }
        else
            gameObject.SetActive(false);
    }
    
    protected override string GetName()
    {
        return base.GetName() +"_"+ ProjectSettings.GetKey(BuiltInEnumType.ShopItemKey.ToString(),key);
    }
}
