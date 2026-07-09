using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexPropertyChangeActionScriptOnChanged : IndexPropertyChangeActionScript
{
    [SerializeField] private bool updateScriptOnEnable = true;
    [SerializeField] private IndexBindingScript[] bindingScripts;
    protected override void OnEnable()
    {
        base.OnEnable();
        if (updateScriptOnEnable)
        {
            bindingScripts.OnChanged(Property.Value);
        }
    }
}
