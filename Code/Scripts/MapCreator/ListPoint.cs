using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum ListPointType
{
    Relative,
    Absolute,
    ColorMap,
}

[Serializable]
public class ListVector3
{
    public Vector3[] vecs;
}

[Serializable]
public class ListPoint
{
    public CustomParameter customParameter = new CustomParameter();
    public List<PointParameter> points = new List<PointParameter>();
    
    public int Count => points.Count;
    public ListPoint Clone()
    {
        return new ListPoint()
        {
            customParameter = customParameter.Clone(),
            points = points.Select(x => x.CloneCurrent()).ToList(),
        };
    }
    
    public ListPoint Rotate(int rotate)
    {
        var clone = new ListPoint
        {
            customParameter = customParameter.Clone(), points = points.Select(x => x.CloneCurrent()).ToList()
        };
        foreach (var pointWithTwoValue in clone.points)
        {
            pointWithTwoValue.Rotate90(rotate);
        }

        return clone;
    }
}


[Serializable]
public class ListPointPoint
{
    public CustomParameter customParameter = new CustomParameter();
    public List<PointPointParameter> points = new List<PointPointParameter>();
    
    public int Count => points.Count;
    public ListPointPoint Clone()
    {
        return new ListPointPoint()
        {
            customParameter = customParameter.Clone(),
            points = points.Select(x => x.CloneCurrent()).ToList(),
        };
    }
    
    public ListPointPoint Rotate(int rotate)
    {
        var clone = new ListPointPoint
        {
            customParameter = customParameter.Clone(), points = points.Select(x => x.CloneCurrent()).ToList()
        };
        foreach (var pointWithTwoValue in clone.points)
        {
            pointWithTwoValue.Rotate90(rotate);
        }

        return clone;
    }
}

