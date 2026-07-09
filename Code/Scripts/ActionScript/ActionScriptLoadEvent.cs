using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptLoadEvent : BaseActionScript
{
    [IndexAsEnum(nameof(EventDayType))]
    [SerializeField] private int eventType;
    protected override void DoAction()
    {
        base.DoAction();
        StaticDataController.LoadEventPlayerData(eventType,RootView.rootView.gameController.Level,eventKey => RootView.rootView.gameController.OnLoadedEventDay(eventType,eventKey));
    }
}
