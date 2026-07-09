using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingCameraSizeRemap : IndexBindingScript
{
    [SerializeField] private Vector2Int remap;
    [SerializeField] private Vector2 remapValue;
    [SerializeField] private Camera cam;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var value = (computedValue - remap.x * 1f) / (remap.y - remap.x);
        cam.orthographicSize = Mathf.Lerp(remapValue.x, remapValue.y, value);
    }
}
