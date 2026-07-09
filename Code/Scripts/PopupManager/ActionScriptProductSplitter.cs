using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptProductSplitter : BaseActionProductScript
{
    [IndexAsEnum(BuiltInEnumType.Quantity)]
    [SerializeField] private IntBool quantityCondition;
    [SerializeField] private BaseBaseActionScript[] updateQuantity;
    [SerializeField] private BaseBaseActionScript[] updateIndex;
    [SerializeField] private BaseBaseActionScript[] updateAmount;
    [SerializeField] private BaseBaseActionScript[] updateParam1;
    [SerializeField] private BaseBaseActionScript[] updateParam2;
    [SerializeField] private BaseBaseActionScript[] updateArray;
    [SerializeField] private IndexBindingScriptBase[] bindQuantity;
    [SerializeField] private IndexBindingScriptBase[] bindIndex;
    [SerializeField] private IndexBindingScriptBase[] bindAmount;
    [SerializeField] private IndexBindingScriptBase[] bindParam1;
    [SerializeField] private IndexBindingScriptBase[] bindParam2;
    [SerializeField] private IndexIndexBindingScriptBase[] bindQuantityIndex;
    [SerializeField] private IndexIndexBindingScriptBase[] bindQuantityParam1;
    public override void StartAction(Product product)
    {
        base.StartAction(product);
        if (!quantityCondition.value2 || quantityCondition.value == (int) product.Quantity)
        {
            updateQuantity.StartAction((int) product.Quantity);
            updateIndex.StartAction(product.index);
            updateAmount.StartAction(product.Amount);
            updateParam1.StartAction(product.parameter);
            updateParam2.StartAction(product.parameter2);
            
            bindQuantity.OnChanged((int) product.Quantity);
            bindQuantityIndex.OnChanged((int) product.Quantity,product.index);
            bindQuantityParam1.OnChanged((int) product.Quantity,product.parameter);
            bindIndex.OnChanged(product.index);
            bindAmount.OnChanged(product.Amount);
            bindParam1.OnChanged(product.parameter);
            bindParam2.OnChanged(product.parameter2);
            
            foreach (var baseBaseActionScript in updateArray)
                baseBaseActionScript.StartAction(new[]
                {
                    (int) product.Quantity,
                    product.index,
                    product.Amount,
                    product.parameter,
                    product.parameter2,
                });
        }
    }
}
