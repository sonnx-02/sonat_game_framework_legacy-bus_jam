using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;

public class OpenShopLogEvent : BaseBaseActionScriptStartOnEnable
{
    [SerializeField] private string placement;
    [SerializeField] private BaseGetStringScript getPlacement;
 
    protected override void DoAction()
    {
        base.DoAction();
        
        new SonatLogOpenShop()
        {
            placement = string.IsNullOrEmpty(placement) ? getPlacement.GetString() : placement,
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            mode = Kernel.ParameterValueRetriever.Mode,
        }.Post();
    }
}
