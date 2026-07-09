using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActionButton : BaseButton
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.ProductGameAction)]
    private int key;
    [SerializeField] private int amount;
    [SerializeField] private int parameter;
    [SerializeField] private int parameter2;
 
    // Start is called before the first frame update
    protected override void OnClick()
    {
        base.OnClick();
        SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(new Product()
        {
            quantity = Quantity.GameAction,
            amount = amount,
            parameter = parameter,
            parameter2 = parameter2,
            index = key
        }));
    }

    // Update is called once per frame

    protected override string GetName()
    {
        return "btnGameRoot_" + ProjectSettings.GetKey(BuiltInEnumType.ProductGameAction.ToString(), key);
    }
}