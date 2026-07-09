using System;
using UniRx;
using UnityEngine;

public class CustomClaimScript : BaseFrameworkView
{
    [SerializeField] private float delayToClaim = 1;
    [SerializeField] private BaseRxAnimator[] animsIn;
    [SerializeField] private BaseRxAnimator[] animsOut;
    [SerializeField] private IndexBindingScriptBase[] bindingFinalRewardAmount;
    [SerializeField] private ToggleAction[] toggleOnStart;
    [SerializeField] private ActionScriptProductSplitter[] productsScripts;
    [SerializeField] private BaseBaseActionScript[] actionScripts;
    [SerializeField] private BaseActionProductScript[] actionProductScripts;
    public virtual void Handler(ShowRewardInput input)//, Action actionAfterClaim = null)
    {
        var toClaim =  input.trade.Product;
        bindingFinalRewardAmount.OnChanged(toClaim.amount);
        input.trade.Spend();
        foreach (var baseProductSplitter in productsScripts)
            baseProductSplitter.StartAction(toClaim);
        actionScripts.StartAction();
        actionProductScripts.StartAction(toClaim);
        foreach (var baseRxAnimator in animsIn)
            baseRxAnimator.Animator_In();
        foreach (var baseRxAnimator in animsOut)
        {
            if(baseRxAnimator.Busy)
                baseRxAnimator.StopAnim();
            baseRxAnimator.Animator_Out();
        }
        foreach (var toggleAction in toggleOnStart)
            toggleAction.Invoke();
       
        delayToClaim.Timer(() =>
        {
            PlayerData.Claim(toClaim);
            PlayerData.Save();
            DailyData.Save();
//            actionAfterClaim?.Invoke();
        });
    

        if (input.byAds)
        {
            if(input.logWhenAdsClaim != null)
                foreach (var log in input.logWhenAdsClaim)
                    log.Post();
        }
        else
        {
            if(input.logWhenClaim != null)
                foreach (var log in input.logWhenClaim)
                    log.Post();
        }
    }
}