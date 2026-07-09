using System;
using UnityEngine;

[Serializable]
public class AchievementItem : BaseQuestItem
{
    [IndexAsEnum(BuiltInEnumType.QuestEnum)]
    public int key;

    public Product[] rewards; // parameter 1 as condition

    public int IncrementProgressValue => BaseFrameworkView.PlayerData.achievementData[key];

    public int CurrentIndex => BaseFrameworkView.PlayerData.achievementChecker[key];

    public override Product GetReward() => rewards[Mathf.Min(CurrentIndex, rewards.Length - 1)];

    [SerializeField] private bool isInfinitive;

    public override QuestState GetCurrentState()
    {
        var taken = BaseFrameworkView.PlayerData.achievementChecker[key] >= rewards.Length;
        if (taken && !isInfinitive)
            return QuestState.Taken;
        return IncrementProgressValue < Condition ? QuestState.InProgress : QuestState.Complete;
    }

    public override int Condition => GetIncreConditionByIndex(CurrentIndex);

    private int GetIncreConditionByIndex(int index)
    {
        if (!isInfinitive)
            return base.Condition;

        if (index < 0)
            return 0;
        if (index < rewards.Length)
            return rewards[index].parameter;
            
        var last = rewards[rewards.Length - 1].parameter;
        var beforeLast = rewards.Length > 1 ? rewards[rewards.Length - 2].parameter : 0;
        var add = index - rewards.Length + 1;
        return last + add * (last - beforeLast);
    }

    private int GetFragmentCondition(int index)
    {
        if (index < 0)
            return 0;
        if (index == 0)
            return rewards[index].parameter;

        var i = Mathf.Min(rewards.Length - 1, index);
        return rewards[i].parameter - rewards[i - 1].parameter;
    }

    private int GetFragmentProgressValue(int index)
    {
        return IncrementProgressValue - GetIncreConditionByIndex(index - 1);
    }

    public float GetFragmentProgress()
    {
        return GetFragmentProgressValue(CurrentIndex)*1f / GetFragmentCondition(CurrentIndex);
    }
    
    


    public override NotifyEnum NotifyEnum => NotifyEnum.Achievement;


    public override string GetProgressString()
    {
        return $"{IncrementProgressValue}/{Condition}";
    }
    
    public string GetFragmentProgressString()
    {
        return $"{GetFragmentProgressValue(CurrentIndex)}/{GetFragmentCondition(CurrentIndex)}";
    }

    public override float GetProgress()
    {
        if (Condition <= 0)
            return 1;
        return IncrementProgressValue * 1f / Condition;
    }

    public override void LogFirebase()
    {
        Kernel.Resolve<FireBaseController>().LogEvent("achievement",
            new[]
            {
                new LogParameter(GetReward().quantity.ToString().ToLower(), GetReward().amount),
                new LogParameter("name", "AchievementItem_" + BuiltInEnumType.QuestEnum.GetName(key))
            });
    }



    public override void FillData()
    {
        BaseFrameworkView.PlayerData.achievementChecker[key]++;
    }

    public override int Key => key;
}


[Serializable]
public class DailyQuestItem : BaseQuestItem
{
    [IndexAsEnum(BuiltInEnumType.QuestEnum)]
    public int key;
    public Product reward; // condition as parameter 1

    private int CurrentProgress => BaseFrameworkView.DailyData.dailyQuestData[key];

    public override QuestState GetCurrentState()
    {
        var taken = BaseFrameworkView.DailyData.dailyQuestChecker[key];
        if (taken)
            return QuestState.Taken;
        return CurrentProgress < Condition ? QuestState.InProgress : QuestState.Complete;
    }

    public override NotifyEnum NotifyEnum => NotifyEnum.DailyQuest;

    public override int Key => key;
    public override Product GetReward() => reward;


    public override void LogFirebase()
    {
        Kernel.Resolve<FireBaseController>().LogEvent("daily_quest",
            new[]
            {
                new LogParameter(GetReward().quantity.ToString().ToLower(), GetReward().amount),
                new LogParameter("name", "QuestItem_" + BuiltInEnumType.QuestEnum.GetName(key))
            });
    }

    public override void FillData()
    {
        BaseFrameworkView.DailyData.dailyQuestChecker[key] = true;
        BaseFrameworkView.DailyData.Save();
    }

    public override string GetProgressString()
    {
        return $"{CurrentProgress}/{Condition}";
    }

    public override float GetProgress()
    {
        if (Condition <= 0)
            return 1;
        return CurrentProgress * 1f / Condition;
    }
}

[Serializable]
public abstract class BaseQuestItem
{
    [SerializeField] protected Sprite questAvatar;
    [SerializeField] protected string description;

    public Sprite QuestAvatar => questAvatar;
    public string Description => description;

    public abstract int Key { get; }
    public abstract Product GetReward();

    public abstract void LogFirebase();

    public abstract void FillData();

    public abstract string GetProgressString();

    public abstract float GetProgress();

    public abstract QuestState GetCurrentState();

    public virtual int Condition => GetReward().parameter;

    public abstract NotifyEnum NotifyEnum { get; }
}
