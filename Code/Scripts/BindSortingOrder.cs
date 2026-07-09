using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SortOrderItem
{
    [SerializeField] private BindSortingOrder[] targets;

    public void SetOrder(int sortingKey)
    {
        var sorting = Kernel.GetDatabase<GameSetting>().GetSortingOrderData(sortingKey);
        foreach (var bindSortingOrder in targets)
            bindSortingOrder.Change(sorting);
    }
}

public class BindSortingOrder : LoadMono
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private MeshRenderer target2;
    public int add = 1;

    public override void Load()
    {
        target = GetComponent<SpriteRenderer>();
        target2 = GetComponent<MeshRenderer>();
    }

    public void Change(int targetSortingOrder, int targetSortingLayerId)
    {
        if (target != null)
        {
            target.sortingOrder = targetSortingOrder + add;
            target.sortingLayerID = targetSortingLayerId;
        }
        if (target2 != null)
        {
            target2.sortingOrder = targetSortingOrder + add;
            target2.sortingLayerID = targetSortingLayerId;
        }
    }
    
    public void Change(Vector2Int data)
    {
        if (target != null)
        {
            target.sortingOrder = data.x + add;
            target.sortingLayerID = data.y;
        }
        if (target2 != null)
        {
            target2.sortingOrder = data.x + add;
            target2.sortingLayerID = data.y;
        }
    }
}
