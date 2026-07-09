using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMove : object
{
    public Point From;
    public Point To;
    public PointMoveType MoveType;
    public float Delay;

    public PointMove(Point to, PointMoveType moveType = PointMoveType.Instant)
    {
        To = to;
        MoveType = moveType;
    }

    public PointMove()
    {

    }
}
