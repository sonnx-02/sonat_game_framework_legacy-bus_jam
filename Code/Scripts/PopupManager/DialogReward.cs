using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogReward : BaseDialog
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int soundClick;
    [SerializeField] private BaseActionProductScript[] productBindings;
    [SerializeField] private IndexBindingScriptBase[] amountBindings;
    [SerializeField] private Button takeButton;
    protected override void Register()
    {
        base.Register();
        if(takeButton != null)
            takeButton.onClick.AddListener(() =>
            {
                soundClick.PlaySound();
                PlayerData.Claim(_product);
                Close();
            });
    }

    private Product _product;

    public void Show(Product product)
    {
        base.Show();
        _product = product;
        amountBindings.OnChanged(product.amount);
        foreach (var productBinding in productBindings)
            productBinding.StartAction(product);
    }

  
}
