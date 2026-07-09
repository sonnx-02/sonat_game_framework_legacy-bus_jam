using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IndexBindingChangeSortingOrder : IndexBindingScript
{
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private SpriteRenderer[] renderers;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (sortingGroup != null)
            sortingGroup.sortingOrder = computedValue;
        foreach (var spriteRenderer in renderers)
            spriteRenderer.sortingOrder = computedValue;
    }
}