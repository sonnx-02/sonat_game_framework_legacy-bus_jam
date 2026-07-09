using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetLevelDifficulty : BaseActionScript
{
    [SerializeField] private IndexBindingScript[] scripts;
    protected override void DoAction()
    {
        base.DoAction();
        var dif = RootView.rootView.gameController.GetDifficulty(PlayerData.CurrentLevel);
        scripts.OnChanged(dif);
    }
}
