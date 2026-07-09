using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupWin_CustomBase : BasePopup
{
    [SerializeField] private BaseBaseActionScript[] actions;
    [SerializeField] private IndexBindingScriptBase[] playModeBinds;
    [SerializeField] private IndexBindingScriptBase[] levelBinds;
    [SerializeField] private ArrayIntSplitScript[] arrayIntHandlers;
    [SerializeField] private IndexBindingScriptGetRewardsBase rewardToClaim;
    [SerializeField] private float delayToClaim;

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        foreach (var baseActionScript in actions)
            baseActionScript.StartAction(0);
    }

    public virtual void SetData(int timesPlay, int playModeValue, int level, int[] arrayInt)
    {
        playModeBinds.OnChanged(playModeValue);
        levelBinds.OnChanged(level);
        foreach (var arrayIntSplitScript in arrayIntHandlers)
            arrayIntSplitScript.OnChanged(arrayInt);
        if (timesPlay == 0 && rewardToClaim != null)
            delayToClaim.Timer(rewardToClaim.Claim);
    }
}
