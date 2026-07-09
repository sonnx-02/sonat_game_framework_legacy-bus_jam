using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class BaseReactiveData
{
    protected virtual int CustomListSize => 10;
    protected virtual int CustomListSize2 => 5;


    public IntReactiveProperty[] customPropertyList;
    public ArrayIntReactiveProperty[] customArrayIntProperty;

    public IntReactiveProperty GetCustomProperty(int customPropertyEnum)
    {
        return customPropertyList[customPropertyEnum];
    }

    public ArrayIntReactiveProperty GetCustomArrayIntProperty(int customPropertyEnum)
    {
        return customArrayIntProperty[customPropertyEnum];
    }


    public IntReactiveProperty GetPropertyByName(string propertyName)
    {
        var getProperty = typeof(ReactivePlayerData).GetField(propertyName);
        return (IntReactiveProperty) getProperty.GetValue(this);
    }

    public FloatReactiveProperty GetFloatPropertyByName(string propertyName)
    {
        var getProperty = typeof(ReactivePlayerData).GetField(propertyName);
        return (FloatReactiveProperty) getProperty.GetValue(this);
    }

    public virtual void Clear()
    {
        if (customPropertyList == null)
            InitCustomPropertyList();
        else
            for (var i = 0; i < customPropertyList.Length; i++)
                customPropertyList[i].Value = 0;

        if (customArrayIntProperty == null)
            InitCustomArrayIntPropertyList();
        else
            for (var i = 0; i < customArrayIntProperty.Length; i++)
                customArrayIntProperty[i].Value = null;
    }

    protected virtual void InitCustomPropertyList()
    {
        if (customPropertyList == null) NewCustomPropertyList();
    }

    protected virtual void InitCustomArrayIntPropertyList()
    {
        if (customArrayIntProperty == null) NewCustomArrayIntPropertyList();
    }

    protected virtual void NewCustomPropertyList()
    {
        customPropertyList = new IntReactiveProperty[CustomListSize];
        for (var i = 0; i < customPropertyList.Length; i++)
            customPropertyList[i] = new IntReactiveProperty(0);
    }

    protected virtual void NewCustomArrayIntPropertyList()
    {
        customArrayIntProperty = new ArrayIntReactiveProperty[CustomListSize2];
        for (var i = 0; i < customArrayIntProperty.Length; i++)
            customArrayIntProperty[i] = new ArrayIntReactiveProperty(null);
    }
}

[Serializable]
public class BaseLoadReactiveData : BaseReactiveData
{
    public static bool IgnoreSave;

    public virtual void Validate()
    {
        InitCustomPropertyList();
    }
}

//[Serializable]
//public class BaseBySessionReactiveData : BaseReactiveData
//{
//    public override void Clear()
//    {
//        base.Clear();
//        InitCustomPropertyList();
//    }
//}

public static class DuongDataHelper
{
    public static void JsonPreSavePlayerDataSpendClaim<T>(IEnumerable<T> spends, IEnumerable<T> claims)
        where T : Product
    {
        try
        {
            var current = BaseFrameworkView.PlayerData;
            ReactivePlayerData clone = JsonUtility.FromJson<ReactivePlayerData>(JsonUtility.ToJson(current));
            clone.Validate();
            foreach (var price in spends)
                if (price.quantity != Quantity.CustomDailyProperty)
                    clone.Claim(price.ReverseQuantity());
            foreach (var price in claims)
                if (price.quantity != Quantity.CustomDailyProperty && price.quantity != Quantity.Stock)
                    clone.Claim(price, true, true);
            clone.Save();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }
}

[Serializable]
public class PlayerDataEquipData
{
    public List<int> equipped = new List<int>();
    public List<int> availables = new List<int>();
    public bool CheckEquipped(int index) => equipped.Contains(index);
    public bool CheckAvailable(int index) => availables.Contains(index);
    public int EnumKey { get; set; }

    public int GetCurrent()
    {
        if (equipped.Count == 0)
            return 0;
        return equipped[0];
    }

    public void Add(int index)
    {
        availables.AddDistinct(index);
    }

    public int GetSwitchValue(int defaultVal = -1)
    {
        if (equipped.Count == 0)
            return defaultVal;
        return equipped[0];
    }

    public bool ToggleEquipped(int index)
    {
        if (!availables.Contains(index))
            return false;
        if (equipped.Contains(index))
            equipped.Remove(index);
        else
            equipped.Add(index);
        SubjectController.OnEquipDataUpdate.OnNext(new EquipDataUpdate(EnumKey, index, EquipDataUpdateType.Switch));
        return true;
    }

    public bool SetEquipped(int index, bool value, bool fill = false)
    {
        if (fill && !availables.Contains(index))
            availables.Add(index);
        if (!availables.Contains(index))
            return false;

        if (value && !equipped.Contains(index))
        {
            equipped.Add(index);
            if (!availables.Contains(index))
                availables.Add(index);
        }

        if (!value && equipped.Contains(index))
            equipped.Remove(index);

        return true;
    }

    public void Switch(int index)
    {
        equipped.Clear();
        equipped.Add(index);
        SubjectController.OnEquipDataUpdate.OnNext(new EquipDataUpdate(EnumKey, index, EquipDataUpdateType.Switch));
    }
}

[Serializable]
public class ReactivePlayerData : BaseLoadReactiveData
{
    public int CurrentLevel => currentLevel.Value;
    public string DisplayCurrentLevel => DisplayCurrentLevelInt.ToString();
    public int DisplayCurrentLevelInt => (currentLevel.Value + 1);

    public IntReactiveProperty currentLevel;
    public IntReactiveProperty gold;
    public IntReactiveProperty gem;
    public IntReactiveProperty medal;
    public IntReactiveProperty spin;
    public IntReactiveProperty energy;
    public IntReactiveProperty time;
    public IntReactiveProperty bestScore;
    public IntReactiveProperty isMusicEnable;
    public IntReactiveProperty isSoundEnable;
    public IntReactiveProperty isVibrationEnable;
    public int dailyGiftProgress;
    public bool dailyGiftChecked;
    public bool firstTime;
    public bool won;
    public bool played;
    public bool noAdsForever;
    public bool noAds24h;

    // for inventory like upgrade 
    public int tutorialStep;
    public int playTimes;
    public int openTimes;
    public int days;

    // Booster
    public IntReactiveProperty bUndo;
    public IntReactiveProperty bMore3Slot;
    public IntReactiveProperty bGoStraight;

    public bool[] alreadyGetFreeBoosters = new bool[3];

    public IntegerSerializableDictionary logAllTime = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary logEachGame = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary items = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary adsRewardCounter = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary adsRewardTaken = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary shopPriceStockLifeTime = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary achievementData = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary achievementChecker = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary subscription = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary levelProgress = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary levelPlayTimes = new IntegerSerializableDictionary();
    public LongSerializableDictionary adsTimeTick = new LongSerializableDictionary();
    public LongSerializableDictionary offerTimeTick = new LongSerializableDictionary();
    public PlayerDataEquipData[] equipData = new PlayerDataEquipData[5];
    public IntegerSerializableDictionary shopItemShowTimes;
    //  public PlayGameInfo playGameInfo = new PlayGameInfo();
    public List<int> navigationChecker = new List<int>();
    public long[] customTimeTick = new long[10];
    [NonSerialized] public DateTime[] CustomDateTime;
    public IntegerSerializableDictionary iapBought = new IntegerSerializableDictionary();
    private static string FilePath => $"{Application.persistentDataPath}/{nameof(ReactivePlayerData)}.json";


    public override void Validate()
    {
        base.Validate();
        //   if (playGameInfo == null) playGameInfo = new PlayGameInfo();
        if (currentLevel == null) currentLevel = new IntReactiveProperty(0);
        if (time == null) time = new IntReactiveProperty();
        if (gem == null) gem = new IntReactiveProperty(0);
        if (bestScore == null) bestScore = new IntReactiveProperty(0);
        if (gem == null) gem = new IntReactiveProperty(0);
        if (medal == null) medal = new IntReactiveProperty(0);
        if (energy == null) energy = new IntReactiveProperty(0);
        if (spin == null) spin = new IntReactiveProperty(0);
        if (gold == null) gold = new IntReactiveProperty(0);
        if (isMusicEnable == null) isMusicEnable = new IntReactiveProperty(0);
        if (isSoundEnable == null) isSoundEnable = new IntReactiveProperty(1);
        if (isVibrationEnable == null) isVibrationEnable = new IntReactiveProperty(1);
        if (items == null) items = new IntegerSerializableDictionary();
        if (logEachGame == null) logEachGame = new IntegerSerializableDictionary();
        if (logAllTime == null) logAllTime = new IntegerSerializableDictionary();
        if (iapBought == null) iapBought = new IntegerSerializableDictionary();
        if (adsRewardCounter == null) adsRewardCounter = new IntegerSerializableDictionary();
        if (adsRewardTaken == null) adsRewardTaken = new IntegerSerializableDictionary();
        if (achievementData == null) achievementData = new IntegerSerializableDictionary();
        if (shopPriceStockLifeTime == null) shopPriceStockLifeTime = new IntegerSerializableDictionary();
        if (achievementData == null) achievementData = new IntegerSerializableDictionary();
        if (achievementChecker == null) achievementChecker = new IntegerSerializableDictionary();
        if (subscription == null) subscription = new IntegerSerializableDictionary();
        if (levelProgress == null) levelProgress = new IntegerSerializableDictionary();
        if (levelPlayTimes == null) levelPlayTimes = new IntegerSerializableDictionary();
        if (adsTimeTick == null) adsTimeTick = new LongSerializableDictionary();
        if (customTimeTick == null) customTimeTick = new long[10];
        if (offerTimeTick == null) offerTimeTick = new LongSerializableDictionary();
        if (navigationChecker == null) navigationChecker = new List<int>();
        if (shopItemShowTimes == null) shopItemShowTimes = new IntegerSerializableDictionary();

        if (bUndo == null) bUndo = new IntReactiveProperty();
        if (bMore3Slot == null) bMore3Slot = new IntReactiveProperty();
        if (bGoStraight == null) bGoStraight = new IntReactiveProperty();

        if (equipData == null)
            equipData = new[]
            {
                new PlayerDataEquipData(),
                new PlayerDataEquipData(),
                new PlayerDataEquipData(),
                new PlayerDataEquipData(),
                new PlayerDataEquipData(),
            };
        for (var i = 0; i < equipData.Length; i++)
            if (equipData[i] == null)
                equipData[i] = new PlayerDataEquipData();

        for (var i = 0; i < equipData.Length; i++)
        {
            equipData[i].EnumKey = i;
        }

        CustomDateTime = customTimeTick.Select(DateTime.FromBinary).ToArray();
//        if (epochDate != DuongExtensions.GetEpochDate() || Kernel.GetDatabase<GameSetting>().alwaysIsNewDayTestEditor)
//        {
//            epochDate = DuongExtensions.GetEpochDate();
//            if (dailyGiftChecked)
//            {
//                dailyGiftProgress++;
//                dailyGiftChecked = false;
//            }
//        }

        if (tutorialStep >= 0)
            tutorialStep = 0;
    }

    public void OnNewDay()
    {
        days++;
        if (dailyGiftChecked)
        {
            dailyGiftProgress++;
            dailyGiftChecked = false;
        }

        for (var i = 0; i < subscription.items.Count; i++)
            subscription.items[i]--;
    }


    private static bool _ignoreSave;

    public void AddCustomTimeTickFromNow(int key, int seconds, bool save = true)
    {
        var now = DateTime.Now;
        var next = now.Add(TimeSpan.FromSeconds(seconds));
        customTimeTick[key] = next.Ticks;
        CustomDateTime[key] = DateTime.FromBinary(customTimeTick[key]);
        if (save)
            Save();
    }

    public void Save()
    {
        if (_ignoreSave)
            return;

        if (!IgnoreSave)
        {
            Debug.Log("Save" + nameof(ReactivePlayerData));
            File.WriteAllText(FilePath, JsonUtility.ToJson(this));
        }
    }

    public void IgnoreSaveUntilTurnOn()
    {
#if UNITY_EDITOR
     //   Debug.LogError(nameof(IgnoreSaveUntilTurnOn));
#endif
        _ignoreSave = true;
    }

    public void CheckAndTurnOnSave()
    {
        if (_ignoreSave)
        {
            // Debug.LogError("CheckAndTurnOnSave");
            _ignoreSave = false;
            Save();
        }
    }


    public static ReactivePlayerData Load()
    {
//        if (PlayerPrefs.GetInt("Delete"+nameof(ReactivePlayerData)) == 0)
//        {
//            PlayerPrefs.SetInt("Delete"+nameof(ReactivePlayerData), 1);
//            DeleteFile();
//            var temp = new ReactivePlayerData();
//            temp.Validate();
//            return temp;
//        }
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);

            if (string.IsNullOrEmpty(json))
                return NewData();

            try
            {
                ReactivePlayerData data = JsonUtility.FromJson<ReactivePlayerData>(json);
                if (data != null)
                {
                    data.Validate();
                    return data;
                }
            }
            catch (Exception e)
            {
                return NewData();
            }
        }

        return NewData();
    }

    private static ReactivePlayerData NewData()
    {
        var newData = new ReactivePlayerData();
        newData.Validate();
        return newData;
    }


    public static void DeleteFile()
    {
        File.Delete(FilePath);
    }

    public void ClaimGold(int amount)
    {
        Product goldPack = new Product();
        goldPack.Amount = amount;
        goldPack.quantity = Quantity.Gold;

        Claim(goldPack);
    }

    public ClaimedProduct Claim(Product reward, bool logAchievement = true, bool isPreSave = false)
    {
        var claimedReward = reward.ConvertRandom();
        switch (reward.quantity)
        {
            case Quantity.Gold:
                if (claimedReward.amount < 0 && gold.Value < Mathf.Abs(claimedReward.amount))
                {
                    break;
                }

                gold.Value += claimedReward.amount;

                if (claimedReward.amount < 0)
                {
                    if (logAchievement)
                    {
                        if (!isPreSave)
                            BaseFrameworkView.FillQuest((int) BaseQuestEnum.SpendGold, Mathf.Abs(claimedReward.amount));
                    }
                }

                if (gold.Value < 0)
                {
                    gold.Value = 0;
                }
                break;
            case Quantity.Diamond:
                if (claimedReward.amount < 0 && gem.Value < Mathf.Abs(claimedReward.amount))
                {
                    break;
                }

                gem.Value += claimedReward.amount;

                if (claimedReward.amount < 0)
                {
                    if (logAchievement)
                    {
                        if (!isPreSave)
                            BaseFrameworkView.FillQuest((int) BaseQuestEnum.SpendGem, Mathf.Abs(claimedReward.amount));
                    }
                }

                if (gem.Value < 0)
                {
                    gem.Value = 0;
                }
                break;
            case Quantity.Medal:
                medal.Value += claimedReward.amount;
                break;
            case Quantity.None:
                break;
            case Quantity.Energy:
                if (claimedReward.amount > 0)
                    energy.Value += claimedReward.amount;
                else if (!IsEnergyUnlimitedTime())
                    energy.Value += claimedReward.amount;
                break;
            case Quantity.Spin:
                spin.Value += claimedReward.amount;
                break;
            case Quantity.WatchAds:
                return null;
            case Quantity.IapBuying:
                return null;
            case Quantity.Subscription:
                var subscriptionItem = RootView.rootView.ShopDatabase.GetSubscription(claimedReward.index);
                if (subscriptionItem.activeDays <= 0)
                    Debug.LogError("active days cannot be zero", RootView.rootView.ShopDatabase);
                subscription[claimedReward.index] = subscriptionItem.activeDays;
                if (!isPreSave)
                    SubjectController.SubscriptionActivated.OnNext(claimedReward.index);
                return null;
            case Quantity.Free:
                return null;
            case Quantity.CustomQuantity:
                return RootView.rootView.gameController.HandlerClaimCustomQuantity(claimedReward);
            case Quantity.SystemAction:
                if (!isPreSave)
                    SubjectController.ProductSystemActionEvent.OnNext(ProductAction.FromProduct(claimedReward));
                return null;
            case Quantity.GameAction:
                if (!isPreSave)
                    SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(claimedReward));
                return null;
            case Quantity.Item:
                items[claimedReward.index] += claimedReward.amount;
                break;
            case Quantity.CustomProperty:
                customPropertyList[claimedReward.index].Value += claimedReward.amount;
                break;
            case Quantity.TutorialSkip:
                if (claimedReward.index == 0)
                    tutorialStep += claimedReward.amount;
                else
                {
                    if (!isPreSave)
                        RootView.rootView.gameController.SkipTutorial(claimedReward.index, claimedReward.amount);
                }

                break;
            case Quantity.SystemGameAction:
                if (!isPreSave)
                    SubjectController.GameActionEvent.OnNext((GameActionEvent) claimedReward.index);
                break;
            case Quantity.Achievement:
                achievementChecker[claimedReward.index] += claimedReward.amount;
                break;
            case Quantity.Quest:
                if (!isPreSave)
                    BaseFrameworkView.DailyData.dailyQuestChecker[claimedReward.index] = true;
                break;
            case Quantity.CustomProduct:
                break;
            case Quantity.CustomDailyProperty:
                BaseFrameworkView.DailyData.customPropertyList[claimedReward.index].Value += claimedReward.amount;
                break;
            case Quantity.CustomTimeTick:
                //No ads 24h
                if (claimedReward.index == 3 && noAds24h)
                {
                    break;
                }

                if (DateTime.FromBinary(customTimeTick[claimedReward.index]) < DateTime.Now)
                    customTimeTick[claimedReward.index] =
                        DateTime.Now.Add(TimeSpan.FromSeconds(claimedReward.amount)).Ticks;
                else
                    customTimeTick[claimedReward.index] = DateTime.FromBinary(customTimeTick[claimedReward.index])
                        .Add(TimeSpan.FromSeconds(claimedReward.amount)).Ticks;
                if (CustomDateTime != null) // for clone not execute Validate
                    CustomDateTime[claimedReward.index] = DateTime.FromBinary(customTimeTick[claimedReward.index]);
                break;
            case Quantity.EquipData:
                bool add = BaseFrameworkView.PlayerData.equipData[claimedReward.index].availables
                    .AddDistinct(claimedReward.parameter);
                if (!isPreSave && add)
                    SubjectController.OnEquipDataUpdate.OnNext(new EquipDataUpdate(claimedReward.index,
                        claimedReward.parameter, EquipDataUpdateType.NewlyBought));
                break;
            case Quantity.Random:
                break;
            case Quantity.EventItem:
                RootView.rootView.gameController.ClaimEventItem(claimedReward.parameter, claimedReward.index,
                    claimedReward.amount);
                break;
            case Quantity.Stock:
                RootView.rootView.gameController.AddStock(claimedReward.index, claimedReward.amount);
                break;
            default:
                Debug.LogError("Not define claim for this reward" + claimedReward.quantity);
                break;
        }

        return claimedReward;
    }

    public bool IsInRange(int timeTickKey)
    {
        return CustomDateTime[timeTickKey] > DateTime.Now;
    }

    public List<ClaimedProduct> Claim<T>(IEnumerable<T> rewards) where T : Product
    {
        List<ClaimedProduct> claimed = new List<ClaimedProduct>();

        foreach (var product in rewards)
        {
            var claim = Claim(product);
            if (claim != null)
                claimed.Add(claim);
        }

        return claimed;
    }

//    private DialogController DialogController => RootView.rootView.screenRoot.dialogController;
    private ScreenRoot ScreenRoot => RootView.rootView.screenRoot;

    public void FillShopPriceStock(ShopPrice shopPrice, int amount = 1)
    {
        switch (shopPrice.priceKey.stockType)
        {
            case StockType.None:
                break;
            case StockType.Daily:
                foreach (var shopPriceOtherFill in shopPrice.otherFills)
                    BaseFrameworkView.DailyData.shopPriceStock[shopPriceOtherFill] += amount;
                BaseFrameworkView.DailyData.shopPriceStock[shopPrice.priceKey.key] += amount;
                SubjectController.ShopPriceStockChangedEvent.OnNext(true);
                break;
            case StockType.LifeTime:
                foreach (var shopPriceOtherFill in shopPrice.otherFills)
                    BaseFrameworkView.PlayerData.shopPriceStockLifeTime[shopPriceOtherFill] += amount;
                BaseFrameworkView.PlayerData.shopPriceStockLifeTime[shopPrice.priceKey.key] += amount;
                SubjectController.ShopPriceStockChangedEvent.OnNext(true);
                break;
            case StockType.Custom:
                break;
            case StockType.Weekly:
                foreach (var shopPriceOtherFill in shopPrice.otherFills)
                    BaseFrameworkView.WeeklyData.shopPriceStock[shopPriceOtherFill] += amount;
                BaseFrameworkView.WeeklyData.shopPriceStock[shopPrice.priceKey.key] += amount;
                SubjectController.ShopPriceStockChangedEvent.OnNext(true);
                break;
            case StockType.Subscription:
                throw new ArgumentOutOfRangeException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsEnergyUnlimitedTime()
    {
        return IsInRange((int) BaseCustomTimeTick.CountDownUnlimitedLives);
    }

    public bool IsDoubleRewardTime()
    {
        return IsInRange((int) BaseCustomTimeTick.CountDownDoubleReward);
    }

    public bool IsAffordable(Product price)
    {
        switch (price.quantity)
        {
            case Quantity.Gold:
                return gold.Value >= price.amount;
            case Quantity.Diamond:
                return gem.Value >= price.amount;
            case Quantity.Spin:
                return spin.Value >= price.amount;
            case Quantity.Energy:
                return energy.Value >= price.amount || IsEnergyUnlimitedTime();
            case Quantity.WatchAds:
                return true;
            case Quantity.Medal:
                return medal.Value >= price.amount;
            case Quantity.None:
                return false;
            case Quantity.IapBuying:
                return true;
            case Quantity.Subscription:
                return false;
            case Quantity.Free:
                return true;
            case Quantity.SystemAction:
                return true;
            case Quantity.GameAction:
                return true;
            case Quantity.Item:
                return items[price.index] >= price.amount;
            case Quantity.CustomProperty:
                return customPropertyList[price.index].Value >= price.amount;
            case Quantity.TutorialSkip:
                return true;
            case Quantity.Achievement:
                return true;
            case Quantity.Quest:
                return true;
            case Quantity.SystemGameAction:
                return true;
            case Quantity.CustomProduct:
                return true;
            case Quantity.CustomDailyProperty:
                return BaseFrameworkView.PlayerData.customPropertyList[price.index].Value >= price.amount;
            case Quantity.CustomQuantity:
                return true;
            default:
                throw new ArgumentOutOfRangeException(price.quantity.ToString(), price.quantity, null);
        }
    }

    public bool CanUseItem(int key)
    {
        switch (key)
        {
            case GameDefine.BoosterUndoKey:
                return bUndo.Value > 0;
            case GameDefine.BoosterMore3SlotKey:
                return bMore3Slot.Value > 0;
            case GameDefine.BoosterGoStraightKey:
                return bGoStraight.Value > 0;
            default:
                return false;
        }
    }

    public void UseItem(int key)
    {
        switch (key)
        {
            case GameDefine.BoosterUndoKey:
                bUndo.Value--;
                break;
            case GameDefine.BoosterMore3SlotKey:
                bMore3Slot.Value--;
                break;
            case GameDefine.BoosterGoStraightKey:
                bGoStraight.Value--;
                break;
            default:
                break;
        }

        Save();
    }

    public void AddBoosterToInventory(int key, int num)
    {
        switch (key)
        {
            //Undo
            case GameDefine.BoosterUndoKey:
                bUndo.Value += num;
                break;
            //More3Slot
            case GameDefine.BoosterMore3SlotKey:
                bMore3Slot.Value += num;
                break;
            //GoStraight
            case GameDefine.BoosterGoStraightKey:
                bGoStraight.Value += num;
                break;
        }
    }
}

//
//[SerializeField]
//public class PlayGameInfo
//{
//    public List<int> shot_digit_count_after_ad_name = new List<int>();
//    public List<int> shot_digit_count_after_ad_value = new List<int>();
//    public List<int> view_ad_count_name = new List<int>();
//    public List<int> view_ad_count_value = new List<int>();
//
//    public void AddShotByAds()
//    {
//        int numberOfAds = BaseFrameworkView.PlayerData.logEachGame[(int) LogEachGameEnum.video_count];
//        for (var i = 0; i < shot_digit_count_after_ad_name.Count; i++)
//        {
//            if (shot_digit_count_after_ad_name[i] == numberOfAds)
//            {
//                shot_digit_count_after_ad_value[i] = shot_digit_count_after_ad_value[i] + 1;
//                return;
//            }
//        }
//
//        Debug.Log(numberOfAds);
//        shot_digit_count_after_ad_name.Add(numberOfAds);
//        shot_digit_count_after_ad_value.Add(1);
//    }
//
//    public void AddViewAdsByGem(int gem)
//    {
//        for (var i = 0; i < view_ad_count_name.Count; i++)
//            if (view_ad_count_name[i] == gem)
//            {
//                view_ad_count_value[i] = view_ad_count_value[i] + 1;
//                return;
//            }
//
//        view_ad_count_name.Add(gem);
//        view_ad_count_value.Add(1);
//    }
//}