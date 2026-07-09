using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndexBindingRectTransformPositions : IndexBindingScript
{
    [SerializeField] private RectTransform target;
    [SerializeField] private Vector2[] anchorPositions;

   public override int Size => anchorPositions.Length;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        target.anchoredPosition = anchorPositions[computedValue];
    }

    

    public override void LoadIndex(int index)
    {
        base.LoadIndex(index);
        if (index < anchorPositions.Length && index >= 0)
            anchorPositions[index] = target.anchoredPosition;
        else
        {
            var newList = anchorPositions.ToList();
            newList.Add(target.anchoredPosition);
            anchorPositions = newList.ToArray();
        }
    }
}
