using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductValueRetrieverUpdateAmount : ProductValueRetrieverBase
{
    [SerializeField] private Product toUpdate;
    // Start is called before the first frame update
    [SerializeField] protected IntValueRetrieverBase input;
    [SerializeField] private int multiple = 1;
    [SerializeField] private int add = 0;

    public override Product Product
    {
        get
        {
            var clone = toUpdate.Clone();
            clone.amount = input.Value * multiple + add;
            return clone;
        }
        set { }
    }

    public override void Clear()
    {
    }
}
