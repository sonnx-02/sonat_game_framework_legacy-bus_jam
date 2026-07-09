using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ProductValueRetrieverBase : BaseFrameworkView
{
    public abstract Product Product { get; set; }

    public RewardView[] bindOnEnable;
    public BaseActionProductScript[] bindOnEnable2;

    protected void OnEnable()
    {
        foreach (var rewardView in bindOnEnable)
            rewardView.SetData(Product);
        foreach (var rewardView in bindOnEnable2)
            rewardView.StartAction(Product);
    }

    public virtual void Clear()
    {
        Product = null;
    }
}