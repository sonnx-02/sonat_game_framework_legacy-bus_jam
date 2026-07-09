using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ShopStock
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int key;
    public StockType stockType;
    public int stock;
}


[Serializable]
public class ShopPrice : Product
{
    public ShopStock priceKey;
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int[] otherFills;
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int[] otherOnNext;
    
    
    public ShopPrice CloneCurrent()
    {
        return new ShopPrice()
        {
            quantity = quantity,
            index = index,
            amount = amount,
            parameter = parameter,
            parameter2 = parameter2,
            priceKey = new ShopStock
            {
                key = priceKey.key,stock = priceKey.stock,stockType = priceKey.stockType
            }
        };
    }
    
    public string GetPriceText()
    {
        if (quantity != Quantity.IapBuying)
            return amount.ToString();

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return "$" + RootView.rootView.ShopDatabase.storeProductDescriptors
                       .Find(x => x.key == priceKey.key)
                       .price.ToString(CultureInfo.InvariantCulture);
        }
        #endif

        return Kernel.Resolve<BasePurchaser>().GetPriceText(priceKey.key);
    }

    public Sprite GetImageSprite()
    {
        return Kernel.GetDatabase<GraphicAssetDatabase>().GetRewardImage(this);
    }

    public Vector2Int GetProgress()
    {
        int current = 0;
        switch (priceKey.stockType)
        {
            case StockType.Daily:
                current = BaseFrameworkView.DailyData.shopPriceStock[priceKey.key];
                break;
            case StockType.LifeTime:
                current = BaseFrameworkView.PlayerData.shopPriceStockLifeTime[priceKey.key];
                break;
            case StockType.Custom:
                throw new ArgumentOutOfRangeException();
            case StockType.Subscription:
                throw new ArgumentOutOfRangeException();
            case StockType.Weekly:
//                current = PlayerDataController.PlayerWeeklyData.ShopPriceStock[(int)KeyItem];
                break;
            case StockType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new Vector2Int(current, priceKey.stock);
    }

    public bool IsInStock()
    {
        switch (priceKey.stockType)
        {
            case StockType.None:
                return true;
            case StockType.Daily:
                return BaseFrameworkView.DailyData.shopPriceStock[priceKey.key] < priceKey.stock;
            case StockType.LifeTime:
                return BaseFrameworkView.PlayerData.shopPriceStockLifeTime[priceKey.key] < priceKey.stock;
            case StockType.Custom:
                throw new ArgumentOutOfRangeException();
            case StockType.Subscription:
                throw new ArgumentOutOfRangeException();
            case StockType.Weekly:
                return BaseFrameworkView.WeeklyData.shopPriceStock[priceKey.key] < priceKey.stock;
//               return PlayerDataController.PlayerWeeklyData.ShopPriceStock[(int) KeyItem] < ShopStock;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

   
}


[Serializable]
public enum OfferRuleEnum
{
    GameLogicDecide, // for game logic decide when it can be activated
    ActiveAfterTime, // when play game amount time
    PlayTimes, // active after play game a number of times
}

public enum CountDownTrim
{
    None,
    EndOfDay,
    EndOfWeek,
}


[Serializable]
public class Offer
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)] [SerializeField]
    public int key;
    [SerializeField] OfferRuleEnum ruleEnum;
    [ConditionalField(nameof(ruleEnum),new object[]{OfferRuleEnum.ActiveAfterTime})]
    [SerializeField] private float amountTime; // time for rule
    [ConditionalField(nameof(ruleEnum),new object[]{OfferRuleEnum.PlayTimes})]
    [SerializeField] private int playTimes; // play times
    [SerializeField] private int countDownOffer;
    [SerializeField] private CountDownTrim countDownTrim;
    [SerializeField] private Product[] bonuses;
    [SerializeField] public bool activeWhenScreenShow;
    [SerializeField] public float delayToShow;
    [SerializeField] public float displayRate = 1f;

    public bool RandomShowRate()
    {
        return UnityEngine.Random.Range(0, 100)  > (1 - displayRate)/100f;
    }

    private ShopItem ShopItem => Kernel.GetDatabase<ShopDatabase>().GetShopItem(key);
  
    // duong_comment_flow_offer_show2 : Check condition if offer ready to show
    public bool IsReadyToShowOnSession()
    {
        if (BaseFrameworkView.PlayerData.tutorialStep >= 0)
            return false;
        if (BaseFrameworkView.PlayerData.openTimes < Kernel.GetDatabase<GameSetting>().openTimesToOfferShow)
            return false;
        if (BaseFrameworkView.PlayerData.playTimes < Kernel.GetDatabase<GameSetting>().playTimesToOfferShow)
            return false;
        
        if (BaseFrameworkView.SessionData.offerChecker[key])
            return false;
        
        if (BaseFrameworkView.DailyData.offerChecker[key])
            return false;

        if (key == (int) BaseShopItemKey.NoAds)
            if (AdsManager.NoAds)
                return false;

        switch (ruleEnum)
        {
            case OfferRuleEnum.GameLogicDecide:
                return (RootView.rootView.gameController.IsOfferOkayToShow(key));
            case OfferRuleEnum.ActiveAfterTime:
                return Time.time > amountTime;
            case OfferRuleEnum.PlayTimes:
                return BaseFrameworkView.PlayerData.playTimes > playTimes;
            default:
                return false;
        }
    }
    
    // duong_comment_flow_offer_show4 : Check if offer is already show in day
    public bool IsReadyToShowOnDay()
    {
        return BaseFrameworkView.DailyData.shopItemShowTimes[key] == 0;
    }

    public IEnumerable<Product> GetProducts()
    {
        foreach (var product in ShopItem.GetProducts())
            yield return product;
        foreach (var product in bonuses)
            yield return product;
    }


    // activate to save data 
    public void ActiveOffer()
    {
        if(countDownOffer < 1)
            Debug.LogError("count down too small");
        var dateTime = DateTime.Now.AddSeconds(countDownOffer);
        switch (countDownTrim)
        {
            case CountDownTrim.None:
                break;
            case CountDownTrim.EndOfDay:
                dateTime = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day);
                break;
            case CountDownTrim.EndOfWeek:
                dateTime = DateTime.Today.EndOfWeek().AddDays(1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        

        if (!BaseFrameworkView.DailyData.offerChecker[key])
        {
            BaseFrameworkView.DailyData.offerChecker[key] = true;
            BaseFrameworkView.DailyData.Save();
            TimeTick[key] = dateTime.ToBinary();
            SubjectController.OfferActivated.OnNext(key);
        }
    }

    public bool IsOfferActive()
    {
        if (key == (int) BaseShopItemKey.NoAds && AdsManager.NoAds)
            return false;
        var shopItem = Kernel.GetDatabase<ShopDatabase>().GetShopItem(key);
        if (!shopItem.Price.IsInStock())
            return false;
        if (BaseFrameworkView.DailyData.offerBought[key])
            return false;
        return BaseFrameworkView.DailyData.offerChecker[key] && DateTime.Now < DateTime.FromBinary(TimeTick[key]);
    }

    public LongSerializableDictionary TimeTick
    {
        get
        {
//            switch (countDownType)
//            {
//                case AdsItemCountDownType.Unlimited:
            return BaseFrameworkView.PlayerData.offerTimeTick;
//                case AdsItemCountDownType.Daily:
//                    return BaseFrameworkView.DailyData.adsTimeTick;
//                case AdsItemCountDownType.WaveSession:
//                    return BaseFrameworkView.WaveData.adsTimeTick;
//                case AdsItemCountDownType.Weekly:
//                    return BaseFrameworkView.WeeklyData.adsTimeTick;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
        }
    }
}


[Serializable]
public class ShopItem
{
    public int Key => price.priceKey.key;
    [SerializeField] private ShopPrice price;
    [SerializeField] private Product[] products;
    [SerializeField] private Product[] adsProducts;
    [SerializeField] private Product[] extraProducts;

    public ShopItem()
    {
        
    }

    public ShopItem(ShopPrice price, Product product)
    {
        this.price = price.CloneCurrent();
        products = new[]{product};
        adsProducts = new[]{product};
    }

    public ShopItem Clone()
    {
        return new ShopItem()
        {
            price = price.CloneCurrent(),
            products = products.Select(x => x.Clone()).ToArray(),
            adsProducts = adsProducts.Select(x => x.Clone()).ToArray(),
            extraProducts = extraProducts.Select(x => x.Clone()).ToArray(),
        };
    }
    public Quantity OriginalPriceQuantity => price.quantity;
    public ShopPrice Price
    {
        get
        {
            if (price.quantity != Quantity.CustomProduct)
                return price;

            return  RootView.rootView.gameController.GetCustomProduct(price)
                .ToShopPrice(price.priceKey);
        }
    }

    public bool IsInStock()
    {
        return Price.IsInStock();
    }
    
    
    public Product Product // only for binding view
    {
        get
        {
            foreach (var product in products)
            {
                return product;
            }

            return null;
        }
    }


    public enum RandomType
    {
        None,
        RandomOneByLevelSeed
    }

    [SerializeField] private RandomType randomType = RandomType.None;
    [SerializeField] private int adsItemMultiple = 0;

    public RandomType RandomOneByLevelSeed => randomType;

    public bool IsRandomReward() => randomType != RandomType.None;
    public IEnumerable<Product> GetProducts()
    {
        switch (RandomOneByLevelSeed)
        {
            case RandomType.None:
                return products;
            case RandomType.RandomOneByLevelSeed:
                UnityEngine.Random.State originalRandomState = UnityEngine.Random.state;
                UnityEngine.Random.InitState(BaseFrameworkView.PlayerData.CurrentLevel);
                var pr = new[]
                {
                    products[UnityEngine.Random.Range(0,products.Length)]
                };
                UnityEngine.Random.state = originalRandomState;
                return pr;
            default:
                return products;
        }
    }

    public IEnumerable<Product> GetAdsProducts()
    {
        if (adsItemMultiple > 1)
            return GetProducts().Select(x => x.AlterAmount(x.amount * adsItemMultiple));
        
        return adsProducts.Length > 0 ? adsProducts : GetProducts();
    }

    public IEnumerable<Product> GetProductsWithExtra()
    {
        foreach (var product in products)
            yield return product;
        foreach (var product in extraProducts)
            yield return product;
    }

    public string description;
    public bool takeRandom;
    public bool ignoreSave;
    public string boosterName;

    public virtual void Spend()
    {
        BaseFrameworkView.PlayerData.Claim(price.ReverseQuantity());
        RootView.rootView.LogShopItem(Key, new List<ShopPrice>() {price});
    }


    public bool IsEnable()
    {
        switch (price.priceKey.stockType)
        {
            case StockType.None:
                break;
            case StockType.Daily:
                break;
            case StockType.LifeTime:
                break;
            case StockType.Custom:
                break;
            case StockType.Subscription:
                break;
            case StockType.Weekly:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }
}

[Serializable]
public class AdsItem
{
#if UNITY_EDITOR
 //   public string name;
#endif

    [IndexAsEnum(BuiltInEnumType.AdsItemKey)]
    public int key;

    public AdsItemCountDownType countDownType;
    public int countToGetReward = 1;

    [ConditionalField(nameof(countDownType), new object[]
        {AdsItemCountDownType.Daily, AdsItemCountDownType.WaveSession, AdsItemCountDownType.Weekly})]
    public int maxInDay = 1;

    public Product[] products;
    public bool takeRandom;

    public int adsTimeGap;

//    public int adsRewardGap;
    [IndexAsEnum(BuiltInEnumType.CustomGameCondition)]
    public int customCondition;

    public TradeLog adsLog;

    public string boosterName;

    // ex : watch 3 times to get reward
    public IntegerSerializableDictionary AdsRewardCounter
    {
        get
        {
            switch (countDownType)
            {
                case AdsItemCountDownType.Unlimited:
                    return BaseFrameworkView.PlayerData.adsRewardCounter;
                case AdsItemCountDownType.Daily:
                    return BaseFrameworkView.DailyData.adsRewardCounter;
                case AdsItemCountDownType.WaveSession:
                    return BaseFrameworkView.WaveData.adsRewardCounter;
                case AdsItemCountDownType.Weekly:
                    return BaseFrameworkView.WeeklyData.adsRewardCounter;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public IntegerSerializableDictionary AdsRewardTaken
    {
        get
        {
            switch (countDownType)
            {
                case AdsItemCountDownType.Unlimited:
                    return BaseFrameworkView.PlayerData.adsRewardTaken;
                case AdsItemCountDownType.Daily:
                    return BaseFrameworkView.DailyData.adsRewardTaken;
                case AdsItemCountDownType.WaveSession:
                    return BaseFrameworkView.WaveData.adsRewardTaken;
                case AdsItemCountDownType.Weekly:
                    return BaseFrameworkView.WeeklyData.adsRewardTaken;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public LongSerializableDictionary AdsTimeTick
    {
        get
        {
            switch (countDownType)
            {
                case AdsItemCountDownType.Unlimited:
                    return BaseFrameworkView.PlayerData.adsTimeTick;
                case AdsItemCountDownType.Daily:
                    return BaseFrameworkView.DailyData.adsTimeTick;
                case AdsItemCountDownType.WaveSession:
                    return BaseFrameworkView.WaveData.adsTimeTick;
                case AdsItemCountDownType.Weekly:
                    return BaseFrameworkView.WeeklyData.adsTimeTick;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public IEnumerable<Product> Multiple(int multiple)
    {
        if (!takeRandom)
            foreach (var product in products)
            {
                var convert = product.ConvertRandom().Clone();
                convert.amount *= multiple;
                yield return convert;
            }
        else
        {
            var product = TakeByWeight(products);
            var convert = product.Clone();
            convert.amount *= multiple;
            yield return convert;
        }

        Product TakeByWeight(IList<Product> arr)
        {
            if (arr.Count == 1)
                return arr[0];

            int totalWeight = 0;
            int[] weightValues = new int[arr.Count];
            for (var i = 0; i < arr.Count; i++)
            {
                totalWeight += arr[i].parameter;
                weightValues[i] = totalWeight;
            }

            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            for (int i = 0; i < weightValues.Length; i++)
                if (randomValue < weightValues[i])
                    return arr[i];

            Debug.LogError("Error:GetByWeight");
            return arr[-1];
        }
    }

//    public IEnumerable<ClaimedProduct> TakeReward(int multiple)
//    {
//        if (!takeRandom)
//            foreach (var product in products)
//            {
//                var convert = product.ConvertRandom().Clone();
//                convert.amount *= multiple;
//                var data = PlayerData.Claim(convert);
//                if (data != null)
//                    yield return data;
//            }
//        else
//        {
//            var product = TakeByWeight(products);
//            var convert = product.Clone();
//            convert.amount *= multiple;
//            var data = PlayerData.Claim(convert.ConvertRandom());
//            if (data != null)
//                yield return data;
//        }
//
//        Product TakeByWeight(IList<Product> arr)
//        {
//            if (arr.Count == 1)
//                return arr[0];
//
//            int totalWeight = 0;
//            int[] weightValues = new int[arr.Count];
//            for (var i = 0; i < arr.Count; i++)
//            {
//                totalWeight += arr[i].parameter;
//                weightValues[i] = totalWeight;
//            }
//
//            int randomValue = UnityEngine.Random.Range(0, totalWeight);
//
//            for (int i = 0; i < weightValues.Length; i++)
//                if (randomValue < weightValues[i])
//                    return arr[i];
//
//            Debug.LogError("Error:GetByWeight");
//            return arr[-1];
//        }
//    }
    
    public virtual AdsItemState GetState()
    {
        if (!RootView.rootView.gameController.GetCustomCondition(customCondition))
            return AdsItemState.ConditionNotValid;
        if (countDownType != AdsItemCountDownType.Unlimited && AdsRewardTaken[key] == maxInDay)
            return AdsItemState.Complete;
        if (AdsRewardCounter[key] >= Mathf.Max(countToGetReward,1)) // cannot be zero
            return AdsItemState.Receivable;
        if (DateTime.Now >= DateTime.FromBinary(AdsTimeTick[key]))
            return AdsItemState.WatchAds;

        return AdsItemState.Wait;
    }

    public bool IsInStock()
    {
        return !(countDownType != AdsItemCountDownType.Unlimited && AdsRewardTaken[(int) key] == maxInDay);
    }
}


[Serializable]
public class SubscriptionItem
{
    [IndexAsEnum(BuiltInEnumType.SubscriptionKey)]
    public int key;
    public string storeProductId;
    public int activeDays;
    public bool autoTakeReward;
    public Product[] rewards;
//    public TradeLog Log => useAmountFromReward ?  log.Clone(rewards[0].amount) : log;
}