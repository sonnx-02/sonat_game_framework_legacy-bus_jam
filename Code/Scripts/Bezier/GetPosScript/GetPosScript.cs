using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GetPosScript : MonoBehaviour
{
    public abstract RectTransform GetRectTransformTarget();
    public abstract Vector3 GetWorldPos();

    public virtual Vector2 GetAnchorPos()
    {
        return  FrameworkExtensions.WorldToPositionInCanvas(GetWorldPos());
    }
}
