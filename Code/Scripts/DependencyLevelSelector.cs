using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DependencyLevelSelector : BaseScreenDependency
{
    [SerializeField] private LevelUnlockType levelUnlockType;
    [SerializeField] protected ScrollRectLevelSelectInfinitive scrollRect;


    enum LevelUnlockType
    {
        ByDay,
        PassPreviousLevel,
        AllUnlocked,
    }

    protected override void Initialize()
    {
        base.Initialize();
        scrollRect.CheckRegister();
        scrollRect.onItemClick += HandlerClick;
        scrollRect.onItemRent += HandlerItemChanged;
        scrollRect.InvokeItemRentAll();
    }


    public override void OnShow()
    {
        base.OnShow();
        CheckInitialize();
        scrollRect.InvokeItemRentAll();
    }

    private void HandlerItemChanged(LevelSelect levelSelect)
    {
        var state = GetState(levelSelect.Index);
        if ((state & LevelSelectState.Locked) != LevelSelectState.None && levelSelect.Index == PlayerData.days)
            state |= LevelSelectState.ReadyToUnlock;
        levelSelect.SetState((int)state);
        levelSelect.SetIndex(levelSelect.Index);
        levelSelect.SetStar(PlayerData.levelProgress[levelSelect.Index]);
    }
    
    protected virtual LevelSelectState GetState(int level)
    {
        switch (levelUnlockType)
        {
            case LevelUnlockType.ByDay:
                return GetStateByDay(level);
            case LevelUnlockType.PassPreviousLevel:
                break;
            case LevelUnlockType.AllUnlocked:
                return LevelSelectState.Unlocked;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return LevelSelectState.Unlocked;
    }
    
    protected virtual LevelSelectState GetStateByDay(int level)
    {
        var state = LevelSelectState.None;
        if (level < PlayerData.days)
        {
            state |= LevelSelectState.Unlocked;
            if (PlayerData.levelProgress[level] > 0)
                state |= LevelSelectState.Passed;
            if (level == PlayerData.days - 1)
            {
                state |= LevelSelectState.Current;
                if ((state & LevelSelectState.Passed) == 0)
                    state |= LevelSelectState.Indicate;
            }
        }
        else
            state |= LevelSelectState.Locked;

        if (level > 0 && level == PlayerData.days && PlayerData.levelProgress[level - 1] > 0)
            state |= LevelSelectState.Indicate;

        return state;
    }
    

    protected virtual void HandlerClick(LevelSelect select)
    {
        if ((select.State & LevelSelectState.Unlocked) != LevelSelectState.None)
        {
            BaseGameController.SelectLevel(select.Level);
        }
    }
}