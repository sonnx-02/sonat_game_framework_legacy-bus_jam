using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingScriptGetRewards : IndexBindingScriptGetRewardsBase
{
    [SerializeField] private GetRewardSource source;

    [System.Serializable]
    private enum GetRewardSource
    {
        Challenger
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        switch (source)
        {
            case GetRewardSource.Challenger:
                Rewards = BaseGameController.BaseCurrentGameSetting.challengerRewards[computedValue];
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        for (var i = 0; i < Rewards.products.Length; i++)
        {
            rewardViews[i].gameObject.SetActive(true);
            rewardViews[i].SetData(Rewards.products[i]);
        }
    }
}