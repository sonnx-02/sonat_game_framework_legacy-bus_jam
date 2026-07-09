using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


// ReSharper disable InconsistentNaming
[Serializable]
public class CustomParameter
{
    public int intValue;
    public int intValue2;
    public int intValue3;
    public bool boolValue;
    public bool boolValue2;

    public CustomParameter Clone() => new CustomParameter()
    {
        intValue = intValue,
        intValue2 = intValue2,
        intValue3 = intValue3,
        boolValue = boolValue,
        boolValue2 = boolValue2,
    };

    public Vector3Int vector3
    {
        get => new Vector3Int(intValue, intValue2, intValue3);
        set
        {
            intValue = value.x;
            intValue2 = value.y;
            intValue3 = value.z;
        }
    }
    
    public Vector2Int vector2
    {
        get => new Vector2Int(intValue, intValue2);
        set
        {
            intValue = value.x;
            intValue2 = value.y;
        }
    }
}


[Serializable]
public class PointPoint : Point
{
    public int col2;
    public int row2;

//    public int col1;
//    public int row1;
    
    public Point Point1 => this;

//    private Point _point2;
//    public Point Point2
//    {
//        get
//        {
//            return new Point(col2, row2);
//        }
//    }
//    
    public Point Point2Get => new Point(col2, row2);
   
}

[Serializable]
public class PointPointParameter : PointPoint
{
    public int value;
    public int value2;
    public int value3;
    public int value4;

    public PointPointParameter CloneCurrent()
    {
        return new PointPointParameter()
        {
            col = col,
            row = row,
            col2 = col2,
            row2 = row2,
            value = value,
            value2 = value2,
            value3 = value3,
            value4 = value4
        };
    }
}

[Serializable]
public class Point3 : Point
{
    public int forward;
    
    public Point3(int col,int row,int forward)
    {
        base.col = col;
        base.row = row;
        this.forward = forward;
    }
    
    public Point3 Clone2() => new Point3(col,row,forward);
}

[Serializable]
public partial class Point
{
    public static int TotalCols = 5;
    public static int TotalRows = 5;

    [FormerlySerializedAs("x")]
    public int col;
    [FormerlySerializedAs("y")]
    public int row;

    
    public int Sum => col + row;

    public static Point Zero => new Point(0, 0);
    public static Point MinusOne => new Point(-1, -1);
    public static Point One => new Point(1, 1);

    public static Point Left => new Point(-1, 0);

    public static Point Right => new Point(1, 0);

    public static Point Bottom => new Point(0, -1);

    public static Point Top => new Point(0, 1);
    public static Point TopRight => new Point(1, 1);
    public static Point BottomRight => new Point(1, -1);
    public static Point BottomLeft => new Point(-1, -1);
    public static Point TopLeft => new Point(-1, 1);

    public static Point[] ListAdjacent => new[]
    {
        Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, TopLeft
    };
    
    public static Point[] FourBaseDirection = {
        Top, Right, Bottom, Left
    };


    public int Step()
    {
        return Mathf.Max(Mathf.Abs(col), Mathf.Abs(row));
    }

    public static Point Create(int col, int row)
    {
        return new Point(col,row);
    }
    
    public static Point Create(Vector2Int vec)
    {
        return new Point(vec.x,vec.y);
    }

    public static Point WorldToDirectionPoint(Vector2 pos1, Vector2 pos2, Vector2 clamp)
    {
        var sub = pos1 - pos2;
        int a = 0;
        if (!Mathf.Approximately(sub.x, 0f))
        {
            a = Mathf.RoundToInt(sub.x / clamp.x);
        }

        int b = 0;
        if (!Mathf.Approximately(sub.y, 0f))
        {
            b = Mathf.RoundToInt(sub.y / clamp.y);
        }

        return new Point(a, b);
    }

    /// <summary>
    /// left top right bottom
    /// </summary>
    /// <returns></returns>
    public int ToFourDirectionIndex()
    {
        if (col == -1 && row == 0)
            return 0;
        if (col == 1 && row == 0)
            return 2;
        if (col == 0 && row == 1)
            return 1;
        if (col == 0 && row == -1)
            return 3;

        return -1;
    }

    public bool IsInRange(int rangeFrom, int rangeTo)
    {
        return col >= rangeFrom && col <= rangeTo && row >= rangeFrom && row <= rangeTo;
    }
    
    public bool IsInRange(Vector2Int vec)
    {
        return col >= 0 && col < vec.x && row >= 0 && row < vec.y;
    }

    public bool IsInRange(int unit)
    {
        return col >= 0 && col < unit && row >= 0 && row < unit;
    }


    public Point()
    {
    }

    public Point(int col, int row)
    {
        this.col = col;
        this.row = row;
    }

    public int GetRandomBetween()
    {
        return UnityEngine.Random.Range(col, row);
    }
    
    public static Point FromVector2Int(Vector2Int vec) => new Point(vec.x,vec.y);

    public void Print()
    {
        Debug.Log("Point: " + col + "/" + row);
    }
    
    public int MaxStepDistance(IList<Point> others,PointType pointType)
    {
        int step = 0;
        foreach (var other in others)
        {       
            var temp = GetWorldToRelative(pointType, other)
                .Magnitude(pointType);
            if (temp > step)
                step = temp;
        }

        return step;
    }
    
    public int MinRange(IList<Point> others,PointType pointType)
    {
        int step = int.MaxValue;
        foreach (var other in others)
        {       
            var temp = GetWorldToRelative(pointType, other)
                .Range(pointType);
            if (temp < step)
                step = temp;
        }
        return step;
    }

    public override bool Equals(object other)
    {
        return (other != null) && (this.col == (other as Point).col && this.row == (other as Point).row);
    }

    public static bool operator ==(Point emp1, Point emp2)
    {
        if (object.ReferenceEquals(emp1, emp2)) return true;
        if (object.ReferenceEquals(emp1, null)) return false;
        if (object.ReferenceEquals(emp2, null)) return false;

        return emp1.Equals(emp2);
    }
    
 

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator !=(Point emp1, Point emp2)
    {
        if (object.ReferenceEquals(emp1, emp2)) return false;
        if (object.ReferenceEquals(emp1, null)) return true;
        if (object.ReferenceEquals(emp2, null)) return true;

        return !emp1.Equals(emp2);
    }

    public static Point operator -(Point emp1, Point emp2)
    {
        return new Point(emp1.col - emp2.col, emp1.row - emp2.row);
    }

    public static Point operator +(Point emp1, Point emp2)
    {
        return new Point(emp1.col + emp2.col, emp1.row + emp2.row);
    }

    /// <summary>
    /// 1 == top, 2 == right, 4 == bottom, 8 == left
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static PointDirection GetDirection(Point from, Point to)
    {
        if (to == null)
            return PointDirection.None;
        var point = to - from;

        if (point.col == 0 && point.row == 1)
            return PointDirection.Top;
        else if (point.col == 0 && point.row == -1)
            return PointDirection.Bottom;
        else if (point.col == 1 && point.row == 0)
            return PointDirection.Right;
        else if (point.col == -1 && point.row == 0)
            return PointDirection.Left;
        else
            return PointDirection.None;
    }

    public string toString()
    {
        return col.ToString() + "/" + row.ToString();
    }

  
    public int Magnitude(PointType type)
    {
        switch (type)
        {
            case PointType.Square:
                return Math.Abs(col) + Math.Abs(row);
            case PointType.HexaHorizontal:
                var abscol = col < 0 && Mathf.Abs(row) % 2 == 1 ? Mathf.Abs(col) + 1 : Mathf.Abs(col);
                return Mathf.Abs(row) + Mathf.Max(0, abscol - (Mathf.Abs(row) + 1) / 2);
            case PointType.HexaVertical:
                var absrow = row < 0 && Mathf.Abs(col) % 2 == 1 ? Mathf.Abs(row) + 1 : Mathf.Abs(row);
                return Mathf.Abs(col) + Mathf.Max(0, absrow - (Mathf.Abs(col) + 1) / 2);
            case PointType.Triangle:
                break;
            case PointType.HexaHorizontal2:
                var abscol2 = col < 0 && Mathf.Abs(row) % 2 == 1 ? Mathf.Abs(col) + 1 : Mathf.Abs(col);
                return Mathf.Abs(row) + Mathf.Max(0, abscol2 - (Mathf.Abs(row) + 1) / 2);
                break;
            case PointType.HexaVertical2:
                var absrow2 = row < 0 && Mathf.Abs(col) % 2 == 1 ? Mathf.Abs(row) + 1 : Mathf.Abs(row);
                return Mathf.Abs(col) + Mathf.Max(0, absrow2 - (Mathf.Abs(col) + 1) / 2);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return -1;
    }
    
    public int Range(PointType type)
    {
        switch (type)
        {
            case PointType.Square:
                return (Math.Abs(col) > Math.Abs(row) ? Math.Abs(col) : Math.Abs(row));
            case PointType.HexaHorizontal:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
            case PointType.HexaVertical:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
            case PointType.Triangle:
                break;
            case PointType.HexaHorizontal2:
                break;
            case PointType.HexaVertical2:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return -1;
    }
    
    
    public int StepDistance(Point other)
    {
        return Math.Abs(this.col - other.col) + Math.Abs(this.row - other.row);
    }

    public int StepDistance(int col1, int row1)
    {
        return Math.Abs(this.col - col1) + Math.Abs(this.row - row1);
    }


    public int WorldDistance(Point other)
    {
        return Mathf.Max(Math.Abs(this.col - other.col), Math.Abs(this.row - other.row));
    }

    public int WorldDistance(int col1, int row1)
    {
        return Mathf.Max(Math.Abs(this.col - col1), Math.Abs(this.row - row1));
    }
    


    public int SubAsBitmask(Point other)
    {
        var col1 = other.col - this.col;
        var row1 = other.row - this.row;

        if (col1 == 0 && row1 == 1)
            return 4;
        if (col1 == 0 && row1 == -1)
            return 1;
        if (col1 == 1 && row1 == 0)
            return 2;
        if (col1 == -1 && row1 == 0)
            return 8;
        return 0;
    }


    // clone
    public Point Clone()
    {
        return new Point
        {
            col = col,
            row = row,
        };
    }

    public Point Clone(int addcol, int addrow)
    {
        return new Point(col + addcol, row + addrow);
    }

    public Point Clone(Point addPoint)
    {
        return new Point(col + addPoint.col, row + addPoint.row);
    }
    
    // Around Points
    
    public IEnumerable<Point> GetAroundPoints<T>(T[,] map, Func<T, bool> predicate,PointType pointType)
    {
        int sizeX = map.GetLength(0);
        int sizeY = map.GetLength(1);
        var list = OneStepAroundRelativePoints(pointType);
        foreach (var relativePoint in list)
        {
            var world = GetRelativeToWorld(relativePoint, pointType);
            if (world.col >= 0 && world.col < sizeX && world.row >= 0 && world.row < sizeY
                && predicate(map[world.col, world.row]))
                yield return world;
        }
    }
    
    public int StepDistance(Point other,PointType pointType)
    {
        var step = GetWorldToRelative(pointType, other)
            .Magnitude(pointType);

        return step;
    }


    public IEnumerable<Point> GetAroundPoints<T>(T[,] map,PointType pointType = PointType.Square)
    {
        return GetAroundPoints(map.GetLength(0), map.GetLength(1),pointType);
    }

    public IEnumerable<Point> GetAroundPoints(int size,PointType pointType = PointType.Square)
    {
        return GetAroundPoints(size, size,pointType);
    }

    
    public IEnumerable<Point> GetAroundPoints(bool[,] ignore,PointType pointType = PointType.Square)
    {
        int sizeX = ignore.GetLength(0);
        int sizeY = ignore.GetLength(1);
        var list = OneStepAroundRelativePoints(pointType);
        foreach (var relativePoint in list)
        {
            var world = GetRelativeToWorld(relativePoint, pointType);
            if (world.col >= 0 && world.col < sizeX && world.row >= 0 && world.row < sizeY
                && !ignore[world.col,world.row])
                yield return world;
        }
    }

    public IEnumerable<Point> GetAroundPoints<T>(T[,] map, bool[,] ignore,PointType pointType = PointType.Square)
    {
        int sizeX = map.GetLength(0);
        int sizeY = map.GetLength(1);
        var list = OneStepAroundRelativePoints(pointType);
        foreach (var relativePoint in list)
        {
            var world = GetRelativeToWorld(relativePoint, pointType);
            if (world.col >= 0 && world.col < sizeX && world.row >= 0 && world.row < sizeY
                && !ignore[world.col,world.row])
                yield return world;
        }
    }

    
    public IEnumerable<Point> GetAroundPoints(int sizeX, int sizeY,PointType pointType)
    {
        var list = OneStepAroundRelativePoints(pointType);
        foreach (var relativePoint in list)
        {
            var world = GetRelativeToWorld(relativePoint, pointType);
            if (world.col >= 0 && world.col < sizeX && world.row >= 0 && world.row < sizeY)
                yield return world;
        }
    }
    
    public static Point[] OneStepAroundRelativePoints(PointType pointType)
    {
        
            switch (pointType)
            {
                case PointType.Square:
                    return SquareAroundPoints;
                case PointType.HexaHorizontal:
                    return HorizontalHexaAroundPoints;
                case PointType.HexaVertical:
                    return VerticalAroundPoints;
                case PointType.Triangle:
                    return TriangleAroundPoints;
                case PointType.HexaHorizontal2:
                    return HorizontalHexaAroundPoints;
                case PointType.HexaVertical2:
                    return VerticalAroundPoints;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }

    static readonly Point[] HorizontalHexaAroundPoints =
    {
        Right,
        BottomRight,
        Bottom,
        Left,
        Top,
        TopRight,
    };

    static readonly Point[] TriangleAroundPoints =
    {
        Right,
        BottomRight,
        Bottom,
        Left,
        Top,
        TopRight,
    };

    static readonly Point[] SquareAroundPoints =
    {
        Right,
        Bottom,
        Left,
        Top,
    };

    static readonly Point[] VerticalAroundPoints =
    {
        Right,
        BottomRight,
        Bottom,
        Left,
        Top,
        TopRight,
    };


    
    public IEnumerable<Point> GetAdjacents<T>(T[,] map, Func<T, bool> predicate)
    {
        foreach (var point in GetAdjacents(map))
            if (predicate(map[point.col, point.row]))
                yield return point;
    }


    public IEnumerable<Point> GetAdjacents<T>(T[,] map)
    {
        return GetAdjacents(map.GetLength(0), map.GetLength(1));
    }

    public IEnumerable<Point> GetAdjacents(int size)
    {
        return GetAdjacents(size, size);
    }

    public IEnumerable<Point> GetAdjacents(int sizeX, int sizeY)
    {
        if (col > 0)
            yield return Clone(-1, 0);
        if (col < sizeX - 1)
            yield return Clone(1, 0);
        if (row > 0)
            yield return Clone(0, -1);
        if (row < sizeY - 1)
            yield return Clone(0, 1);
    }

    public IEnumerable<Point> GetAdjacents<T>(T[,] map, bool[,] ignore)
    {
        foreach (var point in GetAdjacents(map))
            if (!ignore[point.col, point.row])
                yield return point;
    }

    public IEnumerable<Point> GetAroundPoints(int sizeX, int sizeY)
    {
        if (col > 0)
        {
            yield return Clone(-1, 0);
            if (row > 0)
                yield return Clone(-1, -1);
        }

        if (col < sizeX - 1)
        {
            yield return Clone(1, 0);
            if (row > 0)
                yield return Clone(1, -1);
        }

        if (row > 0)
        {
            yield return Clone(0, -1);
            if (col < sizeX - 1)
                yield return Clone(1, -1);
        }

        if (row < sizeY - 1)
        {
            yield return Clone(0, 1);
            if (col < sizeX - 1)
                yield return Clone(1, 1);
        }
    }
}