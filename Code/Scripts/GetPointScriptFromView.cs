using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPointScriptFromView : GetPointScriptBase
{
    [SerializeField] private EffectView effectView;
    
    public override PointParameter GetPoint(BuiltInEnumType type, int afterInCallBack)
    {
        var point = new PointParameter(type,afterInCallBack);
        if (effectView != null)
            point.value = effectView.GetIndex();
        return point;
    }

}
