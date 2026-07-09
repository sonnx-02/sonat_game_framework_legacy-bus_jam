using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
// ReSharper disable PossibleNullReferenceException


[System.Serializable]
public class GridLayoutSizeApply : ResolutionAdjustApply
{
     public Vector2 cellSize;
     public RectOffset padding;
     public Vector2 spacing;
}


[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutSizeByResolution : BaseAdjustByResolution
{
     public List<GridLayoutSizeApply> conditions = new List<GridLayoutSizeApply>();

    public override ResolutionAdjustApply this[int index] => conditions[index];

    public override int Count => conditions.Count;

    private GridLayoutGroup Grid => GetComponent<GridLayoutGroup>();
    
    protected override void DoApply<T>(T apply)
    {
        var adjust = apply as GridLayoutSizeApply;
        Grid.cellSize = adjust.cellSize;
        Grid.padding = adjust.padding;
    }

    public override void AddCurrentAdjust()
    {
        conditions.Add(new GridLayoutSizeApply()
        {
            aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight,
            cellSize = Grid.cellSize,
            padding = Grid.padding,
            spacing = Grid.spacing,
        });
    }

    public override void ModifyCondition(ResolutionAdjustApply apply)
    {
        var adjust = apply as GridLayoutSizeApply;
        adjust.cellSize = Grid.cellSize;
        adjust.padding = Grid.padding;
        adjust.spacing = Grid.spacing;
    }
}
