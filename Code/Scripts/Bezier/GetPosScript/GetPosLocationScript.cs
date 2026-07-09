using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GetPosLocationScript : GetPosScript
{
    [SerializeField] private LocationEnum location;

    public override RectTransform GetRectTransformTarget()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            foreach (var item in  FindObjectsOfType<LocationItem>())
            {
                if (item.gameObject.activeInHierarchy && item.locationEnum == location)
                    return item.transform as RectTransform;
            }
            Debug.LogError("not found"+location);
            return null;
        }
#endif
        return RootView.rootView.screenRoot.GetLocation(location);
    }

    public override Vector3 GetWorldPos()
    {
        return GetRectTransformTarget().position;
    }
}
