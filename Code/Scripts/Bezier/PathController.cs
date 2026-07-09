using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPath
{
    void GizmoPath(out IList<Vector3> gizmoPath, out float length, int segment);
    Vector3 GetPosition(int index1, float elapseTime, float duration);
}


public abstract class BasePathController : IPath
{
    public readonly IList<BezierPointVector3> points;

    protected BasePathController(IList<BezierPointVector3> points)
    {
        this.points = points;
    }

    private Vector3 GetPositionAtPoint(int pointIndex)
    {
        return points[pointIndex].point;
    }

    protected int Count => points.Count;

    public abstract void GizmoPath(out IList<Vector3> gizmoPath, out float length, int segment);
    
    public float totalMoveTime;
    protected float[] lengths;
    protected float[] segmentTime;
    protected float[] accumulatedTime;
    protected float deltaTime;
    protected int index;
    protected bool rotate;

    public abstract Vector3 GetPosition(int index1, float elapseTime, float duration);

    public virtual void SetUp(float speed,bool rotate1,int segmentDividePerOne = 10)
    {
        rotate = rotate1;
        deltaTime = 0;
        index = 0;
        lengths = new float[Count - 1];
        segmentTime = new float[Count - 1];
        accumulatedTime = new float[Count - 1];
        totalMoveTime = 0;
        for (var i = 0; i < Count - 1; i++)
        {
            lengths[i] = GetLength(i);
            segmentTime[i] = lengths[i] / speed;
            totalMoveTime += segmentTime[i];
            accumulatedTime[i] = totalMoveTime;
        }

        if (segmentTime.Length > 0)
        {
            var currentPos = GetPosition(0, segmentTime[0] / 10, segmentTime[0]);
            if (rotate)
            {
                _lastPosition = GetPositionAtPoint(0);
                zDegree = Mathf.Atan2(currentPos.y - _lastPosition.y, currentPos.x - _lastPosition.x) * Mathf.Rad2Deg + 90;
            }
        }
    }

    public float zDegree;
    private Vector3 _lastPosition;
    public virtual Vector3 AddTime(float deltaTime1)
    {
        this.deltaTime += deltaTime1;
        while (index < segmentTime.Length - 1 && this.deltaTime > accumulatedTime[index])
            index++;

        var currentPos =  GetPosition(index, (index == 0 ? this.deltaTime : this.deltaTime - accumulatedTime[index - 1]), segmentTime[index]);

        if (rotate)
        {
            zDegree = Mathf.Atan2(currentPos.y - _lastPosition.y, currentPos.x - _lastPosition.x) * Mathf.Rad2Deg + 90;
            _lastPosition = currentPos;
        }

        return currentPos;
    }

    public abstract void StartAtPosition(Vector3 position);


    public virtual Vector3 GetAtPosAtT(float t)
    {
        if (t >= 1)
            return points[points.Count - 1].point;
        var convertedTime = t * totalMoveTime;

        int i = 0;
        while (i < segmentTime.Length - 1 && convertedTime > accumulatedTime[i])
            i++;

        return GetPosition(i, (i == 0 ? convertedTime : convertedTime - accumulatedTime[i-1]), segmentTime[i]);
    }

    public virtual Vector3 RepeatTimer()
    {
        deltaTime -= totalMoveTime;
        index = 0;
        return AddTime(0);
    }

    public virtual Vector3 ResetTimer()
    {
        deltaTime = 0;
        index = 0;
        return AddTime(0);
    }

    protected abstract float GetLength(int pathIndex);

    public virtual bool IsFinish()
    {
        return deltaTime >= totalMoveTime;
    }
}

