using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionClaimProduct : BaseActionProductScript
{
    [SerializeField] private bool safe = true;
    [ConditionalField(nameof(rewardTypeFlag),new object[]{64})]
    [SerializeField] protected CustomClaimScript customClaimScript;
    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    protected int rewardTypeFlag = 1;
    [SerializeField] private bool closeCurrentPopupOnRewarded;

    public override void StartAction(Product product)
    {
        base.StartAction(product);
        var trade = new Trade<Product, Product>(Product.Free, product);
        var i = new ShowRewardInput(trade, rewardTypeFlag);
        if ((rewardTypeFlag & (int) RewardTypeFlag.CustomClaimScript) != 0 && customClaimScript != null)
            i.CustomClaimScript = customClaimScript;
        i.closeCurrentPopup = closeCurrentPopupOnRewarded;
        RootView.rootView.screenRoot.ShowReward(i);
    }

    protected override void DoAction()
    {
        base.DoAction();
    }

    public override void StartAction()
    {
        if(!safe)
            base.StartAction();
        else
        {
            Debug.LogError("claim not invoke because of safe");
        }
    }
}