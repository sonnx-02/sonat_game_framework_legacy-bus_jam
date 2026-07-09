using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ProductExchangeSelectItem : MonoBehaviour
{ 
    [SerializeField] private TradeData tradeData;
    [SerializeField] private ToggleScript[] onSelect;
    [SerializeField] private RewardView rewardView;
    [SerializeField] private Button button;

    public Button Btn => button;

    public Trade<Product, Product> GetTrade() => tradeData.GetTrade();

    private void Start()
    {
        rewardView.SetData(tradeData.price);
    }

    public void OnSelect(bool val)
    {
        onSelect.OnChanged(val);
    }
}