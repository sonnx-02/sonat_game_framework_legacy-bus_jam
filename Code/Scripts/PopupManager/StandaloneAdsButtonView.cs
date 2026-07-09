using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UnityEngine.Serialization;

public class StandaloneAdsButtonView : BaseAdsRewardButton
{

    protected override void RewardedHandler()
    {
        _action.Action.Invoke();
        base.RewardedHandler();
    }
    
    private BaseAction _action;

    public Product _reward;
    public void SetAction(BaseAction onReward,Product reward)
    {
        _action = onReward;
        _reward = reward;
    }

    
}