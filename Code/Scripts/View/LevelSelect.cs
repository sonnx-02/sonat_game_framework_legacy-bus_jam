using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelect : RxScrollItemBase
{
    [SerializeField] protected IndexBindingScriptBase[] stars;
    [SerializeField] protected IndexBindingScriptBase[] difficultyBindings;
    [SerializeField] [DisplayAsDisable] private LevelSelectState display;
    public LevelSelectState State => display;
    public int Level => Index;
    
    public override void SetState(int value)
    {
        base.SetState(value);
        display = (LevelSelectState)value;
    }

    public void SetStar(int i)
    {
        stars.OnChanged(i);
    }

    [SerializeField] private int difficulty;

    public int Difficulty => difficulty;
    public virtual void SetDifficulty(int dif)
    {
        difficulty = dif;
        difficultyBindings.OnChanged(dif);
    }
}