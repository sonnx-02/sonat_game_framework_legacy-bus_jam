using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxHandClickToggle : ToggleScript
{
    [SerializeField] private RxFrameAnimationScript frameAnimationScript;

    [SerializeField] private ClickData whenClick;
    [SerializeField] private ClickData whenIdle;


    [Serializable]
    private struct ClickData
    {
        public FramePlayType type;
        public int index;
    }

    private void Handler(ClickData data)
    {
        switch (data.type)
        {
            case FramePlayType.PingPongOnce:
                frameAnimationScript.PlayPingPongOnce(data.index);
                break;
            case FramePlayType.Once:
                frameAnimationScript.PlayOnce(data.index);
                break;
            case FramePlayType.Idle:
                frameAnimationScript.StartIdle(data.index);
                break;
            case FramePlayType.OnceReverse:
                frameAnimationScript.PlayReverseOnce(data.index);
                break;
            case FramePlayType.PingOnce:
                frameAnimationScript.PlayPingOnce(data.index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    

    [Serializable]
    private enum FramePlayType
    {
        PingPongOnce,
        Once,
        OnceReverse,
        Idle,
        PingOnce,
    }

    protected override void LoadComponent()
    {
        frameAnimationScript = GetComponent<RxFrameAnimationScript>();
    }

    protected override void Trigger(bool val)
    {
        Handler(val ? whenClick : whenIdle);
    }
}