using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : BaseShopItemView
{
    [SerializeField] private ToggleScript[] affordableBindings;
    // ReSharper disable once ConvertToAutoProperty

    protected override void OnEnable()
    {
        base.OnEnable();
        if (ShopItem != null)
        {
            var affordable = PlayerData.IsAffordable(ShopItem.Price);
            affordableBindings.OnChanged(affordable);
        }
    }



    protected override string GetName()
    {
        return base.GetName() + "_" + ProjectSettings.GetKey(BuiltInEnumType.ShopItemKey.ToString(), key);
    }

//    [SerializeField] [MyButtonInt(nameof(OverrideKey), BuiltInEnumType.ShopItemKey)]
//    private int testNewKey;

    public override void LogCustom()
    {
        base.LogCustom();
//        shopItem = RootView.rootView.ShopDatabase.GetShopItem(key);
//        UpdateView();
    }


    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (ShopItem == null)
        {
            Debug.LogError("not found " + key, gameObject);
            Debug.LogError("database", RootView.rootView.ShopDatabase);
        }
    }

    public BaseBaseActionScript[] actionOnSuccess;

    protected override void OnSuccess()
    {
        base.OnSuccess();
        foreach (var baseActionScript in actionOnSuccess)
        {
#if UNITY_EDITOR
            if (baseActionScript is BaseRxAnimator)
                Debug.LogError("check again if this is correct", gameObject);
#endif
            baseActionScript.StartAction();
        }
    }
}