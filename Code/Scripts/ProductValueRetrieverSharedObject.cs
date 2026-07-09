using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductValueRetrieverSharedObject : ProductValueRetrieverBase
{
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedObject)]
    [SerializeField] private int key;

    public override Product Product
    {
        get => (Product) SharedObject[key];
        set => SharedObject[key] = value;
    }
    public override void Clear()
    {
        SharedObject[key] = null;
    }
}
