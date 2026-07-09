using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LogicMap3D
{
    public Vector2Int size;
    public List<PointParameterBase> mapData = new List<PointParameterBase>();

    // cached
    // update
    private bool[,] _currentMap;
    private int[,] _currentBoundary;
    private Vector3Int[,] _currentMergeBoundary;

    public Vector3 CenteredPos { get; private set; }
    public Vector2 ZeroPosition { get; private set; }
    public float Distance { get; private set; }


    public bool[,] CurrentMap => _currentMap;
    public int[,] CurrentBoundary => _currentBoundary;
    public Vector3Int[,] CurrentMergeBoundary => _currentMergeBoundary;


    public List<PointParameterBase> MapData => mapData;

  

    public Action onMapSizeChanged;
    public Action onMapChanged;


    public void SetDistance(float d)
    {
        Distance = d;
        RecalculateMapPosition();
        onMapSizeChanged?.Invoke();
    }
    
    public void RecalculateMapPosition()
    {
        CenteredPos = PointGetPosition.GetBoardAlignCenteredPosition(size.x, size.y, Distance, Distance)
            .ToXz();
        ZeroPosition = -CenteredPos.GetXz() + new Vector2(Distance / 2, Distance / 2);
        RecalculateMap();
        onMapSizeChanged?.Invoke();
    }
    
    public void RecalculateMap()
    {
        _mergeBoundaryMap = GetBoundaryMap().GetMergeMap();
        onMapChanged?.Invoke();
    }

    public bool AddDistinct(PointParameterBase point)
    {
        foreach (var p in mapData)
        {
            if (point == p && p.value == point.value && p.value2 == point.value2 && p.value3 == point.value3 &&
                p.value4 == point.value4)
                return false;
        }

        mapData.Add(point);
        RecalculateMap();
        return true;
    }

    public bool Remove(Point point)
    {
        if (mapData.RemoveAll(x => x == point) > 0)
        {
            RecalculateMap();
            return true;
        }
        return false;
    }
    
    public bool Exist(Point p)
    {
        return mapData.Any(x => x == p);
    }

    public bool RemoveAt(PointParameterBase point)
    {
        bool changed = false;
        for (var i = mapData.Count - 1; i >= 0; i--)
        {
            if (mapData[i] == point)
            {
                mapData.RemoveAt(i);
                changed = true;
            }
        }

        if (changed)
        {
            RecalculateMap();
            return true;
        }

        return false;
    }

    public void ClearMap()
    {
        mapData.Clear();
        RecalculateMapPosition();
        RecalculateMap();
    }

    public bool[,] GetMap()
    {
        var map = new bool[size.x, size.y];
        foreach (var p in mapData)
        {
            map.SetByPointDirection(p, true, (PointDirection) p.value, p.value2);
        }

        return map;
    }

    private Vector3Int[,] _mergeBoundaryMap;
    public Vector3Int[,] MergeBoundaryMap => _mergeBoundaryMap;

    public int[,] GetBoundaryMap()
    {
        return GetMap().GetMaskMap(true);
    }


    public Vector3 GetPosXz(Point point)
    {
        var posX = point.col * Distance;
        var posY = point.row * Distance;
        return new Vector3(posX+ (size.x % 2 == 0 ? Distance / 2 : 0), 0, posY+ (size.x % 2 == 0 ? Distance / 2 : 0));
    }
    
    public Vector3 GetPosXz(Point point,int divided)
    {
        var posX = point.col * Distance;
        var posY = point.row * Distance;
        return new Vector3(posX/divided + (size.x % 2 == 0 ? Distance / 2 : 0), 0, posY/divided + (size.x % 2 == 0 ? Distance / 2 : 0));
    }
    
//    public static PointParameterBase GetRootPointFromDivided(PointParameterBase divided,int divide)
//    {
//        var clone = divided.CloneCurrentBase();
//        clone.col = Mathf.FloorToInt(divided.col *1f/ divide);
//        clone.row = Mathf.FloorToInt(divided.row *1f/ divide);
//        if (divided.col % divide > divide / 2f)
//            clone.col++;
//        if (divided.row % divide > divide / 2f)
//            clone.row++;
//        return clone;
//    }

//    public static Point GetRootPointFromDivided(Point divided, int divide)
//    {
//        var clone = divided.Clone();
//        clone.col = Mathf.FloorToInt(divided.col * 1f / divide);
//        clone.row = Mathf.FloorToInt(divided.row * 1f / divide);
//        if (divided.col % divide > divide / 2f)
//            clone.col++;
//        if (divided.row % divide > divide / 2f)
//            clone.row++;
//        return clone;
//    }
}