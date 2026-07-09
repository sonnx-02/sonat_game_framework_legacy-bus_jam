using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AchievementItemView : GenericQuestItemView
{
    [IndexAsEnum(BuiltInEnumType.QuestEnum)]
    [SerializeField] protected int key;
    [SerializeField] protected ProgressScript[] fragmentProgress;
    [SerializeField] protected BindingToStringTextChange[] fragmentProgressText;
    [SerializeField] protected bool tween;
    [SerializeField] protected float timeTween = 0.5f;
    
    protected AchievementItem Data;

    public int Key => Data.key;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        var gameSetting = Kernel.GetDatabase<GameSetting>();
        foreach (var item in gameSetting.achievementItems)
            if (item.Key == key)
            {
                Data = item;
                Setup(item);
                
                var progress = Data.GetFragmentProgress();
                foreach (var progressScript in fragmentProgress)
                    progressScript.OnChanged(progress);
                break;
            }
    }


//    private bool firstTime;
    public override void UpdateStatus()
    {
        base.UpdateStatus();
        CheckKernelLoaded();
        var progress = Data.GetFragmentProgress();
        foreach (var progressScript in fragmentProgress)
            if (tween)
            {
                progressScript.OnChangedTween(progress,timeTween);
            }
            else
                progressScript.OnChanged(progress);
        
        foreach (var bindingToStringTextChange in fragmentProgressText)
            bindingToStringTextChange.OnChanged(Data.GetFragmentProgressString());
    }
}
