using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCheckScript : BaseFrameworkView
{
    [SerializeField] private int levelStartCheck = -1;

    public virtual bool Check()
    {
        return PlayerData.CurrentLevel >= levelStartCheck;
    }
}

public abstract class BaseCheckScriptSkip : BaseCheckScript
{
    [SerializeField] private bool skip;

    public bool Skip => skip;
}
