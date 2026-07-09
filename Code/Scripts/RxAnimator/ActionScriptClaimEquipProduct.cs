using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActionScriptClaimEquipProduct : BaseActionScriptInput
{
    [SerializeField] private Button[] equipButton;
    [SerializeField] private bool autoClaimOnEnable;
    [SerializeField] private BaseBaseActionScript[] afterEquipActions;

    protected override void Register()
    {
        base.Register();
        foreach (var buttons in equipButton)
            buttons.onClick.AddListener(EquipSwitch); 
    }

    protected override void DoAction()
    {
        base.DoAction();
        PlayerData.Claim(input.Product);
    }

    private void EquipSwitch()
    {
        if(input.Product.Quantity == Quantity.EquipData)
            PlayerData.equipData[input.Product.index].Switch(input.Product.parameter);
        afterEquipActions.StartAction();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (autoClaimOnEnable)
        {
            PlayerData.Claim(input.Product);
//            if(input.Product.Quantity == Quantity.EquipData)
//                PlayerData.equipData[input.Product.index].Add(input.Product.parameter);
        }
    }
}
