using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingScriptAnchorPos : IndexBindingScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2[] onAnchorPos;

    protected override void LoadComponent()
    {
        rectTransform = (RectTransform) transform;
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        rectTransform = GetComponent<RectTransform>();
        onAnchorPos[value] = rectTransform.anchoredPosition;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        rectTransform.anchoredPosition = onAnchorPos[computedValue];
    }
}