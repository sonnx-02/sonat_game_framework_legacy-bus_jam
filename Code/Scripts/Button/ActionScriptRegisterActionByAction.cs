using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptRegisterActionByAction : BaseBaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.RegisterAction)]
    [SerializeField] private int action;

    [SerializeField] private BaseBaseActionScript[] actions;

    protected override void Register()
    {
        base.Register();
        SubjectController.RegisterAction(action, () =>
        {
            actions.StartAction();
        });
    }

    public override void LogCustom()
    {
        base.LogCustom();
        actions.StartAction();
    }
}
