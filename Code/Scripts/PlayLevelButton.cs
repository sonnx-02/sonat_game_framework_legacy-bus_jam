using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayLevelButton : BaseFrameworkView
{
    public InputField inputField;
    public int add;
    protected override void Register()
    {
        base.Register();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(inputField == null)
                BaseGameController.StartLevel(Mathf.Max(PlayerData.CurrentLevel + add,0));
            else
                BaseGameController.StartLevel(Mathf.Max(int.Parse(inputField.text),0));
        });
    }
}
