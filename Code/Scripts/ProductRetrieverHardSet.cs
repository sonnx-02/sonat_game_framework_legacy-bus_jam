using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniRx;


/// <summary>
/// to many integer properties from one source
/// </summary>
public class ProductRetrieverHardSet : ProductValueRetrieverBase
{
    [SerializeField] private Product product;
    public override Product Product
    {
        get { return product; }
        set { }
    }

  
}