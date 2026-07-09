using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptAnchorPos : ToggleScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 onAnchorPos;
    [SerializeField] private Vector2 offAnchorPos;

    protected override void LoadComponent()
    {
        rectTransform = (RectTransform) transform;
    }

    protected override void Trigger(bool val)
    {
        rectTransform.anchoredPosition = val ? onAnchorPos : offAnchorPos;
    }

    protected override void LoadOn()
    {
        base.LoadOn();
        rectTransform = GetComponent<RectTransform>();
        onAnchorPos = rectTransform.anchoredPosition;
    }

    protected override void LoadOff()
    {
        base.LoadOff();
        rectTransform = GetComponent<RectTransform>();
        offAnchorPos = rectTransform.anchoredPosition;
    }
}