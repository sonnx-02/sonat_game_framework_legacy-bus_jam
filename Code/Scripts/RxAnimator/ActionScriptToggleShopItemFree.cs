using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptToggleShopItemFree : BaseBaseActionScript
{
    [SerializeField] private bool once = true;
    [SerializeField] private BaseBaseShopItemView shopItemView;
    [SerializeField] private bool defaultSet;
    public override void StartAction(bool value)
    {
        base.StartAction(value);
        if(!once)
            shopItemView.SetFree(value);
        else
            shopItemView.SetFreeOnce(value);
    }

    public override void StartAction()
    {
        base.StartAction();
        StartAction(defaultSet);
    }
}
