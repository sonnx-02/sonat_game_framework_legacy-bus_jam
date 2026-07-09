using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public static class MathematicsExtensions
{
    public static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    public static float GetProjectileAngle(int i, float angle, int nShots)
    {
        if (angle >= -0.1f && angle <= 0.1f)
        {
            return i * 360f / nShots;
        }

        return (nShots > 1) ? (i - ((nShots - 1) / 2f)) * (angle / (nShots - 1)) : 0;
    }

    public static Vector3 GetBezierPosition(float t, Vector3 point1, Vector3 followingControl1, Vector3 preceedControl2,
        Vector3 point2)
    {
        t = Mathf.Clamp(t, 0, 1);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * point1 +
            3f * oneMinusT * oneMinusT * t * followingControl1 +
            3f * oneMinusT * t * t * preceedControl2 +
            t * t * t * point2;
    }
    
    
    public static float GetPingPong(this float t)
    {
        if (t <= 0.5f)
            return t * 2;
        return Mathf.Max(1 - (t - 0.5f)*2,0);
    }

    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        Vector3 a = target - current;
        float magnitude = Magnitude2(a);
        if (magnitude <= maxDistanceDelta || magnitude <= 0.001f)
            return target;
        return current + a / magnitude * maxDistanceDelta;
    }

    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta, float minPosition)
    {
        Vector3 a = target - current;
        float magnitude = Magnitude2(a);
        if (magnitude <= maxDistanceDelta || magnitude <= 0.001f)
            return target;

        var returnPos = current + a / magnitude * maxDistanceDelta;
        if (Magnitude2(returnPos, target) > minPosition)
            returnPos = current + a / magnitude * (Magnitude2(returnPos, target) - minPosition);
        return returnPos;
    }

    public static float Evaluate(float t, Keyframe keyframe0, Keyframe keyframe1)
    {
        float dt = keyframe1.time - keyframe0.time;

        float m0 = keyframe0.outTangent * dt;
        float m1 = keyframe1.inTangent * dt;

        float t2 = t * t;
        float t3 = t2 * t;

        float a = 2 * t3 - 3 * t2 + 1;
        float b = t3 - 2 * t2 + t;
        float c = t3 - t2;
        float d = -2 * t3 + 3 * t2;

        return a * keyframe0.value + b * m0 + c * m1 + d * keyframe1.value;
    }

    public static float Magnitude2(this Vector2 thisVector2)
    {
        return Mathf.Sqrt(thisVector2.x * thisVector2.x + thisVector2.y * thisVector2.y);
    }

    public static float Magnitude2(Vector2 float1, Vector2 vector2)
    {
        var distance = vector2 - float1;
        return (float) Math.Sqrt(distance.x * distance.x + distance.y * distance.y);
    }

    public static float Magnitude2(this Vector3 thisVector2)
    {
        return (float) Math.Sqrt(thisVector2.x * thisVector2.x + thisVector2.y * thisVector2.y);
    }

    public static float Magnitude2(Vector3 float1, Vector3 vector2)
    {
        var distance = vector2 - float1;
        return (float) Math.Sqrt(distance.x * distance.x + distance.y * distance.y);
    }

    public static float Magnitude2(this IList<Vector3> points)
    {
        float sum = 0;
        for (var i = 1; i < points.Count; i++)
        {
            sum += (points[i] - points[i - 1]).Magnitude2();
        }

        return sum;
    }


    public static float GetInterval(this IList<Vector3> points, Action<Vector3, float, float> action)
    {
        float sum = points.Magnitude2();
        float temp = 0;
        action(points[0], temp, temp / sum);
        for (var i = 1; i < points.Count; i++)
        {
            var distance = (points[i] - points[i - 1]).Magnitude2();
            temp += distance;
            action(points[i], temp, temp / sum);
        }

        return sum;
    }

    public static Vector3 GetNearestPosition<T>(Vector3 root, IList<T> enemyPositions) where T : MonoBehaviour
    {
        var nearest = enemyPositions[0].transform.position;
        var min = Magnitude2(root, enemyPositions[0].transform.position);
        for (int i = 1; i < enemyPositions.Count; i++)
        {
            var temp = Magnitude2(root, enemyPositions[i].transform.position);
            if (temp < min)
            {
                nearest = enemyPositions[i].transform.position;
                min = temp;
            }
        }

        return nearest;
    }


    public static Quaternion GetQuaternion(Vector3 from, Vector3 to)
    {
        Vector3 distance = to - from;
        var quaternion = Quaternion.LookRotation(distance, Vector3.forward);
        quaternion.x = 0;
        quaternion.y = 0;
        return Quaternion.Euler(quaternion.eulerAngles);
    }

    public static Quaternion GetQuaternion(Vector3 from, Vector3 to, Vector3 upward)
    {
        Vector3 distance = to - from;
        var quaternion = Quaternion.LookRotation(distance, upward);
        quaternion.x = 0;
        quaternion.y = 0;
        return Quaternion.Euler(quaternion.eulerAngles);
    }

    public static Vector2 GetNearestPosition(Vector2 root, IEnumerable<Vector2> positions)
    {
        float minDistance = int.MaxValue;
        Vector2 nearest = root;
        foreach (var pos in positions)
        {
            var distance = Vector2.Distance(root, pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = pos;
            }
        }

        return nearest;
    }
}