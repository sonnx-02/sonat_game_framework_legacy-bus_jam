using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptSortOrder : ToggleScript
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private IntString orderInLayersOn;
    [SerializeField] private IntString orderInLayersOff;
    [SerializeField] private BindSortingOrder[] binds;
 
    protected override void LoadComponent()
    {
        target = GetComponent<SpriteRenderer>();
    }

    protected override void Trigger(bool val)
    {
        var order = val ? orderInLayersOn : orderInLayersOff;
        target.sortingOrder = order.value;
        target.sortingLayerID = SortingLayer.NameToID(order.value2);
        foreach (var bindSortingOrder in binds)
            bindSortingOrder.Change(target.sortingOrder, target.sortingLayerID);
    }
}
