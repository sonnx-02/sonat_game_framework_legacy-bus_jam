using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class DialogAddCurrencyBase : BaseDialog
{
    [SerializeField] private BaseBaseActionScript[] testAction;

    public virtual void Show(Product product)
    {
        base.Show();
    }


    [MyButtonInt(nameof(TestDiamond), nameof(TestGold))] [SerializeField]
    private int abc123;
    [MyButtonInt(nameof(TestEnergy), nameof(TestStock))] [SerializeField]
    private int abc1231;
    public void TestDiamond(int test)
    {
        foreach (var baseBaseActionScript in testAction)
            baseBaseActionScript.StartAction();
        Show(new Product(Quantity.Diamond, test, 0,0,0));
    }

    public void TestGold(int test)
    {
        foreach (var baseBaseActionScript in testAction)
            baseBaseActionScript.StartAction();
        Show(new Product(Quantity.Gold, test, 0,0,0));
    }
    
    public void TestEnergy(int test)
    {
        foreach (var baseBaseActionScript in testAction)
            baseBaseActionScript.StartAction();
        Show(new Product(Quantity.Energy, test, 0,0,0));
    }

    public void TestStock(int test)
    {
        foreach (var baseBaseActionScript in testAction)
            baseBaseActionScript.StartAction();
        Show(new Product(Quantity.Stock, test, 0,0,0));
    }
}