using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingScriptProductUpdate : IndexBindingScriptGetRewardsBase
{
    [SerializeField] private Product product;

    protected override void OnChangedType(int type)
    {
        base.OnChangedType(type);
        product.index = type;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        Rewards = new ListProduct();
        Rewards.products = new[]
        {
            product.Clone(computedValue)
        };
        for (var i = 0; i < Rewards.products.Length; i++)
        {
            rewardViews[i].gameObject.SetActive(true);
            rewardViews[i].SetData(Rewards.products[i]);
        }
    }
}