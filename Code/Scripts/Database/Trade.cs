using System;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;


[Serializable]
public struct TradeData
{
    public Product price;
    public Product @return;

    public Trade<Product, Product> GetTrade()
    {
        return new Trade<Product, Product>(price, @return);
    }
}

[Serializable]
public class EarningRewardSetting
{
    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    public int rewardTypeFlag = (int) RewardTypeFlag.ClaimInstantInPopup;
    public bool closeCurrentPopupOnRewarded;
}

[Serializable]
public class TradeLogBase
{
}

[Serializable]
public class TradeLog : TradeLogBase
{
    public static TradeLog Empty => new TradeLog();

    public TradeLog Clone()
    {
        return new TradeLog()
        {
        };
    }
}


public class Trade<T, T2> where T : Product where T2 : Product
{
    private readonly IEnumerable<T> _prices;
    private IEnumerable<T2> _products;
    private readonly TradeLog _tradeLog;
    private string _boosterName;

    private bool _isMultiReward;

    public bool IsMultiReward => _isMultiReward;

    public IEnumerable<T> Prices => _prices;
    public IEnumerable<T2> Products => _products;
    public TradeLog TradeLog => _tradeLog;
    public string BoosterName => _boosterName;

    public T2 Product => Products.FirstOrDefault();
    public T Price => Prices.FirstOrDefault();
    public bool IsAffordable => _notEnough == null;

    private IList<T> _notEnough;
    public IList<T> NotEnough => _notEnough;


    private void Calculate()
    {
        var playerData = BaseFrameworkView.PlayerData;
        _notEnough = null;
        if (!_prices.MyAll(price => playerData.IsAffordable(price)))
        {
            _notEnough = new List<T>();
            foreach (var price in _prices)
                if (!playerData.IsAffordable(price))
                    NotEnough.Add(price);
        }
    }

    private bool _logged;

    private void CheckLog()
    {
        if (!_logged)
            RootView.rootView.gameController.CheckLogVirtualCurrency(this);
        _logged = true;
    }

    
    
    public bool Spend()
    {
        if (IsAffordable)
        {
            var playerData = BaseFrameworkView.PlayerData;
            foreach (var price in _prices)
                playerData.Claim(price.ReverseQuantity());
            CheckLog();
            return true;
        }

        CheckLog();
        Debug.LogError("It's not affordable to spendOn");
        return false;
    }

    public Trade(IEnumerable<T> prices, IEnumerable<T2> products, TradeLog tradeLog, bool claimed = false,
        bool spent = false)
    {
        _prices = prices;
        var enumerable = products as T2[] ?? products.ToArray();
        _products = enumerable;
        Calculate();
        _tradeLog = tradeLog;
        _isMultiReward = enumerable.Length > 1;
    }

    public Trade(IEnumerable<T> prices, T2 product)
    {
        _prices = prices;
        _products = new[] {product};
        Calculate();
    }
    
    public Trade(T prices, IEnumerable<T2> product)
    {
        _prices = new[]{ prices};
        _products = product;
        Calculate();
        _isMultiReward = product.ToArray().Length > 1;
    }


    public Trade(T price, T2 product,bool noCalculate = false)
    {
        _prices = new[] {price};
        _products = new[] {product};
        if(!noCalculate)
            Calculate();
    }


    public Trade(T price, IEnumerable<T2> product, TradeLog tradeLog)
    {
        _prices = new[] {price};
        _products = product;
        Calculate();
        _tradeLog = tradeLog;
        _isMultiReward = product.ToArray().Length > 1;
    }
    
    public void OverrideProduct(IEnumerable<T2> val)
    {
        _products = val;
    }

    public void SetBoosterName(string shopItemBoosterName)
    {
        _boosterName = shopItemBoosterName;
    }
}