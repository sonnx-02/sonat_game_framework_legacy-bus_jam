using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductGameActionButton : BaseButton
{
    [SerializeField] protected Product product;

    protected override void Handler()
    {
        SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(product));
    }

    private void Reset()
    {
        if(product == null)
            product = new Product(Quantity.GameAction,1);
    }
}
