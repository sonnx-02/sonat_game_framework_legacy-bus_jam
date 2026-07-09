using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBlockRaycast : ToggleScript
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image[] images;
    protected override void LoadComponent()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void Trigger(bool val)
    {
        canvasGroup.blocksRaycasts = val;
        foreach (var image in images)
            image.raycastTarget = val;
    }
}
