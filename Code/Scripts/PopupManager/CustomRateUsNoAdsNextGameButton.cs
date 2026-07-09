using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomRateUsNoAdsNextGameButton : CustomBufferActionButton
{
    [SerializeField] private GameActionEvent gameAction = GameActionEvent.NextGame;
    [SerializeField] private bool popupNoAds;
    [SerializeField] private int rateUsPopupLevel;
    private readonly PlayerPrefInt _nextGameRated = new PlayerPrefInt("next_game_rated");
    [SerializeField] private PlayerPrefRemoteArrayInt level_show_no_ads_pack = new PlayerPrefRemoteArrayInt(
        RemoteConfigKey.level_show_no_ads_pack, new[]
        {
            1, 5, 10, 15, 30
        });

    protected override void DoOnClick()
    {
        if (!_nextGameRated.BoolValue &&
            PlayerData.currentLevel.Value == RemoteConfigKey.game_play_level_pass_to_rate.GetValueInt())
        {
            Pause.Value = true;
            _nextGameRated.BoolValue = true;
            ScreenRoot.popupManager.ShowPopup<PopupRateUs>(rateUsPopupLevel).SetActionWhenClose(Action);
        }
        else if (popupNoAds && !Kernel.Resolve<AdsManager>().IsNoAds() &&
                 DuongExtensions.IsListContains(level_show_no_ads_pack.Value, LevelInt, true))
        {
            Pause.Value = true;
            ScreenRoot.popupManager.ShowPopup<PopupNoAds>(rateUsPopupLevel).SetActionWhenClose(Action);
        }
        else
        {
            base.DoOnClick();
        }
    }

    protected override void Action()
    {
        base.Action();
        SubjectController.GameActionEvent.OnNext(gameAction);
    }

    protected override string GetName()
    {
        return "btn_next_game_popup_rate";
    }
}
