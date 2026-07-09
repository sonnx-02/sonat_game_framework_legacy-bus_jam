using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyHomeShowPackOnStart : BasePopupDependency
{
    [IndexAsEnum(BuiltInEnumType.ScreenEnum)] [SerializeField]
    private int key = (int) ScreenEnum.Home;
    
    [SerializeField] private bool loopLastValue;

    [SerializeField] private PlayerPrefRemoteArrayInt level_show_no_ads_pack = new PlayerPrefRemoteArrayInt(
        RemoteConfigKey.level_show_no_ads_pack, new[]
        {
            1, 5, 10, 15, 30
        });

    [SerializeField]
    private PlayerPrefRemoteBool show_pack_on_start = new PlayerPrefRemoteBool("show_pack_on_start", false);
    [SerializeField]
    private PlayerPrefRemoteBool prior_no_ads_pack = new PlayerPrefRemoteBool("prior_no_ads_pack", false);
    [SerializeField]
    private PlayerPrefRemoteInt start_level_show_pack_home = new PlayerPrefRemoteInt("start_level_show_pack_home",5);

    public override void OnShow()
    {
        base.OnShow();
    }

    private readonly Dictionary<int, int> _keyLastLevelShow = new Dictionary<int, int>();

    public override bool ReadyToShow()
    {
        var screen = ScreenRoot.Resolve(key);
        if (screen.ShowTimes > 1) // show when back to home
        {
            if (DuongExtensions.IsListContains(level_show_no_ads_pack.Value,LevelInt,loopLastValue) && CheckShowPack(0))
                return true;
        }
        else // show when first launch
        {
            if (show_pack_on_start.Value && LevelInt >= start_level_show_pack_home.Value)
            {
                if (prior_no_ads_pack.Value
                    && CheckShowPack(0))
                    return true;

                var packPopup = ScreenRoot.popupManager.Resolve<PopupPack>();
                var packs = packPopup.Items;
                packs.Shuffle();

                foreach (var popupPackItem in packs)
                    if (CheckShowPack(popupPackItem.Key()))
                        return true;
            }
        }

        return base.ReadyToShow();
    }

    private bool CheckShowPack(int shopItemKey)
    {
        var packPopup = ScreenRoot.popupManager.Resolve<PopupPack>();
        if (packPopup.Exist(shopItemKey) && (shopItemKey != 0 || !AdsManager.NoAds)) // popup no ads
        {
            var popupPackItem = packPopup.SetPack(shopItemKey);
            var shopItem = RootView.rootView.ShopDatabase.GetShopItem(shopItemKey);
            if (shopItem.Price.IsInStock())
            {
                if (!_keyLastLevelShow.ContainsKey(shopItemKey) || _keyLastLevelShow[shopItemKey] != LevelInt)
                {
                    _keyLastLevelShow[shopItemKey] = LevelInt;
                    return true;
                }
            }
        }

        return false;
    }
}