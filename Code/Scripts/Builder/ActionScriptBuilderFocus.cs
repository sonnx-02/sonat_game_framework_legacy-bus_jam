using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptBuilderFocus : BaseBaseActionScript
{
    [SerializeField] private BuilderRoot.FocusEnum focusEnum = BuilderRoot.FocusEnum.LastFinishStage;
    [SerializeField] private bool instant;

    protected override void DoAction()
    {
        base.DoAction();
        RootView.rootView.gameController.BuilderRoot.Focus(focusEnum,instant);
    }
}
