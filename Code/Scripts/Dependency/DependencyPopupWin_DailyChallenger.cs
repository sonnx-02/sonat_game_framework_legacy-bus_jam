using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DependencyPopupWin_DailyChallenger : BasePopupDependency
{
    [SerializeField] private RewardView rewardView;

    [IndexAsEnum(BuiltInEnumType.CustomDailyDataProperty)] [SerializeField]
    private int dailyDataEnum;

    public override void OnShow()
    {
        base.OnShow();
        var current = DailyData.GetCustomProperty(dailyDataEnum);
        var rewards = BaseGameController.BaseCurrentGameSetting.dailyChallengerRewards;
        var reward = rewards[Mathf.Min(rewards.Length - 1, current.Value)].products[0];
        PlayerData.Claim(reward);
        rewardView.SetData(reward);
        current.Value++;
    }
}