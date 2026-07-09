using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ValidScriptProperty : ValidScriptBase
{
    [SerializeField] private IndexPropertySourceRetrieverModelBase model;
    protected  IntReactiveProperty Property => model.Property;
    protected override bool IsThisValid()
    {
        return Property.BoolValue;
    }
}
