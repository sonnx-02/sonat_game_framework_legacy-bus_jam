using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupShowRewardInBox : BasePopup
{
    [SerializeField] private RewardView[] rewardViews;
    [SerializeField] private IndexBindingScriptBase[] bindingScripts;
    
    public void SetRewards(IList<Product> rewards)
    {
        PlayerData.Claim(rewards);
        rewardViews.SetActiveAll(false);
        for (var i = 0; i < rewards.Count; i++)
        {
            rewardViews[i].gameObject.SetActive(true);
            rewardViews[i].SetData(rewards[i]);
        }
    }

    public override void SetParameter(CustomParameter parameter)
    {
        base.SetParameter(parameter);
        if(parameter != null)
            bindingScripts.OnChanged(parameter.intValue);
    }

    public void SetClose()
    {
       
    }
    
}
