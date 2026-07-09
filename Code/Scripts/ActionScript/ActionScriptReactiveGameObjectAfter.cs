using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ActionScriptReactiveGameObjectAfter : BaseActionScript
{
    [SerializeField] private GameObject[] gameObjects;


    public override void Start()
    {
        base.Start();
//        .1f.Timer(() => { gameObjects.SetActiveAll(false); });
//        SubjectController.ScreenShowed.Subscribe(data => DoAction());
//        SubjectController.ScreenClosed.Subscribe(data => DoAction());
    }

    protected override void DoAction()
    {
        base.DoAction();
        gameObjects.SetActiveAll(false);
        1f.Timer(() => { gameObjects.SetActiveAll(true); });
    }
}
