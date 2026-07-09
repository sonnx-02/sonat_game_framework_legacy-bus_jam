using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class PointBindingScript : BaseFrameworkView
{
    [SerializeField] private IndexBindingScriptBase[] bindCols;
    [SerializeField] private IndexBindingScriptBase[] bindRows;
   
    public Point Point { get; set; }
    public virtual void OnChanged(Point point)
    {
        Point = point;
        bindCols.OnChanged(point.col);
        bindRows.OnChanged(point.row);
    }
}
