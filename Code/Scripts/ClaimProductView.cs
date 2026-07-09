using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClaimProductView : BaseFrameworkView
{
    [SerializeField] private Button claimButton;
    [SerializeField] private Product[] products;
    public Product[] Products => products;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        claimButton?.onClick.AddListener(() =>
        {
            foreach (var product in products)
                PlayerData.Claim(product);
        });
    }

    public override void LogCustom()
    {
        base.LogCustom();
        foreach (var product in products)
            PlayerData.Claim(product); 
    }
}