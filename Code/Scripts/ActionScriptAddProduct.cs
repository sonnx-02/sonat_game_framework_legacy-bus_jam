using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptAddProduct : BaseActionScript
{
    [SerializeField] private Product[] products;
    [SerializeField] private RewardView rewardBindings;
    [SerializeField] private float delay;

    protected override void Register()
    {
        base.Register();
        if (products.Length > 0)
            if (rewardBindings != null)
                rewardBindings.SetData(products[0]);
    }

    protected override void DoAction()
    {
        base.DoAction();
        delay.Timer(RealAction);
    }

    private void RealAction()
    {
        foreach (var product in products)
            PlayerData.Claim(product);
    }
}
