using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public enum BuiltInFunction
{
    Pause,
    Resume,
    Retry,
    Undo,
    NextGame,
    Hint,
    Play,
    QuitGame,
    Surrender,
    SkipTut,
    ClearMoves,
    Restore,
    ClearPendingShopItem,
    UnPause
//    ShowPopup,
}

[RequireComponent(typeof(Button))]
public class BuiltInButton : BaseButton
{
    [SerializeField] private BuiltInFunction Enum;
    protected virtual BuiltInFunction BuiltInFunction => Enum;
    [SerializeField] private int levelTriggerCondition;

    [SerializeField]
    [ConditionLabel(nameof(BuiltInFunction),
        new object[] {BuiltInFunction.Retry, BuiltInFunction.QuitGame},
        new[] {"ForceRetry", "ScreenTransition"})]
    private bool value;

    [SerializeField] private bool showInters;

    [SerializeField] [ConditionalField(nameof(showInters))]
    private string showIntersLog;

    [SerializeField] [ConditionalField(nameof(showInters))]
    private bool conditionLevel = true;

    [SerializeField] private int[] exceptLevels;

    protected override string IconShortCutName => Enum.ToString();

    protected override void Register()
    {
        switch (BuiltInFunction)
        {
            case BuiltInFunction.Pause:
                break;
            case BuiltInFunction.Resume:
                break;
            case BuiltInFunction.Retry:
                break;
            case BuiltInFunction.Undo:
                break;
            case BuiltInFunction.NextGame:
                break;
            case BuiltInFunction.Hint:
                break;
            case BuiltInFunction.Play:
                break;
            case BuiltInFunction.QuitGame:
                break;
            case BuiltInFunction.Surrender:
                break;
            case BuiltInFunction.SkipTut:
                break;
            case BuiltInFunction.ClearMoves:
                break;
            case BuiltInFunction.Restore:
                break;
            case BuiltInFunction.ClearPendingShopItem:
                break;
            case BuiltInFunction.UnPause:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        base.Register();
    }

    protected override void OnClick()
    {
        if (PlayerData.currentLevel.Value < levelTriggerCondition)
            return;
        base.OnClick();
    }

    protected override void Handler()
    {
        Handler1(BuiltInFunction);
    }

    public void Handler1(BuiltInFunction builtInFunction)
    {
        if (showInters && !exceptLevels.Contains(PlayerData.currentLevel.Value))
            Kernel.Resolve<AdsManager>()
                .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false, conditionLevel);

        switch (builtInFunction)
        {
            case BuiltInFunction.Pause:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.Pause);
                break;
            case BuiltInFunction.Resume:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.Resume);
                if (ScreenRoot.popupManager.GetCurrentPopup() is PopupRateUs)
                {
                    new SonatLogShowRate()
                    {
                        open_by = SonatLogShowRate.ShowRateOpenBy.user,
                        action = SonatLogShowRate.ShowRateAction.close
                    }.Post();
                }

                break;
            case BuiltInFunction.Retry:
                if (value)
                    SubjectController.GameActionEvent.OnNext(GameActionEvent.Retry);
                else
                    SubjectController.GameActionEvent.OnNext(GameActionEvent.ForceRetry);
                Kernel.Resolve<FireBaseController>().LogEvent($"{builtInFunction}_clicked");
                break;
            case BuiltInFunction.Undo:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.Undo);
                Kernel.Resolve<FireBaseController>().LogEvent($"{builtInFunction}_clicked");
                break;
            case BuiltInFunction.NextGame:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.NextGame);
                Kernel.Resolve<FireBaseController>().LogEvent($"{builtInFunction}_clicked");
                break;
            case BuiltInFunction.Hint:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.Hint);
                Kernel.Resolve<FireBaseController>().LogEvent($"{builtInFunction}_clicked");
                break;
            case BuiltInFunction.Play:
                BaseGameController.HandlerButtonPlay(); //.LoadAndStartCurrentGameSave();
                break;
            case BuiltInFunction.QuitGame:
                Pause.Value = false;
                Time.timeScale = 1;
                if (value)
                {
                    ScreenRoot.IoUnloadScreen(() =>
                    {
                        ScreenRoot.Show<HomeScreen>(value);
                        BaseGameController.Clear();
                        BaseGameController.QuitGameHandler();
                    }).Subscribe();
                }
                else
                {
                    ScreenRoot.Show<HomeScreen>();
                    BaseGameController.Clear();
                    BaseGameController.QuitGameHandler();
                }
                break;
            case BuiltInFunction.Surrender:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.Surrender);
                Kernel.Resolve<FireBaseController>().LogEvent($"{builtInFunction}_clicked");
                break;
            case BuiltInFunction.SkipTut:
                SubjectController.GameActionEvent.OnNext(GameActionEvent.SkipTutorial);
                Kernel.Resolve<FireBaseController>().LogEvent($"{builtInFunction}_clicked");
                break;
            case BuiltInFunction.ClearMoves:
                BaseGameController.ClearMovesThenRestartGame();
                break;
            case BuiltInFunction.Restore:
                Kernel.ResolveDirty<BasePurchaser>().RestorePurchase();
                break;
            case BuiltInFunction.ClearPendingShopItem:
                BaseGameController.ClearUseShopItem(false);
                break;
            case BuiltInFunction.UnPause:
                BaseGameController.UnPause();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame

    protected override string GetName()
    {
        return "btn_" + BuiltInFunction;
    }
}