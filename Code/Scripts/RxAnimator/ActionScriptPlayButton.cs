using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionScriptPlayButton : BaseActionScript
{
    [SerializeField] private BaseButton[] buttons;
    [SerializeField] private Button[] buttons2;
    protected override void DoAction()
    {
        base.DoAction();
        foreach (var baseButton in buttons)
            baseButton.TriggerOnClick();
        foreach (var baseButton in buttons2)
            baseButton.onClick.Invoke();
    }
}
