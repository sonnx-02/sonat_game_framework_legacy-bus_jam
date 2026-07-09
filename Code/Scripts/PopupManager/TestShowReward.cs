using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShowReward : BaseFrameworkView
{
    [SerializeField] private Product[] products;

    public override void LogCustom()
    {
        base.LogCustom();
        if (products.Length == 1)
        {
            RootView.rootView.screenRoot.popupManager.ShowPopup<PopupShowReward>(-1)
                .SetData(new ShowRewardInput(new Trade<Product, Product>(new []{Product.Free},new []{products[0]},TradeLog.Empty)),() =>{});
        }
    }
}
