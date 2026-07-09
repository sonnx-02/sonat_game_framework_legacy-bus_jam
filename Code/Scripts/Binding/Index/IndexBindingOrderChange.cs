using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingOrderChange : IndexBindingScript
{
    [SerializeField] private SpriteRenderer render;
    [SerializeField] private IntString[] orders= {new IntString() {value = 0, value2 = "Default"}};
    [SerializeField] private BindSortingOrder[] binds;

    protected override void LoadComponent()
    {
        render = GetComponent<SpriteRenderer>();
    }

    protected override bool IsNoComponent()
    {
        return render == null;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        render.sortingOrder = orders[computedValue].value;
        render.sortingLayerID = SortingLayer.NameToID(orders[computedValue].value2);
        foreach (var bindSortingOrder in binds)
            bindSortingOrder.Change(render.sortingOrder, render.sortingLayerID);
    }
}
