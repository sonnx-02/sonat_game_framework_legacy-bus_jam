using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogUseShopItemView : BaseDialog
{
//    [SerializeField] private BindingToStringTextChange message;
    private ShopItem ShopItem { get; set; }
    [SerializeField] private IndexBindingScriptBase[] shopItemKeyBindings; 
    public void SetData(ShopItem data)
    {
        ShopItem = data;
        shopItemKeyBindings.OnChanged(ShopItem.Key);
    }

//    public void SetMessage(string mess)
//    {
//        message.OnChanged(mess);
//    }

    protected override void Close()
    {
        base.Close();
        RootView.rootView.gameController.ClearUseShopItem(false);
    }
}
