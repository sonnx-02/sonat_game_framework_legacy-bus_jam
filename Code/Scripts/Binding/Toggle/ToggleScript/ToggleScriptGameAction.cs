using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptGameAction : ToggleScript
{
    [SerializeField] private Product gameActionOn;
    [SerializeField] private bool offAction;
    [SerializeField] private Product gameActionOff;

    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        if (val)
            SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(gameActionOn));
        else if (offAction)
            SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(gameActionOff));
    }
}