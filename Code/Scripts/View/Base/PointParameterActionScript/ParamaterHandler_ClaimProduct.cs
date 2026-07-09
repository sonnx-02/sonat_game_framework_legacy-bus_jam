using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamaterHandler_ClaimProduct : ParameterHandlerScript
{
    [SerializeField] private Product product;
    protected override void DoAction()
    {
        base.DoAction();
        PlayerData.Claim(product.AlterAmount(parameters.value));
    }
}
