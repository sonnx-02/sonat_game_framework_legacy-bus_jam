using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptMultiple : BaseActionScript
{
    [SerializeField] private int multipleValue = 2;
    [SerializeField] private IndexBindingScriptGetRewardsBase[] scripts;

    protected override void DoAction()
    {
        base.DoAction();
        foreach (var s in scripts)
            s.Multiple(multipleValue);
    }

    
}
