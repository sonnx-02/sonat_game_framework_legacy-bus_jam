using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PointSetting
{
    public PointType pointType;
    public Vector2 distance;
    public Vector2 adjust;
}

public static class PointGetPosition
{
    public static Vector3 GetBoardCenteredPosition<T>(PointType pointType, T[,] map, float distanceX, float distanceY)
    {
        return GetBoardCenteredPosition(pointType, map.GetLength(0), map.GetLength(1), distanceX, distanceY);
    }

    public static Vector3 GetBoardCenteredPosition(PointType pointType, Vector2Int size, float distanceX,
        float distanceY)
    {
        return GetBoardCenteredPosition(pointType, size.x, size.y, distanceX, distanceY);
    }
    
    public static Vector3 GetBoardCenteredPosition(PointType pointType, Vector2Int size, Vector2 distance)
    {
        return GetBoardCenteredPosition(pointType, size.x, size.y, distance.x, distance.y);
    }

    public static Vector3 GetBoardCenteredPosition(PointSetting setting, Vector2Int size)
    {
        return GetBoardCenteredPosition(setting.pointType, size.x, size.y, setting.distance.x, setting.distance.y);
    }

    public static Vector3 GetBoardCenteredPosition(PointType pointType, int totalCols, int totalRows, float distanceX,
        float distanceY)
    {
        var maxX = new Point(Mathf.Max(totalCols - 1, 0), 0).GetPos(pointType, distanceX).x;
        var maxY = new Point(0, Mathf.Max(totalRows - 1, 0)).GetPos(pointType, distanceY).y;
        return new Vector3(-maxX / 2, -maxY / 2);
    }
    
    public static Vector3 GetBoardAlignCenteredPosition(int totalCols, int totalRows, float distanceX,
        float distanceY)
    {
        var maxX = new Point(Mathf.Max(totalCols - 1, 0), 0).GetPos(PointType.Square, distanceX).x;
        var maxY = new Point(0, Mathf.Max(totalRows - 1, 0)).GetPos(PointType.Square, distanceY).y;
        return new Vector3(-maxX / 2 + (totalCols % 2 == 0 ? -0.5f*distanceX : 0), -maxY / 2 + (totalRows % 2 == 0 ? -0.5f*distanceY : 0));
    }
}

public partial class Point
{
    public static float PointSize = 1;

    public Vector3 GetDefaultPos(PointType pointType)
    {
        switch (pointType)
        {
            case PointType.Square:
                return GetPos();
            case PointType.HexaHorizontal:
                return GetPosHexaHorizontal();
            case PointType.HexaVertical:
                return GetPosHexaVertical();
            case PointType.Triangle:
                return GetPosTriangle();
            case PointType.HexaHorizontal2:
                return GetPosHexaHorizontal2();
            case PointType.HexaVertical2:
                return GetPosHexaVertical2();
            default:
                Debug.LogError("Not implement for this PointType : " + pointType);
                return Vector3.zero;
        }
    }

    public Vector3 GetPos(PointType pointType, float distance)
    {
        switch (pointType)
        {
            case PointType.Square:
                return GetPos(distance);
            case PointType.HexaHorizontal:
                return GetPosHexaHorizontal(distance);
            case PointType.HexaVertical:
                return GetPosHexaVertical(distance);
            case PointType.Triangle:
                return GetPosTriangle(distance);
            case PointType.HexaHorizontal2:
                return GetPosHexaHorizontal2(distance);
            case PointType.HexaVertical2:
                return GetPosHexaVertical2(distance);
            default:
                Debug.LogError("Not implement for this PointType : " + pointType);
                return Vector3.zero;
        }
    }

    public Vector3 GetPos(PointSetting pointSetting)
    {
        switch (pointSetting.pointType)
        {
            case PointType.Square:
                return GetPos(pointSetting.distance.x, pointSetting.distance.y);
            case PointType.HexaHorizontal:
                return GetPosHexaHorizontal(pointSetting.distance.x);
            case PointType.HexaVertical:
                return GetPosHexaVertical(pointSetting.distance.x);
            case PointType.Triangle:
                return GetPosTriangle(pointSetting.distance.x);
            case PointType.HexaHorizontal2:
                return GetPosHexaHorizontal2(pointSetting.distance.x);
            case PointType.HexaVertical2:
                return GetPosHexaVertical2(pointSetting.distance.x);
            default:
                Debug.LogError("Not implement for this PointType : " + pointSetting.pointType);
                return Vector3.zero;
        }
    }

    public Vector3 GetPosXz(PointType pointType, float distance)
    {
        switch (pointType)
        {
            case PointType.Square:
                return GetPosXz(distance);
            case PointType.HexaHorizontal:
                return GetPosXZHexaHorizontal(distance);
            case PointType.HexaVertical:
                return GetPosXZHexaVertical(distance);
            case PointType.Triangle:
                return GetPosTriangle(distance);
            case PointType.HexaHorizontal2:
                return GetPosXZHexaHorizontal2(distance);
            case PointType.HexaVertical2:
                return GetPosXZHexaVertical2(distance);
            default:
                Debug.LogError("Not implement for this PointType : " + pointType);
                return Vector3.zero;
        }
    }
    
    public Vector3 GetPosXz(PointType pointType, float distance,float add)
    {
        switch (pointType)
        {
            case PointType.Square:
                return GetPosXz(distance) + new Vector3(distance*add,0,distance*add);
            case PointType.HexaHorizontal:
                return GetPosXZHexaHorizontal(distance)+ new Vector3(distance*add,0,distance*add);
            case PointType.HexaVertical:
                return GetPosXZHexaVertical(distance)+ new Vector3(distance*add,0,distance*add);
            case PointType.Triangle:
                return GetPosTriangle(distance)+ new Vector3(distance*add,0,distance*add);
            case PointType.HexaHorizontal2:
                return GetPosXZHexaHorizontal2(distance)+ new Vector3(distance*add,0,distance*add);
            case PointType.HexaVertical2:
                return GetPosXZHexaVertical2(distance)+ new Vector3(distance*add,0,distance*add);
            default:
                Debug.LogError("Not implement for this PointType : " + pointType);
                return Vector3.zero;
        }
    }

    // Square
    public Vector3 GetPos()
    {
//        return GetPos(PointSize,Gap);
        Vector3 localPos = Vector3.zero;
        localPos.x = ((float) -TotalCols / 2 + col) * PointSize + PointSize / 2;
        localPos.y = ((float) -TotalRows / 2 + row) * PointSize + PointSize / 2;
        return localPos;
    }

    private Vector3 GetPos(float distance)
    {
        return new Vector2(col * distance, row * distance);
    }

    public Vector3 GetPos(float distanceX, float distanceY)
    {
        return new Vector2(col * distanceX, row * distanceY);
    }

    private Vector3 GetPosXz(float distance)
    {
        return new Vector3(col * distance, 0, row * distance);
    }

    // Hexa

    #region Hexa

    public const float HexaRate = 0.86604f;

    private Vector3 GetPosHexaHorizontal()
    {
        return GetPosHexaHorizontal(PointSize);
    }

    private Vector3 GetPosHexaVertical()
    {
        return GetPosHexaVertical(PointSize);
    }
    
    private Vector3 GetPosHexaHorizontal2()
    {
        return GetPosHexaHorizontal2(PointSize);
    }

    private Vector3 GetPosHexaVertical2()
    {
        return GetPosHexaVertical2(PointSize);
    }

    private Vector3 GetPosHexaHorizontal(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * distance;
        localPos.y = row * (HexaRate * distance);
        if (Mathf.Abs(row) % 2 == 1)
            localPos.x += distance / 2;

        return localPos;
    }
    
    private Vector3 GetPosHexaHorizontal2(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * distance;
        localPos.y = row * (HexaRate * distance);
        if (Mathf.Abs(row) % 2 == 1)
            localPos.x -= distance / 2;

        return localPos;
    }


    private Vector3 GetPosXZHexaHorizontal(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * distance;
        localPos.z = row * (HexaRate * distance);
        if (Mathf.Abs(row) % 2 == 1)
            localPos.x += distance / 2;

        return localPos;
    }
    
    private Vector3 GetPosXZHexaHorizontal2(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * distance;
        localPos.z = row * (HexaRate * distance);
        if (Mathf.Abs(row) % 2 == 1)
            localPos.x -= distance / 2;

        return localPos;
    }


    private Vector3 GetPosHexaVertical(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * (HexaRate * distance);
        localPos.y = row * distance;
        if (Mathf.Abs(col) % 2 == 1)
            localPos.y += distance / 2;

        return localPos;
    }
    
    private Vector3 GetPosHexaVertical2(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * (HexaRate * distance);
        localPos.y = row * distance;
        if (Mathf.Abs(col) % 2 == 1)
            localPos.y -= distance / 2;

        return localPos;
    }

    private Vector3 GetPosXZHexaVertical(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * (HexaRate * distance);
        localPos.z = row * distance;
        if (Mathf.Abs(col) % 2 == 1)
            localPos.z += distance / 2;

        return localPos;
    }
    
    private Vector3 GetPosXZHexaVertical2(float distance)
    {
        Vector3 localPos = Vector3.zero;
        localPos.x = col * (HexaRate * distance);
        localPos.z = row * distance;
        if (Mathf.Abs(col) % 2 == 1)
            localPos.z -= distance / 2;

        return localPos;
    }


    #endregion

    // Triangle 

    #region Triangle

    public static Point GetTriangleRelativePoint(Point rootPoint, Point secondPoint)
    {
        var col = secondPoint.col - rootPoint.col;
        var row = secondPoint.row - rootPoint.row;
        if (rootPoint.IsEven())
        {
        }

        return new Point(col, row);
    }

    public Vector3 GetPosTriangle()
    {
        return GetPosTriangle(PointSize);
    }

    public Vector3 GetPosTriangle(float distance)
    {
        return new Vector2(col * distance * 0.495f, row * distance * 0.863f);
        ;
    }

    #endregion

    public Point GetWorldToRelative(PointType pointType, Point worldPoint)
    {
        var relative = worldPoint - this;

        switch (pointType)
        {
            case PointType.Square:
                break;
            case PointType.HexaHorizontal:
                if (row % 2 == 0)
                {
                    if (relative.row % 2 != 0)
                        return relative.Clone(1, 0);
                }

                break;
            case PointType.HexaVertical:
                if (col % 2 == 0)
                {
                    if (relative.col % 2 != 0)
                        return relative.Clone(0, 1);
                }

                break;
            case PointType.Triangle:
                break;
            case PointType.HexaHorizontal2:
                if (row % 2 == 0)
                {
                    if (relative.row % 2 != 0)
                        return relative.Clone(1, 0);
                }
                break;
            case PointType.HexaVertical2:
                if (col % 2 == 0)
                {
                    if (relative.col % 2 != 0)
                        return relative.Clone(0, 1);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pointType), pointType, null);
        }


        return relative;
    }

    public Point GetRelativeToWorld(Point relativePoint, PointType pointType)
    {
        var world = this + relativePoint;

        switch (pointType)
        {
            case PointType.Square:
                break;
            case PointType.HexaHorizontal:
                if (row % 2 == 0)
                {
                    if (relativePoint.row % 2 != 0)
                        return world.Clone(-1, 0);
                }
                break;
            case PointType.HexaVertical:
                if (col % 2 == 0)
                {
                    if (relativePoint.col % 2 != 0)
                        return world.Clone(0, -1);
                }

                break;
            case PointType.Triangle:
                break;
            case PointType.HexaHorizontal2:
                if (row % 2 == 0)
                {
                    if (relativePoint.row % 2 != 0)
                        return world.Clone(-1, 0);
                }
                break;
            case PointType.HexaVertical2:
                if (col % 2 == 0)
                {
                    if (relativePoint.col % 2 != 0)
                        return world.Clone(0, -1);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pointType), pointType, null);
        }


        return world;
    }
}