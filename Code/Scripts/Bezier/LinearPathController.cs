using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinearPathController : BasePathController
{
    public LinearPathController(IList<BezierPointVector3> points) : base(points)
    {
      
    }

    public override void GizmoPath(out IList<Vector3> gizmoPath, out float length, int segment)
    {
        gizmoPath = points.Select(x => x.point).ToList();
        length = gizmoPath.Magnitude2();
    }

    public override Vector3 GetPosition(int index1, float elapseTime, float duration)
    {
        return Vector3.Lerp(points[index1].point, points[index1 + 1].point, elapseTime / duration);
    }

    public override void StartAtPosition(Vector3 position)
    {

        //int d = 0;
        //var min = float.MaxValue;
        //float distance = 0;

        //int split = 200;

        //var startPos = transform.position;
        //for (int i = 0; i < split; i++)
        //{
        //    var pos = path.AddTime(1f / split * path.TotalMoveTime);
        //    distance = (pos - startPos).Magnitude2();
        //    if (distance < min)
        //    {
        //        d = i;
        //        min = distance;
        //    }
        //}
        //path.ResetTimer();
        //Vector3 firstPoint = path.AddTime(d * 1f / split * path.TotalMoveTime);

        throw new NotImplementedException();
    }

    protected override float GetLength(int pathIndex)
    {
        return (points[pathIndex].point - points[pathIndex + 1].point).Magnitude2();
    }
}
