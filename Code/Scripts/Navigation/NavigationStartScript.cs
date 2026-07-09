using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationStartScript : BaseActionScriptBoolean
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.NavigationEnum)]
    private int key;

    [SerializeField] private Button button;


    protected override void Register()
    {
        base.Register();
        if (!PlayerData.navigationChecker.Contains(key)  && button != null)
            button.onClick.AddListener(StartAction);
    }

    public override void StartAction()
    {
        base.StartAction();
        if (!PlayerData.navigationChecker.Contains(key) && Value)
            ScreenRoot.navigationManager.StartNavi(key);
    }
}
