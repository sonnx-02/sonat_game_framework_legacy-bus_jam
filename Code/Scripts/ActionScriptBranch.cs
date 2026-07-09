using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptBranch : BaseActionScript
{
    [SerializeField] private BaseBaseActionScript[] ifTrue;
    [SerializeField] private BaseBaseActionScript[] ifFalse;
    [SerializeField] private bool checkLevel;
    [SerializeField] private bool setFlag;

    [SerializeField] private bool flag;
    public override void StartAction(bool value)
    {
        if (setFlag)
            flag = value;
        else
        {
            base.StartAction(value);

            if (value)
            {
                ifTrue.StartAction();
            }
            else if (PlayerData.CurrentLevel < GameDefine.AutoGoHomeLevel && checkLevel)
            {
                ifFalse[0].StartAction();
            }
            else
            {
                ifFalse[1].StartAction();
            }
        }
    }

    protected override void DoAction()
    {
        base.DoAction();

        if (flag)
        {
            ifTrue.StartAction();
        }
        else if (PlayerData.CurrentLevel < GameDefine.AutoGoHomeLevel && checkLevel)
        {
            ifFalse[0].StartAction();
        }
        else
        {
            ifFalse[1].StartAction();
        }
    }
}
