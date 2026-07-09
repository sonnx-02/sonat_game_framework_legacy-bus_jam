using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class BezierTarget
{
    public Vector3 start0;
    public Vector3 relativeNext0; 
    public Vector3 relativePre1; 
    public Vector3 pos1; 
    public Vector3 relativeNext1; 
    public Vector3 relativePre2; 
    
    public abstract Vector3 TargetPos { get; }

    public BezierPathController GetPath() => GetPath(start0);
    public BezierPathController GetPath(Vector3 overrideStart0)
    {
        var pathOriginal = new BezierPathController(
            new List<BezierPointVector3>
            {
                new BezierPointVector3(Vector3.zero,overrideStart0,relativeNext0),
                new BezierPointVector3(relativePre1,pos1,relativeNext1),
                new BezierPointVector3(relativePre2,TargetPos,Vector3.zero),
            });
        pathOriginal.SetUp(10, false);
        return pathOriginal;
    }
    
}

[Serializable]
public class BezierTargetPos2 : BezierTarget
{
    public Vector3 pos2;
    public override Vector3 TargetPos => pos2;
}

[Serializable]
public class BezierTargetTransform : BezierTarget
{
    public RectTransform parent; 
    public Transform target;
                                                                                                                                                        
    public override Vector3 TargetPos => parent.InverseTransformPoint(target.position);
}


[Serializable]
public class BezierPointVector3
{
    public Vector3 point;
    public Vector3 preAnchor;
    public Vector3 nextAnchor;

    public string ToStr()
    {
        return $"{point} {preAnchor} {nextAnchor}";
    }

    public BezierPointVector3()
    {
        
    }
    
    public BezierPointVector3( Vector3 pre,Vector3 pos,Vector3 next)
    {
        point = pos;
        preAnchor = pre;
        nextAnchor = next;
    }

    public static float EstimateArcLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var chord = (p3 - p0).magnitude;
        var contNet = (p0 - p1).magnitude + (p2 - p1).magnitude + (p3 - p2).magnitude;
        var appArcLength = (contNet + chord) / 2;
        return appArcLength;
    }

    public static float EstimateArcLength(BezierPointVector3 p0, BezierPointVector3 p1)
    {
        return EstimateArcLength(p0.point, p0.point + p0.nextAnchor, p1.point + p1.preAnchor, p1.point);
    }

    public BezierPointVector3 AddPos(Vector3 localPoint)
    {
      return new BezierPointVector3()
      {
          point = point+localPoint,
          nextAnchor = nextAnchor,
          preAnchor = preAnchor,
      };
    }
}


public class BezierPathController : BasePathController
{
    public BezierPathController(IList<BezierPointVector3> points) : base(points)
    {
    }
    
    public List<Vector3> GizmoLine()
    {
        var gizmoPath = new List<Vector3>();
        for (var i = 0; i < points.Count; i++)
        {
            if (i > 0 && (Math.Abs(points[i].preAnchor.x) > 0.001f || Math.Abs(points[i].preAnchor.y) > 0.001f))
            {
                gizmoPath.Add(points[i].point);
                gizmoPath.Add(points[i].preAnchor + points[i].point);
            }
            if (i < points.Count - 1 && (Math.Abs(points[i].nextAnchor.x) > 0.001f || Math.Abs(points[i].nextAnchor.y) > 0.001f))
            {
                gizmoPath.Add(points[i].point);
                gizmoPath.Add(points[i].nextAnchor + points[i].point);
            }
        }

        return gizmoPath;
    }

    public override void GizmoPath(out IList<Vector3> gizmoPath, out float length, int segment)
    {
        gizmoPath = new List<Vector3>();
        gizmoPath.Add(points[0].point);
        for (var i = 0; i < points.Count - 1; i++)
        {
            for (int j = 1; j < segment; j++)
            {
                gizmoPath.Add(GetPosition(i, j, segment));
            }
        }

        length = gizmoPath.Magnitude2();
    }

    public List<Vector3> GizmoPoints()
    {
        var gizmoPath = new List<Vector3>();
        for (var i = 0; i < points.Count; i++)
        {
            if (i > 0 && (Math.Abs(points[i].preAnchor.x) > 0.001f || Math.Abs(points[i].preAnchor.y) > 0.001f))
            {
                gizmoPath.Add(points[i].point);
                gizmoPath.Add(points[i].preAnchor + points[i].point);
            }

            if (i < points.Count - 1 &&
                (Math.Abs(points[i].nextAnchor.x) > 0.001f || Math.Abs(points[i].nextAnchor.y) > 0.001f))
            {
                gizmoPath.Add(points[i].point);
                gizmoPath.Add(points[i].nextAnchor + points[i].point);
            }
        }

        return gizmoPath;
    }

    public override Vector3 GetPosition(int index1, float elapseTime, float duration)
    {
        Vector3 a = points[index1].point;
        Vector3 b = points[index1].point + points[index1].nextAnchor;
        Vector3 c = points[index1 + 1].point + points[index1 + 1].preAnchor;
        Vector3 d = points[index1 + 1].point;
        float t = elapseTime / duration;
        float oneMinusT = 1f - t;
        Vector3 q = oneMinusT * a + t * b;
        Vector3 r = oneMinusT * b + t * c;
        Vector3 s = oneMinusT * c + t * d;
        Vector3 p = oneMinusT * q + t * r;
        Vector3 T = oneMinusT * r + t * s;
        Vector3 u = oneMinusT * p + t * T;
        return u;
    }

    public float GetLength2(int i)
    {
        return GetLength(i);
    }

    protected override float GetLength(int pathIndex)
    {
        var gizmoPath = new List<Vector3>();
        for (int j = 1; j < 200; j++)
        {
            gizmoPath.Add(GetPosition(pathIndex, j, 200));
        }

        return gizmoPath.Magnitude2();
    }

    private Vector3[] _positions;

    public override void SetUp(float speed, bool rotate1, int segmentDividePerOne = 10)
    {
        rotate = rotate1;
        deltaTime = 0;
        totalMoveTime = 0;
        index = 0;

        int[] subSegmentCount = new int[Count - 1];
        lengths = new float[Count - 1];


        for (var i = 0; i < Count - 1; i++)
        {
            lengths[i] = BezierPointVector3.EstimateArcLength(points[i], points[i + 1]);
            subSegmentCount[i] = Mathf.CeilToInt(lengths[i] * segmentDividePerOne);
        }

        _positions = new Vector3[subSegmentCount.Sum() + 1];
        accumulatedTime = new float[_positions.Length];
        segmentTime = new float[_positions.Length];

        _positions[0] = GetPosition(0, 0, 1);
        accumulatedTime[0] = 0;
        totalMoveTime = 0;
        int count = 1;
        float[] tempLength = new float[_positions.Length];
        for (var i = 0; i < subSegmentCount.Length; i++)
        {
            for (var j = 0; j < subSegmentCount[i]; j++)
            {
                _positions[count] = GetPosition(i, j + 1, subSegmentCount[i]);
                tempLength[count] = (_positions[count] - _positions[count - 1]).magnitude;
                count++;
            }
        }

        var totalLength = tempLength.Sum();
        var totalTime = totalLength / speed;

        for (var i = 1; i < tempLength.Length; i++)
        {
            float subSegmentTime = (tempLength[i] / totalLength) * totalTime;

            totalMoveTime += subSegmentTime;
            accumulatedTime[i] = totalMoveTime;
            segmentTime[i] = subSegmentTime;
        }

        //WaveController._gizmoPath = _positions.ToList();

        if (rotate)
        {
            _lastPosition = _positions[0];
            zDegree =
                Mathf.Atan2(_positions[1].y - _lastPosition.y, _positions[1].x - _lastPosition.x) * Mathf.Rad2Deg + 90;
        }
    }

    //public float zDegree;
    private Vector3 _lastPosition;

    public override Vector3 AddTime(float deltaTime1)
    {
        deltaTime += deltaTime1;
        while (index < segmentTime.Length - 1 && deltaTime >= accumulatedTime[index])
            index++;

        var currentPos = Vector3.Lerp(_positions[Mathf.Max(index - 1, 0)], _positions[index],
            (deltaTime - accumulatedTime[index - 1]) / segmentTime[index]);

        if (rotate)
        {
            zDegree = Mathf.Atan2(_positions[index].y - _positions[index - 1].y,
                          _positions[index].x - _positions[index - 1].x) * Mathf.Rad2Deg + 90;
            _lastPosition = currentPos;
        }

        return currentPos;
    }

    public override void StartAtPosition(Vector3 position)
    {
        var min = float.MaxValue;
        for (var i = 0; i < _positions.Length; i++)
        {
            var distance = (_positions[i] - position).Magnitude2();
            if (distance < min)
            {
                index = i;
                min = distance;
            }
        }

        if (index < _positions.Length - 2)
            index++;

        deltaTime = accumulatedTime[index - 1];
    }

    public override Vector3 GetAtPosAtT(float t)
    {
        deltaTime = 0;
        index = 0;
        return AddTime(t * totalMoveTime);
    }
}