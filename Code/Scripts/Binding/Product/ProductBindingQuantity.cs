using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseProductBindingToggle : BaseActionProductScript
{
    [SerializeField] private bool setActiveThis;
    [SerializeField] protected bool reverse;
    [SerializeField] protected ToggleScript[] toggles;

    protected void Toggle(bool toggle)
    {
        if(reverse)
            DoToggle(!toggle);
        else
            DoToggle(toggle);
    }
    
    protected virtual void DoToggle(bool finalToggle)
    {
        if(setActiveThis)
            gameObject.SetActive(finalToggle);
        toggles.OnChanged(finalToggle);
    }
}


public class ProductBindingQuantity : BaseProductBindingToggle
{
    [SerializeField] private Quantity[] quantities;
    
    public override void StartAction(Product p)
    {
        base.StartAction(p);
        bool valid = false;
        foreach (var quantity in quantities)
            if (quantity == p.quantity)
            {
                valid = true;
                break;
            }

        Toggle(valid);
    }
}