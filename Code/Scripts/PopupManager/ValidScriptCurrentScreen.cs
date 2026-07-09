using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidScriptCurrentScreen : ValidScriptBase
{
    [SerializeField] private string screenName;
    protected override bool IsThisValid()
    {
        return ScreenRoot.CurrentScreen.name.ToLower() == screenName.ToLower();
    }
}
