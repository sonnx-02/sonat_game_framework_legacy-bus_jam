using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductValueRetrieverSharedData : ProductValueRetrieverBase
{
    public override Product Product
    {
        get => SharedProduct;
        set { }
    }
}