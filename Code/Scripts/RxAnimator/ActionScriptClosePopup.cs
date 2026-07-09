using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActionScriptClosePopup : BaseActionScript
{
    [SerializeField] private BasePopup popup;
    protected override void DoAction()
    {
        base.DoAction();
        popup.Close();
    }
}
