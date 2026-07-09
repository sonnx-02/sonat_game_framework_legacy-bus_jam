using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CatmullRomPathController : BasePathController
{

    public CatmullRomPathController(IList<BezierPointVector3> points) : base(points)
    {

    }

    public override void GizmoPath(out IList<Vector3> gizmoPath, out float length, int segment)
    {
        gizmoPath = NewCatmullRom(points.Select(x => x.point).ToArray(), segment, false).ToArray();
        length = gizmoPath.Magnitude2();
    }

    public override Vector3 GetPosition(int index1, float elapseTime, float duration)
    {
        return CatmullRom(
            index1 > 0 ? points[index1 - 1].point : points[index1].point,
            points[index1].point,
            points[index1 + 1].point,
            index1 < points.Count - 2 ? points[index1 + 2].point : points[index1 + 1].point,
            elapseTime, duration);
    }

    public override void StartAtPosition(Vector3 position)
    {
        throw new NotImplementedException();
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

    private static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop)
    {
        return NewCatmullRom<Vector3>(points, Identity, slices, loop);
    }

    private delegate Vector3 ToVector3<T>(T v);

    static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop)
    {
        if (nodes.Count >= 2)
        {
            yield return toVector3((T)nodes[0]);

            int last = nodes.Count - 1;
            for (int current = 0; loop || current < last; current++)
            {
                if (loop && current > last)
                {
                    current = 0;
                }
                int previous = (current == 0) ? ((loop) ? last : current) : current - 1;
                int start = current;
                int end = (current == last) ? ((loop) ? 0 : current) : current + 1;
                int next = (end == last) ? ((loop) ? 0 : end) : end + 1;

                int stepCount = slices + 1;
                for (int step = 1; step <= stepCount; step++)
                {
                    yield return CatmullRom(toVector3((T)nodes[previous]),
                                     toVector3((T)nodes[start]),
                                     toVector3((T)nodes[end]),
                                     toVector3((T)nodes[next]),
                                     step, stepCount);
                }
            }
        }
    }

    static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next,
        float elapsedTime, float duration)
    {
        float percentComplete = elapsedTime / duration;
        float percentCompleteSquared = percentComplete * percentComplete;
        float percentCompleteCubed = percentCompleteSquared * percentComplete;

        return previous * (-0.5f * percentCompleteCubed +
                           percentCompleteSquared -
                           0.5f * percentComplete) +
               start * (1.5f * percentCompleteCubed +
                        -2.5f * percentCompleteSquared + 1.0f) +
               end * (-1.5f * percentCompleteCubed +
                      2.0f * percentCompleteSquared +
                      0.5f * percentComplete) +
               next * (0.5f * percentCompleteCubed -
                       0.5f * percentCompleteSquared);
    }

    static Vector3 Identity(Vector3 v)
    {
        return v;
    }
}



