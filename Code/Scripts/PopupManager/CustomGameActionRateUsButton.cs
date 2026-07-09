using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomGameActionRateUsButton : BaseButton
{
    [SerializeField] private bool loopLastValue;

    [SerializeField] private PlayerPrefRemoteArrayInt levelActive = new PlayerPrefRemoteArrayInt(
        "level_rate_us", new[]
        {
            1, 5, 10, 15, 30
        });

    [SerializeField] private int rateUsPopupLevel;

    [SerializeField] private bool showInters;

    [SerializeField] [ConditionalField(nameof(showInters))]
    private string showIntersLog;

    [SerializeField] [ConditionalField(nameof(showInters))]
    private bool conditionLevel = true;

    [SerializeField] [ConditionalField(nameof(overrideAction), new object[] {null})]
    private GameActionEvent action;

    [SerializeField] private GameActionEventByRemoteScript overrideAction;

    private GameActionEvent GameActionEvent => overrideAction != null ? overrideAction.Action : action;

    public bool RateClicked
    {
        get => PlayerPrefs.GetInt("rate_clicked") == 1;
        set => PlayerPrefs.SetInt("rate_clicked", value ? 1 : 0);
    }


    protected override void OnClick()
    {
        base.OnClick();
        if (showInters)
        {
            if (!SessionData.skipIntersOnce)
                Kernel.Resolve<AdsManager>()
                .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false, conditionLevel);
        }
        SessionData.skipIntersOnce = false;

        if (IsRated() && IsInLevelRate() && !RateClicked)
            ScreenRoot.popupManager.ShowPopup<PopupRateUs>(rateUsPopupLevel).OverrideClose(Action);
        else
            Action();
    }

    private bool IsInLevelRate()
    {
        var level = PlayerData.currentLevel.Value;
        foreach (var i in levelActive.Value)
            if (level == i)
                return true;

        if (loopLastValue && levelActive.Value.Length > 0 )
        {
            var last = levelActive.Value[levelActive.Value.Length - 1];
            if (level > last)
            {
                var repeat = levelActive.Value.GetRepeatValue();
                var sub = level - last;
                if (sub % repeat == 0)
                    return true;
            }
        }

        return false;
    }

    private bool IsRated()
    {
        return PlayerData.levelPlayTimes[PlayerData.CurrentLevel] == 1;
    }

    private void Action()
    {
        SubjectController.GameActionEvent.OnNext(GameActionEvent);
    }


    protected override string GetName()
    {
        return "btn_custom_next_game";
    }
}