using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingSortingOrder : IndexBindingScript
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private IntString[] orderInLayers;
    [SerializeField] private BindSortingOrder[] binds;

    protected override void LoadComponent()
    {
        target = GetComponent<SpriteRenderer>();
    }

    protected override bool IsNoComponent()
    {
        return target == null;
    }

    private int _lastValue;
    protected override void OnValueChanged(int computedValue)
    {
        _lastValue = computedValue;
        base.OnValueChanged(computedValue);
        target.sortingOrder = orderInLayers[computedValue].value;
        target.sortingLayerID = SortingLayer.NameToID(orderInLayers[computedValue].value2);
        foreach (var bindSortingOrder in binds)
            bindSortingOrder.Change(target.sortingOrder, target.sortingLayerID);
    }

    public int GetCurrentSortOrder()
    {
        return orderInLayers[_lastValue].value;
    }
    
    public int GetCurrentSortLayerOrder()
    {
        return SortingLayer.NameToID(orderInLayers[_lastValue].value2);
    }

    public void ApplyToOther(BindSortingOrder other)
    {
        other.Change(target.sortingOrder, target.sortingLayerID);
    }
}
