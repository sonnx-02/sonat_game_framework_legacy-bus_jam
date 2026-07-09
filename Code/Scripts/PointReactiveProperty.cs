using System;
using UniRx;
using UnityEngine;

/// <summary>
/// Inspectable ReactiveProperty.
/// </summary>
[Serializable]
public class PointReactiveProperty : ReactiveProperty<Point>
{
    public PointReactiveProperty()
        : base()
    {
    }

    public PointReactiveProperty(Point initialValue)
        : base(initialValue)
    {
    }

}