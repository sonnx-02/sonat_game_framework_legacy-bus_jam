using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GetPosScriptGameLocation : GetPosScript
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.GameLocationEnum)]
    private int location;

    public override RectTransform GetRectTransformTarget()
    {
        return RootView.rootView.gameController.GetGameLocationEnum(location);
    }

    public override Vector3 GetWorldPos()
    {
        return RootView.rootView.gameController.GetWorldPos(location);
    }
}
