using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPointItem
{
     Point Point { get;  }
}

public class PoolItemPoint : PoolItem,IPointItem
{
    public Vector3Int setValue;
    public Point Point { get; private set; }
    [SerializeField] private Vector3 adjustPos;

    public virtual void SetPoint(Point point)
    {
        Point = point;
        UpdatePos();
    }
    
    public virtual void SetPointOnly(Point point)
    {
        Point = point;
    }


    public void UpdatePos()
    {
        transform.position = RootView.rootView.gameController.GetPos(Point) + adjustPos;
    }
    
    [ContextMenu("Log")]
    private void Log()
    {
        Point.Print();
    }
}