using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UniRx;
using UnityEngine;

public class DuongPurchaser : BasePurchaser
{
    [SerializeField] private bool dummy;

  

    protected override void Register()
    {
        base.Register();
        if (dummy)
        {
            SetDummy(dummy);
            Debug.LogError("turn off dummy on build");
        }
    }

    public override List<StoreProductDescriptor> StoreProductDescriptors =>
        RootView.rootView.ShopDatabase.storeProductDescriptors;

    public override Type GetThisType()
    {
        return typeof(BasePurchaser);
    }

    public Subject<bool> trueInitialized = new Subject<bool>();

    protected override void OnInitializedSuccessfully()
    {
        base.OnInitializedSuccessfully();
        trueInitialized.OnNext(true);
    }

    public override bool RestorePurchase(bool isAuto = false)
    {
       bool noAds = base.RestorePurchase();
       if(!isAuto)
            RootView.rootView.gameController.HandlerRestoreNoAds(noAds);
       return noAds;
    }


    public override string GetPriceText(int shopItemKey)
    {
#if UNITY_EDITOR

        if (RootView.rootView.ShopDatabase.storeProductDescriptors.Find(x => x.key == shopItemKey) == null)
        {
            Debug.LogError("not found shop item"+BuiltInEnumType.ShopItemKey.GetName(shopItemKey));
        }
        return "$" +  RootView.rootView.ShopDatabase.storeProductDescriptors.Find(x => x.key == shopItemKey)
                   .price.ToString(CultureInfo.InvariantCulture);
#endif
        return base.GetPriceText(shopItemKey);
    }
}
