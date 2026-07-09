using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActionEventButton : BaseButton
{
    [SerializeField]
    private GameActionEvent key;

 
    // Start is called before the first frame update
    protected override void OnClick()
    {
        base.OnClick();
        SubjectController.GameActionEvent.OnNext(key);
    }

    // Update is called once per frame

    protected override string GetName()
    {
        return "btn" + key;
    }
}