using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionProductAddGold : BaseActionProductScript
{
    [SerializeField] private ToggleScript[] ifGold;
    [SerializeField] private IndexBindingScript[] bindAmount;
    [SerializeField] private BaseBaseActionScript[] actions;
    
    public override void StartAction(Product product)
    {
        base.StartAction(product);
        ifGold.OnChanged(product.quantity == Quantity.Gold);
        if(product.quantity == Quantity.Gold)
        {
            bindAmount.OnChanged(product.amount);
            actions.StartAction();
        }
    }
}
