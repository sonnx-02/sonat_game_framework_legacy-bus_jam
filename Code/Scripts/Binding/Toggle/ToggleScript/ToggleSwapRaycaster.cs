using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwapRaycaster : ToggleScript
{
    [SerializeField] private GraphicRaycaster[] on;
    [SerializeField] private GraphicRaycaster[] off;
    protected override void LoadComponent()
    {
       
    }

    protected override void Trigger(bool val)
    {
        foreach (var graphicRaycaster in @on)
            graphicRaycaster.enabled = val;
        foreach (var graphicRaycaster in @off)
            graphicRaycaster.enabled = !val;
    }
}
