using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptTurnRayCast : ToggleScript
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image[] imgs;
    protected override void LoadComponent()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void Trigger(bool val)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = val;
        foreach (var image in imgs)
            image.raycastTarget = val;
    }
}
