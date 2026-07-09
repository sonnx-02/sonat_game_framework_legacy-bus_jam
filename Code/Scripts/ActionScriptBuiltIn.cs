using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptBuiltIn : BaseBaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.BuiltInAction)]
    [SerializeField] private int key;

    public override void StartAction()
    {
        base.StartAction();
        RootView.rootView.gameController.HandlerBuiltInAction(key);
    }
}
