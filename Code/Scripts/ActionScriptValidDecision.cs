using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ValidDataAction 
{
    [SerializeField] private BaseBaseActionScript[] actions;
    [SerializeField] private  ValidScriptBase validator; // if has retriever use retriever not value
    [SerializeField] private  bool reverse;
 
    public bool CheckStartAction()
    {
        bool valid = validator.IsValid();
        if (reverse)
            valid = !valid;
        if(valid)
            actions.StartAction();

        return valid;
    }
}

public class ActionScriptValidDecision : BaseActionScriptInput
{
    [SerializeField] private Button button;
//    public BaseBaseActionScript[] defaultActions;
    protected override void Register()
    {
        base.Register();
        if (button != null)
            button.onClick.AddListener(DoAction);
    }

    [SerializeField] private ValidDataAction[] actions;
    [SerializeField] private ForRunType runType = ForRunType.ForEachAll;

    protected override void DoAction()
    {
        base.DoAction();
        foreach (var mathCompareDataAction in actions)
        {
            var check = mathCompareDataAction.CheckStartAction();
            if (runType == ForRunType.StopWhenFalse && !check)
                break;
            if (runType == ForRunType.StopWhenTrue && check)
                break;
        }
    }
}
