using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActionEventByRemoteScript : BaseFrameworkView
{
    [SerializeField] private PlayerPrefRemoteBool remoteToggle = new PlayerPrefRemoteBool("next_or_quit", false);

    public GameActionEvent whenOn = GameActionEvent.NextGame;
    public GameActionEvent whenOff = GameActionEvent.Quit;

    public GameActionEvent Action
    {
        get
        {
            if (remoteToggle.Value)
                return whenOn;
            return whenOff;
        }
    }
}
