using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitProduct : BaseFrameworkView
{
    [SerializeField] private Product[] products;
    [SerializeField] private string key = "init_product";
    [SerializeField] private bool notOnProduction;
    [SerializeField] private bool disable;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(disable)
            return;
        if (PlayerPrefs.GetInt(key) == 0)
        {
            foreach (var product in products)
            {
                    
                #if UNITY_EDITOR
                    PlayerData.Claim(product);
                #else
                if (!notOnProduction)
                    PlayerData.Claim(product);
                #endif
            }
            PlayerData.Save();
        }

        PlayerPrefs.SetInt(key, 1);
    }

    public override void LogCustom()
    {
        base.LogCustom();
        foreach (var product in products)
        {
            PlayerData.Claim(product);
        }
    }
}