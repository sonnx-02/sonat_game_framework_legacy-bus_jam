using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MapListPoint
{
    public ListPoint map = new ListPoint();
    public List<PointParameter> spawns = new List<PointParameter>();

    public MapListPoint Clone()
    {
        return new MapListPoint()
        {
            spawns = spawns.ToArray().ToList(),
            map = map.Clone()
        };
    }
}

[Serializable]
public class ListIntMap
{
    public int[] data;
    public int col => data[0];
    public int row => data[1];

    public int[,] Map()
    {
        var map = new int[col, row];
        map.Populate(-1);
        for (var i = 2; i < data.Length; i+=2)
            map.SetAtPoint(i.ToPoint(col),data[i+1]);
        return map;
    }
    
    public bool[,] MapBool()
    {
        var map = new bool[col, row];
        map.Populate(false);
        for (var i = 2; i < data.Length; i+=2)
            map.SetAtPoint(data[i].ToPoint(col),true);
        return map;
    }

    public ListIntMap(IEnumerable<int> input) => data = input.ToArray();
}

[Serializable]
public class ListInt
{
    public bool active;
    public List<int> value;
}



[Serializable]
public class MapListPointCollection
{
    public List<MapListPoint> maps = new List<MapListPoint>();
}