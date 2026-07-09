using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptCanvasGroup : ToggleScript
{

    [SerializeField] private CanvasGroup[] canvasGroups;

    protected override void Trigger(bool val)
    {
        foreach (var canvasGroup in canvasGroups)
            canvasGroup.interactable = val;
    }


    protected override void LoadComponent()
    {
     
    }
}
