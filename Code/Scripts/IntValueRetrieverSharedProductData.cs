using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class IntValueRetrieverSharedProductData : IntValueRetrieverBase
{
    private static readonly int[] Keys = {
        (int) BaseFrameworkSharedData.ProductQuantity,
        (int) BaseFrameworkSharedData.ProductIndex,
        (int) BaseFrameworkSharedData.ProductAmount,
        (int) BaseFrameworkSharedData.ProductParameter1,
        (int) BaseFrameworkSharedData.ProductParameter2,
    };

    public override int Length => Keys.Length;

    public override int this[int index] => SharedData[Keys[index]];
    

    public override void LogCustom()
    {
        base.LogCustom();
        Debug.LogError(Product.GetString());
    }
}
