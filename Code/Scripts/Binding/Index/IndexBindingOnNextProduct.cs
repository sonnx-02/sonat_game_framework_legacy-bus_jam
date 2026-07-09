using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingOnNextProduct : IndexBindingScript
{
    [SerializeField] private Product[] products;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var product = products[computedValue];
        if (product.quantity == Quantity.GameAction)
            SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(products[computedValue]));
    }
}
