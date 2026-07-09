using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable InconsistentNaming

public abstract class BaseGameControllerNavi : BaseGameController
{
    [Serializable]
    public class NaviSettings
    {
        public bool turnNavigation = true;
        public int navigationTime = 15;
        public IndexBindingScriptBase[] navigations;
        public Point RandomAdBreak => DuongExtensions.GetRemotePoint("ad_break_random_from", "ad_break_random_to");
    }

    [SerializeField] protected NaviSettings naviSettings;

    private IDisposable _navigationDisposable;
    private int _secondsFromAdBreak;
    private int _secondsToAdBreak;
    private int _waitForHintIdle;
    private int _secondsFromLastAction;
    private int Seconds;
    protected int _currentNavigation;


    protected override void InitGame()
    {
        base.InitGame();
        StartCountDownNavigation();
        ResetTimeIdleHint();
    }

   

    protected virtual void StartCountDownNavigation()
    {
        UpdateActionTime();
        _secondsFromAdBreak = 0;
        _secondsToAdBreak = naviSettings.RandomAdBreak.GetRandomBetween();
        _navigationDisposable?.Dispose();
        if (naviSettings.turnNavigation)
            _navigationDisposable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(data =>
                {
                    if (ValidInput())
                    {
                        PlayerData.logEachGame[(int) LogEachGameEnum.time_play]++;
                        if (!IsInTutorial())
                        {
                            Seconds++;
                            if (_currentNavigation < 0)
                                _secondsFromLastAction++;
                            _secondsFromAdBreak++;
//                        Debug.Log(_secondsFromAdBreak+"/"+_secondsToAdBreak);
                            CheckShowAdBreak();
                            _waitForHintIdle++;
                            CheckGameNavigation();
                            OnCounting(Seconds);
                            CheckHintIdle();
                        }
                        OnValidInputPerSeconds(PlayerData.logEachGame[(int) LogEachGameEnum.time_play]);
                    }
                })
                .AddToGameDisposable();
    }

    protected virtual void OnValidInputPerSeconds(int seconds)
    {
    }

    public virtual int Phase => 0;

    protected virtual void LogLevelEnd(bool isWin, string loseCause = "", string mode = "classic",
        Sonat.LogParameter[] extraParameter = null, bool levelUp = true, string continue_with = null,
        int continue_times = 0)
    {
        new SonatLogLevelEnd(PlayerData.DisplayCurrentLevelInt, Phase, GetPlayTime(), isWin,
                PlayerData.levelPlayTimes[Level] == 1, mode)
            {
                lose_cause = loseCause,
                move_count = GetMoveCount(),
                use_booster_count = BoosterCount,
                continue_with = continue_with,
                continue_times = continue_times,
            }
            .SetExtraParameter(extraParameter)
            .Post();

        if (levelUp)
            new SonatLogLevelUp()
                {
                    level = PlayerData.DisplayCurrentLevelInt ,
                }
                .Post();
    }

    protected string _continue_with;

    protected virtual void LogLevelStart(string continue_with = null, int continue_times = 0)
    {
        _continue_with = continue_with;
        new SonatLogLevelStart()
        {
            level = PlayerData.DisplayCurrentLevelInt,
            phase = Phase,
            mode = "classic",
            is_first_play = PlayerData.levelPlayTimes[Level] == 1,
            continue_with = continue_with,
            continue_times = continue_times,
        }.Post();
    }

    protected override void OnStartANewGame()
    {
        base.OnStartANewGame();
        LogLevelStart();
        DuongQualityChecking.CheckDuplicateCall(nameof(LogLevelStart));
    }

    protected int GetPlayTime()
    {
        return PlayerData.logEachGame[(int) LogEachGameEnum.time_play];
    }

    protected int GetMoveCount()
    {
        return PlayerData.logEachGame[(int) LogEachGameEnum.moves];
    }

    protected override void OnInputUpdate()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            ResetTimeIdleHint();
    }

    protected override void HandlerContinue()
    {
        base.HandlerContinue();
        ResetTimeIdleHint();
    }

    protected override void HandlerBoardLose()
    {
        base.HandlerBoardLose();
        _waitForHintIdle = int.MinValue;
        OnHintIdleChanged(false);
    }

    protected void ResetTimeIdleHint()
    {
        _waitForHintIdle = 0;
        HintBoosterIdle(false);
    }

    protected virtual void CheckHintIdle()
    {
        if (ValidToHintIdle())
            HintBoosterIdle(true);
    }

    public override void SkipTutorial(int index, int amount)
    {
        base.SkipTutorial(index, amount);
        _waitForHintIdle = 0;
        HintBoosterIdle(false);
    }

    protected virtual bool ValidToHintIdle() => PlayerData.tutorialStep == -1 && _waitForHintIdle >= TimeToHintIdle;

    protected virtual int TimeToHintIdle => BaseCurrentGameSetting.timeToHintIdle;

    [SerializeField] private ToggleScript[] hintIdle;

    protected virtual void OnHintIdleChanged(bool isOn)
    {
        hintIdle.OnChanged(isOn);
    }

    private bool _lastValueHintIdle;

    private void HintBoosterIdle(bool hint)
    {
        if (hint == _lastValueHintIdle)
            return;

        _lastValueHintIdle = hint;
        OnHintIdleChanged(hint);
    }


    private void CheckShowAdBreak()
    {
        if ((_secondsFromAdBreak - _secondsToAdBreak) % 15 == 0)
            UIDebugLog.Log(_secondsFromAdBreak + "/" + _secondsToAdBreak);
        if (_secondsFromAdBreak >= _secondsToAdBreak && Kernel.Resolve<AdsManager>().IsInterstitialAdsReady() &&
            !BoardBusy.Value)
        {
            if (ScreenRoot.dialogController.HasKey<DialogAdBreak>())
            {
                ScreenRoot.dialogController.Resolve<DialogAdBreak>().Show();
                _secondsFromAdBreak = 0;
                _secondsToAdBreak = naviSettings.RandomAdBreak.GetRandomBetween();
            }
            else
            {
                _secondsFromAdBreak = 0;
                Debug.LogError("duong : not contains DialogAdBreak");
            }
        }
    }

    public override void UpdateActionTime()
    {
        _secondsFromLastAction = 0;
        ClearGameNavigation();
    }

    protected virtual void ClearGameNavigation()
    {
        _currentNavigation = -1;
        naviSettings.navigations.OnChanged(_currentNavigation);
    }

    protected virtual void CheckGameNavigation()
    {
        if (_secondsFromLastAction > naviSettings.navigationTime
            && _currentNavigation < 0
            //  && Blocks.Count > currentGameSetting  .numberOfBlockToNavi
        )
        {
            StartNavigation();
        }
    }
    
    protected virtual void OnCounting(int second)
    {
    }

    protected virtual void StartNavigation()
    {
        if (_currentNavigation < 0 && ValidInput())
        {
            _currentNavigation = UnityEngine.Random.Range(0, 3);
            naviSettings.navigations.OnChanged(_currentNavigation);
        }
    }
}