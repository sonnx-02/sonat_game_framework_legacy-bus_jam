using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionScriptInvokeButton : BaseBaseActionScript
{
    [SerializeField] private Button[] buttons;

    public override void StartAction()
    {
        base.StartAction();
        foreach (var button in buttons)
        {
            button.onClick.Invoke();
        }
    }
}
