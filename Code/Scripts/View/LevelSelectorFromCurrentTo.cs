using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class LevelSelectorFromCurrentTo : LevelSelectorBase
{
    [SerializeField] protected BaseBaseActionScript[] actionCurrentLevel;
    [SerializeField] protected int from;
    [SerializeField] protected int take;

    protected override void Register()
    {
        base.Register();
        var takeFrom = PlayerData.CurrentLevel + @from;
        var takeTo = takeFrom + take;
        _levels = new List<LevelSelect>();

        for (int i = 0; i < take + 1; i++)
        {
            var levelSelect = selects.pools[0].Rent();
            levelSelect.SetIndex(takeFrom+i);
            _levels.Add(levelSelect);
        }
    }

    private IList<LevelSelect> _levels;

    public override void OnShow()
    {
        base.OnShow();
        CheckRegister();
        var takeFrom = PlayerData.CurrentLevel + @from;
        actionCurrentLevel.StartAction(PlayerData.CurrentLevel);
        for (int i = 0; i < take+1; i++)
        {
            var levelSelect = _levels[i];
            var level = takeFrom + i;
            levelSelect.SetIndex(level);
            var state = GetState(levelSelect.Index);
            levelSelect.SetState((int)state);
            levelSelect.SetStar(PlayerData.levelProgress[levelSelect.Index]);
            levelSelect.SetDifficulty(RootView.rootView.gameController.GetDifficulty(level));
            if (PlayerData.CurrentLevel == level)
            {
                _levelSelect.Value = levelSelect;
                bindDifficulty.OnChanged(levelSelect.Difficulty);
            }
        }
    }

    [SerializeField] private IndexBindingScript[] bindDifficulty;
    
    private ReactiveProperty<LevelSelect> _levelSelect = new ReactiveProperty<LevelSelect>();
    
    protected virtual LevelSelectState GetState(int level)
    {
        if (level > PlayerData.CurrentLevel)
            return LevelSelectState.Locked;
        
        var state = LevelSelectState.Unlocked;
        if(level == PlayerData.CurrentLevel)
            return LevelSelectState.Current | LevelSelectState.Unlocked;
        if(level < PlayerData.CurrentLevel+1)
            return LevelSelectState.Unlocked;
        
        return state;
    }
}
