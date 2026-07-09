using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupReceiveReward : BasePopup
{
    [SerializeField] private RewardView[] rewards;
    [SerializeField] private float timeClose;
    public void SetReward<T>(IList<T> products) where T : Product
    {
        rewards.SetActiveAll(false);
        for (var i = 0; i < products.Count; i++)
        {
            rewards[i].gameObject.SetActive(true);
            rewards[i].SetData(PlayerData.Claim(products[i]));
        }

        if(timeClose > 0)
            timeClose.Timer(Close);
    }
}
