using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointDirectionMove
{
    protected Point Point;
    protected PointDirection Direction;
    protected bool Reverse;

    float _curveAnchorRate = -0.05f;
    Vector2 _startPos;
    Vector2 _endPos;
    Vector2 _curvePos;

    public static readonly float[] SpeedRate = new[]
    {
        1f, // 0 NONE
        0.5f, // 1 TOP 
        0.5f, // 2 RIGHT
        0.9f, // 3 TOP_RIGHT
        0.5f, // 4 BOTTOM
        1f, // 5 VERTICAL
        0.9f, // 6  BOTTOM RIGHT
        1F, // 7
        0.5F,// 8 LEFT
        0.9f, // 9 TOP_LEFT
        1F, // 10 HORIZONAL
        1F, // 11
        0.9f, // 12 BOTTOM LEFT
    };

    public enum DirectionType
    {
        NONE,
        HALF,
        STRAIGHT,
        TURN,
        UNDEFINED
    }

    public static readonly DirectionType[] DirectionTypePerBitmask = new[]
    {
        DirectionType.NONE, // 0 NONE
        DirectionType.HALF, // 1 TOP 
        DirectionType.HALF, // 2 RIGHT
        DirectionType.TURN, // 3 TOP_RIGHT
        DirectionType.HALF, // 4 BOTTOM
        DirectionType.STRAIGHT, // 5 VERTICAL
        DirectionType.TURN, // 6  BOTTOM RIGHT
        DirectionType.NONE, // 7
        DirectionType.HALF,// 8 LEFT
        DirectionType.TURN, // 9 TOP_LEFT
        DirectionType.STRAIGHT, // 10 HORIZONAL
        DirectionType.UNDEFINED, // 11
        DirectionType.TURN, // 12 BOTTOM LEFT
        DirectionType.UNDEFINED, // 13 BOTTOM LEFT
        DirectionType.UNDEFINED, // 14 BOTTOM LEFT
        DirectionType.UNDEFINED, // 15 BOTTOM LEFT
    };

    public float GetSpeedRate()
    {
        if (DirectionTypePerBitmask[(int)Direction] == DirectionType.HALF)
            return 0.5f;
        else if (DirectionTypePerBitmask[(int)Direction] == DirectionType.TURN)
            return 0.9f;
        else
            return 1f;
    }

    public PointDirection GetDirection()
    {
        return Direction;
    }

    public bool GetReverse()
    {
        return Reverse;
    }


    public PointDirectionMove(Point point, PointDirection direction, bool reverse = false, float curveAnchorRate = -0.05f)
    {
        Set(point, direction, reverse, curveAnchorRate);
    }

    public void Set(Point point, PointDirection direction, bool reverse = false, float curveAnchorRate = -0.05f)
    {
        Point = point;
        Direction = direction;
        Reverse = reverse;

        _curveAnchorRate = curveAnchorRate;
        _curvePos = point.GetPos();
        if (Direction == PointDirection.Bottom)
        {
            _startPos = Point.GetPos();
            _endPos = Point.GetPos() + new Vector3(0, -Point.PointSize / 2);
        }
        if (Direction == PointDirection.Top)
        {
            _startPos = Point.GetPos();
            _endPos = Point.GetPos() + new Vector3(0, Point.PointSize / 2);
        }

        if (Direction == PointDirection.Right)
        {
            _startPos = Point.GetPos();
            _endPos = Point.GetPos() + new Vector3(Point.PointSize / 2, 0);
        }
        if (Direction == PointDirection.Left)
        {
            _startPos = Point.GetPos();
            _endPos = Point.GetPos() + new Vector3(-Point.PointSize / 2, 0);
        }

        if (Direction == PointDirection.Horizontal)
        {
            _startPos = Point.GetPos() + new Vector3(-Point.PointSize / 2, 0);
            _endPos = Point.GetPos() + new Vector3(Point.PointSize / 2, 0);
        }

        if (Direction == PointDirection.Vertical)
        {
            _startPos = Point.GetPos() + new Vector3(0, -Point.PointSize / 2);
            _endPos = Point.GetPos() + new Vector3(0, Point.PointSize / 2);
        }

        if (Direction == PointDirection.TopRight)
        {
            _startPos = Point.GetPos() + new Vector3(0, Point.PointSize / 2);
            _endPos = Point.GetPos() + new Vector3(Point.PointSize / 2, 0);
            _curvePos += new Vector2(Point.PointSize * _curveAnchorRate, Point.PointSize * _curveAnchorRate);
        }

        if (Direction == PointDirection.TopLeft)
        {
            _startPos = Point.GetPos() + new Vector3(0, Point.PointSize / 2);
            _endPos = Point.GetPos() + new Vector3(-Point.PointSize / 2, 0);
            _curvePos += new Vector2(-Point.PointSize * _curveAnchorRate, Point.PointSize * _curveAnchorRate);
        }

        if (Direction == PointDirection.BottomRight)
        {
            _startPos = Point.GetPos() + new Vector3(0, -Point.PointSize / 2);
            _endPos = Point.GetPos() + new Vector3(Point.PointSize / 2, 0);
            _curvePos += new Vector2(Point.PointSize * _curveAnchorRate, -Point.PointSize * _curveAnchorRate);
        }

        if (Direction == PointDirection.BottomLeft)
        {
            _startPos = Point.GetPos() + new Vector3(0, -Point.PointSize / 2);
            _endPos = Point.GetPos() + new Vector3(-Point.PointSize / 2, 0);

            _curvePos += new Vector2(-Point.PointSize * _curveAnchorRate, -Point.PointSize * _curveAnchorRate);
        }
    }

    public Vector3 GetPos(float interval)
    {
        if (Direction == PointDirection.Top || Direction == PointDirection.Bottom || Direction == PointDirection.Left ||
            Direction == PointDirection.Right)
        {
            return Vector3.Lerp(_startPos, _endPos, Reverse ? 1 - interval : interval);
        }
        if (Direction == PointDirection.Horizontal || Direction == PointDirection.Vertical)
        {
            return Vector3.Lerp(_startPos, _endPos, Reverse ? 1 - interval : interval);
        }
        if (Direction == PointDirection.TopRight || Direction == PointDirection.TopLeft
            || Direction == PointDirection.BottomRight || Direction == PointDirection.BottomLeft)
        {
            return Bezier(_startPos, _curvePos, _endPos, Reverse ? 1 - interval : interval);
        }

        return Vector3.zero;
    }

    public Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        var ab = Vector2.Lerp(a, b, t);
        var bc = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(ab, bc, t);
    }
}
