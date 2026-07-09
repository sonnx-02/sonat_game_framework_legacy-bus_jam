using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanValueRetrieverIndexPropertySource : BooleanValueRetrieverBase
{
    [SerializeField] private IndexPropertySourceRetrieverMultiple source;
    [SerializeField] private int index;
    protected override bool Value
    {
        get => source.models[index].Property.BoolValue;
        set{}
    }
}
