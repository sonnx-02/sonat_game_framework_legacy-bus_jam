using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UnityEngine.UI;

public class ButtonShortcutLog : BaseFrameworkView
{
    [SerializeField]
    private string shortCutName;

    
    protected override void Register()
    {
        base.Register();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }


    protected void OnClick()
    {
        BaseGameController.LogShortcut(shortCutName);
    }

  
}
