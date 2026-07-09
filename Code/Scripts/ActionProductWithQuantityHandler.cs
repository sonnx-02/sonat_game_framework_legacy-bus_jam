using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct QuantityAction
{
    public Quantity quantity;
    public BaseBaseActionScript[] actions;
    public BaseBaseActionScript[] productActions;
}

public class ActionProductWithQuantityHandler : BaseActionProductScript
{
    public BaseBaseActionScript[] sharedActions;
    public BaseBaseActionScript[] defaultActions;
    public BaseBaseActionScript[] defaultProductActions;
    [SerializeField] private QuantityAction[] quantityHandler;

    protected override void DoAction()
    {
        // DO NOT EDIT
       // base.DoAction(); // otherwise if will trigger StartAction(Product product)
        sharedActions.StartAction();
        var prd = Product;
        foreach (var baseBaseActionScript in quantityHandler)
            if (baseBaseActionScript.quantity == prd.quantity)
            {
                baseBaseActionScript.actions.StartAction();
                baseBaseActionScript.productActions.StartAction(prd);
                return;
            }
        defaultActions.StartAction();
        defaultProductActions.StartAction(prd);
    }

    public override void StartAction(Product product)
    {
        // DO NOT EDIT
//        base.StartAction(product);
        var prd = product;
        foreach (var baseBaseActionScript in quantityHandler)
            if (baseBaseActionScript.quantity == prd.quantity)
            {
                baseBaseActionScript.actions.StartAction();
                baseBaseActionScript.productActions.StartAction(prd);
                return;
            }
        defaultActions.StartAction();
        defaultProductActions.StartAction(prd);
    }
}
