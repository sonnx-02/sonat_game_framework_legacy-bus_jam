using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptOrderChange : ToggleScript
{
    [SerializeField] private SpriteRenderer render;
    [SerializeField] private IntOnOff sortOrderOnOff;
    [SerializeField] private StringOnOff layerOrderOn = new StringOnOff()
    {
        @on = "Default",
        @off = "Default",
    };
    
    [SerializeField] private BindSortingOrder[] binds;

    protected override void LoadComponent()
    {
        render = GetComponent<SpriteRenderer>();
    }

    protected override void Trigger(bool val)
    {
        render.sortingOrder = val ? sortOrderOnOff.@on : sortOrderOnOff.off;
        render.sortingLayerID = val ? SortingLayer.NameToID(layerOrderOn.@on):  SortingLayer.NameToID(layerOrderOn.off);
        foreach (var bindSortingOrder in binds)
            bindSortingOrder.Change(render.sortingOrder, render.sortingLayerID);
    }
}
