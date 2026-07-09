using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListShopItemKey
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int[] keys;
}

public class DependencyMiniShop : BaseScreenDependency
{
    [SerializeField] private IntValueRetrieverBase retriever; 
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    [SerializeField] private IntBool sessionKey; 
    [SerializeField] private ListShopItemKey[] tabs;
    [SerializeField] private ScrollRectUnlockEquipInfinitive scrollRect;
    [SerializeField] private IndexBindingScript[] indexBindings;
    public override void OnShow()
    {
        base.OnShow();
        if (retriever != null)
            Handler(retriever[0]);
        else if(sessionKey.value2)
            Handler(sessionKey.value.GetSessionDataCustomProperty().Value);
    }

    public void Handler(int index)
    {
        Debug.LogError("populate list here"+index,scrollRect);
        if (index >= 0 && index < tabs.Length)
        {
            var tab = tabs[index];
            scrollRect.Populate(tab.keys.Length, (shop, i) =>
            {
                shop.ShopItemView.CheckKernelLoaded();
                shop.ShopItemView.Setup(RootView.rootView.ShopDatabase.GetShopItem(tab.keys[i]).Clone());
                Debug.Log(shop+i.ToString(),shop);
            });
            indexBindings.OnChanged(index);
            scrollRect.onItemClick += HandlerClick;
        }
        else
        {
            ScreenRoot.popupManager.CloseAllPopUpInstant();
        }
    }
    
    private void HandlerClick(UnlockEquipItem scrollItem)
    {
        scrollItem.ShopTryToBuy();
    }
    
  
}
