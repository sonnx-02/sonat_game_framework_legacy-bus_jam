using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckScriptPopupTutorial : BaseCheckScript
{
    
    [IndexAsEnum(BuiltInEnumType.TutorialPopupKey)]
    [SerializeField] private int tutorialPopup;

    [SerializeField] private BaseBaseActionScript[] actions;

    public override bool Check()
    {
        var check = base.Check();
        var resolve = ScreenRoot.popupManager.Resolve<PopupTutorial>();

        if (check)
            check &= resolve.CheckTut(tutorialPopup, null);
        if(check)
            actions.StartAction();
        return check;
    }
}
