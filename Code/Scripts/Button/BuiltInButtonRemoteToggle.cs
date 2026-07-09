using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuiltInButtonRemoteToggle : BuiltInButton
{
    [SerializeField] private string remoteKey = "continue_or_quit";

    [FormerlySerializedAs("remoteEnum")] [SerializeField] private BuiltInFunction functionWhenRemoteOn;

    private PlayerPrefRemoteBool _remote;
    protected override BuiltInFunction BuiltInFunction
    {
        get
        {
            if (RemoteConfigHelper.GetValueBoolean(remoteKey))
                return functionWhenRemoteOn;

            return   base.BuiltInFunction;
        }
    }
}
