using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptTransformConfigData : ToggleScript
{
    [SerializeField] private bool pos = true;
    [SerializeField] private bool scale = true;
    [SerializeField] private bool rotate = true;
    [SerializeField] private TransformConfigData onData;
    [SerializeField] private TransformConfigData offData;
    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        (val ? onData : offData).Apply(transform,pos,scale,rotate);
    }
    
   
#if UNITY_EDITOR
    protected override void LoadOn()
    {
        base.LoadOn();
        onData = GetData();
    }

    protected override void LoadOff()
    {
        base.LoadOff();
        offData = GetData();
    }

    TransformConfigData GetData()
    {
        var transform1 = transform;
        return new TransformConfigData()
        {
            position = transform1.localPosition,
            scale = transform1.localScale,
            quaternion = transform1.rotation,
        };
    }

#endif

}
