using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;
using static BitwiseExtensions;
using Object = System.Object;
using Random = System.Random;


public static class DuongQualityChecking
{
#if UNITY_EDITOR
    private static readonly Dictionary<string, float> Timing = new Dictionary<string, float>();
#endif
    public static void CheckDuplicateCall(string function)
    {
#if UNITY_EDITOR
        if (Timing.ContainsKey(function) && Time.time - Timing[function] <= 0.01f)
        {
            Debug.LogError("Duplicate invoke " + function);
            Debug.Break();
        }
        else
        {
            Timing[function] = Time.time;
        }
#endif
    }
}

public static class CameraExtension
{
    public static void FitTo(this Camera cam, float width,float height,float min,float heightScaleFactor= 1f)
    {
        var ratio = cam.pixelWidth * 2f / cam.pixelHeight;
        cam.orthographicSize = Mathf.Max(width / ratio > height/2f / heightScaleFactor ? width / ratio  : height/2f / heightScaleFactor ,min);
    }


    public static bool HitPositionCastFrom(this Camera cam, Vector3 worldPosition, Collider hitCollier,out Vector3 point)
    {
        var viewPort = cam.WorldToViewportPoint(worldPosition);
        var x = cam.ViewportToScreenPoint(viewPort);
        Ray ray = cam.ScreenPointToRay(x);
        bool hitPlane = (hitCollier.Raycast(ray, out RaycastHit hit, 1000));
        point = Vector3.zero;
        if (hitPlane)
        {
            point = hit.point;
        }

        return hitPlane;
    }
}

public static class ReflectionExtension
{
    public static WaitForSeconds ToWaitForSeconds(this float time) => new WaitForSeconds(time);

    public static T GetPrivateFieldValue<T>(this Type type, string field, object obj)
    {
        return (T) type
            .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.GetValue(obj);
    }

    public static void SetPrivateFieldValue<T>(this Type type, string field, object obj, T value)
    {
        type
            .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(obj, value);
    }


    public static MethodInfo GetPrivateMethod(this Type type, string field)
    {
        return type
            .GetMethod(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    }


    public static Type FindTypeInDomain(string typeName, string nameSpace)
    {
        Type type = Type.GetType(typeName);

        if (type != null)
            return type;

        if (!string.IsNullOrEmpty(nameSpace))
        {
            type = Type.GetType(nameSpace + "." + typeName);
            if (type == null)
                type = GetType(nameSpace, typeName);
        }


        return type != null ? type : null;
    }

    public static Type FindTypeInDomain(BuiltInEnumType typeName, string nameSpace)
    {
        return FindTypeInDomain(typeName.ToString(), nameSpace);
    }


    private static Type GetType(string nameSpace, string className)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var type2 = Type.GetType($"{nameSpace}.{className},{assembly.GetName().Name}");
            if (type2 != null)
                return type2;
        }

        foreach (var assembly in assemblies)
        {
            var type2 = Type.GetType($"{className},{assembly.GetName().Name}");
            if (type2 != null)
                return type2;
        }

        return null;
    }

    public static int GetIntValue(this MonoBehaviour go, string pName)
    {
        var getField = go.GetType().GetField(pName);
        if (getField != null)
            return (int) getField.GetValue(go);

        var getProperty = go.GetType().GetProperty(pName);
        if (getProperty != null)
            return (int) getProperty.GetValue(go);

        Debug.LogError("not found" + pName + " in" + go, go);
        return int.MinValue;
    }

    public static int GetIntValue(this ScriptableObject go, string pName)
    {
        var getField = go.GetType().GetField(pName);
        if (getField != null)
            return (int) getField.GetValue(go);

        var getProperty = go.GetType().GetProperty(pName);
        if (getProperty != null)
            return (int) getProperty.GetValue(go);

        Debug.LogError($"not found {pName} in {go}", go);
        return int.MinValue;
    }

    public static int[] GetIntValues(this MonoBehaviour go, string pName)
    {
        var getField = go.GetType().GetField(pName);
        if (getField != null)
            return (int[]) getField.GetValue(go);

        var getProperty = go.GetType().GetProperty(pName);
        if (getProperty != null)
            return (int[]) getProperty.GetValue(go);

        Debug.LogError("not found" + pName + " in" + go, go);
        return null;
    }

    public static int[] GetIntValues(this ScriptableObject go, string pName)
    {
        var getField = go.GetType().GetField(pName);
        if (getField != null)
            return (int[]) getField.GetValue(go);

        var getProperty = go.GetType().GetProperty(pName);
        if (getProperty != null)
            return (int[]) getProperty.GetValue(go);

        Debug.LogError("not found" + go, go);
        return null;
    }
}

public static class MyVectorExtensions
{
    public static Vector3 Angle(Vector2 vec1)
    {
        var d = -vec1;
        var theta = Mathf.Atan2(d.y, d.x);
        theta *= 180 / Mathf.PI; // rads to degs, range (-180, 180]
        //if (theta < 0) theta = 360 + theta; // range [0, 360)
        return new Vector3(0, 0, theta);
    }
    
    public static Vector3 Snap(this Vector3 vec,int snap,float distance)
    {
        var divide = distance / snap;
        return new Vector3(Mathf.RoundToInt(vec.x/divide)*divide,Mathf.RoundToInt(vec.y/divide)*divide,Mathf.RoundToInt(vec.z/divide)*divide);
    }

    public static Vector3 Angle(this Vector3 vector)
    {
        var d = -vector;
        var theta = Mathf.Atan2(d.y, d.x);
        theta *= 180 / Mathf.PI; // rads to degs, range (-180, 180]
        //if (theta < 0) theta = 360 + theta; // range [0, 360)
        return new Vector3(0, 0, theta);
    }

    public static Vector3 CenterOfVectors(this List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Count == 0)
            return sum;

        foreach (Vector3 vec in vectors)
            sum += vec;

        return sum / vectors.Count;
    }
     

    public static Vector3 ToXz(this Vector3 vectors)
    {
        return new Vector3(vectors.x, 0, vectors.y);
    }
    
    public static Vector2 GetXz(this Vector3 vectors)
    {
        return new Vector3(vectors.x,  vectors.z);
    }


    public static float GetLength(this IList<Vector2> list)
    {
        float sum = 0;
        for (var i = 1; i < list.Count; i++)
        {
            sum += Vector2.Distance(list[i - 1], list[i]);
        }

        return sum;
    }
    
    public static float GetLength(this IList<Vector3> path)
    {
        float length = 0;
        for (var i = 1; i < path.Count; i++)
            length += Vector3.Distance(path[i], path[i - 1]);
        return length;
    }

    public static float GetLength2(this IList<Vector2> list)
    {
        float sum = 0;
        for (var i = 1; i < list.Count; i++)
            sum += Vector2.Distance(list[i - 1], list[i]);

        return sum;
    }

    public static Vector3 FlipX(this Vector3 vec) => new Vector3(-vec.x, vec.y, vec.z);
    public static Vector3 FlipY(this Vector3 vec) => new Vector3(vec.x, -vec.y, vec.z);
    public static Vector3 FlipZ(this Vector3 vec) => new Vector3(vec.x, vec.y, -vec.z);
    public static Vector3 FlipXy(this Vector3 vec) => new Vector3(-vec.x, -vec.y, vec.z);
}

public static class RotateExtensions
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

    public static Quaternion GetRotationIso(this Vector3 vec)
    {
        return  Quaternion.LookRotation(vec.ToIso(), Vector3.up);
    }

    public static bool IsFrontOf(this Transform transform,Vector3 pos)
    {
       return Vector3.Dot(Vector3.forward, transform.InverseTransformPoint(pos)) > 0;
    }
    
    public static bool IsFrontOf(this Transform transform,Vector3 pos,Vector3 forward)
    {
        return Vector3.Dot(forward, transform.InverseTransformPoint(pos)) > 0;
    }

    
    public static Quaternion GetRotationIsoFromXy(this Vector3 vec)
    {
        return GetRotationIso(vec.ToXz());
    }

    public static Quaternion GetRotationIsoFromVec2Input(this Vector2 vec)
    {
        return GetRotationIso(new Vector3(vec.x,0,vec.y));
    }

    
    public static int RandomToInt(this float n)
    {
        var floor = Mathf.FloorToInt(n);
        var deci = n - floor;

        return floor + (UnityEngine.Random.Range(0, 1000) <= deci * 1000 ? 1 : 0);
    }

    public static Quaternion GetQuaternionToNearest<T>(Vector2 root, IEnumerable<T> positions, Quaternion current,
        float limitAngle = 360, float addDegree = -90) where T : MonoBehaviour
    {
        bool find = false;
        float minDistance = int.MaxValue;
        Vector2 nearest = root;

        var angleZ = current.eulerAngles.z;
        foreach (var pos in positions)
        {
            var distance = Vector2.Distance(root, pos.transform.position);
            if (distance < minDistance && (limitAngle >= 360 || limitAngle <= 0 ||
                                           GetDistanceRotateEulerZ(root, pos.transform.position, angleZ, addDegree) <
                                           limitAngle / 2))
            {
                find = true;
                minDistance = distance;
                nearest = pos.transform.position;
            }
        }

        if (find)
            return GetRotateQuaternion(root, nearest, addDegree);
        return current;
    }

    public static float GetDistanceZ(Quaternion quaternion1, Quaternion quaternion2)
    {
        var z = (quaternion2 * quaternion1).eulerAngles.z;
        if (z > 180)
            z = 360 - z;

        return z;
    }


    public static Vector2 GetRandomRangeVector2(float range)
    {
        return new Vector2(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
    }

    public static Vector2 GetRandomRangeVector2(float range1, float range2)
    {
        var negativeX = UnityEngine.Random.Range(0, 2) % 2 == 0;
        var negativeY = UnityEngine.Random.Range(0, 2) % 2 == 0;

        var x = negativeX ? -UnityEngine.Random.Range(range1, range2) : UnityEngine.Random.Range(range1, range2);
        var y = negativeY ? -UnityEngine.Random.Range(range1, range2) : UnityEngine.Random.Range(range1, range2);

        return new Vector2(x, y);
    }

    public static IEnumerable<T> TakeUniqueByWeight<T>(this IList<T> arrs, int[] weights, int take)
    {
        if (arrs.Count == 1)
            yield return arrs[0];
        if (take >= arrs.Count)
            Debug.LogError("take > arrs.Count");

        for (int j = 0; j < take; j++)
        {
            int totalWeight = 0;
            int[] weightValues = new int[arrs.Count];
            for (var i = 0; i < arrs.Count; i++)
            {
                totalWeight += weights[i];
                weightValues[i] = totalWeight;
            }

            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            for (int i = 0; i < weightValues.Length; i++)
                if (randomValue < weightValues[i])
                {
                    yield return arrs[i];
                    weights[i] = 0;
                    break;
                }
        }
    }


    public static Product TakeByWeight(this IList<Product> arrs)
    {
        if (arrs.Count == 1)
            return arrs[0];

        int totalWeight = 0;
        int[] weightValues = new int[arrs.Count];
        for (var i = 0; i < arrs.Count; i++)
        {
            totalWeight += arrs[i].parameter;
            weightValues[i] = totalWeight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < weightValues.Length; i++)
            if (randomValue < weightValues[i])
                return arrs[i];

        Debug.LogError("Error:GetByWeight");
        return arrs[-1];
    }

    public static void GetNearestPositions2<T>(Vector2 root, IList<T> positions, Quaternion current,
        out T found, out Vector2 nearestPosition, out Quaternion quaternion,
        float limitAngle = 360, float addDegree = -90) where T : MonoBehaviour
    {
        found = null;
        float minDistance = int.MaxValue;
        nearestPosition = root;

        var angleZ = current.eulerAngles.z;
        foreach (var pos in positions)
        {
            var distance = Vector2.Distance(root, pos.transform.position);
            var z = GetDistanceRotateEulerZ(root, pos.transform.position, angleZ, addDegree);
            if (distance < minDistance && (limitAngle >= 360 || limitAngle <= 0 || z < limitAngle / 2))
            {
                found = pos;
                minDistance = distance;
                nearestPosition = pos.transform.position;
            }
        }

        quaternion = GetRotateQuaternion(root, nearestPosition, addDegree);
    }

    public static float GetDistanceRotateEulerZ(Vector3 current, Vector3 target, float currentZ, float addDegree = -90)
    {
        var targetZ = GetRotateEulerZ(current, target);

        var deltaZ = Mathf.Abs(targetZ - currentZ);
        if (deltaZ > 180)
            deltaZ = 360 - deltaZ;

        return deltaZ;
    }

    public static float GetRotateEulerZ(Vector3 current, Vector3 target, float addDegree = -90)
    {
        Vector3 relativeVector = target - current;
        return Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg + addDegree;
    }

    public static float GetRotateEulerZ(Vector3 relativeVector, float addDegree = -90)
    {
        return Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg + addDegree;
    }

    public static void RotateTo(Transform rotateTransform, Vector3 targetPosition, float maxDegree, Vector3 upward)
    {
        var targetRotation = GetRotateQuaternion(rotateTransform.position, targetPosition);
        rotateTransform.rotation = GetRotateTo(rotateTransform.rotation, targetRotation, maxDegree);
    }
    
    public static Quaternion MirrorOnXAxis(this Quaternion q) {
        q.y = -q.y;
        q.z = -q.z;
        return q;
    }
    
    public static  Vector2 RotateVector2(Vector2 vec, float angle)
    {
        float newAngle = Mathf.Atan2(vec.y, vec.x) + angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
    }
    
    public static  Vector3 RotatePointByVector(Vector3 toRotate, Vector3 directionFrom,Vector3 directionTo)
    {
        return  GetRotate(directionFrom,directionTo) * Vector3.forward;
    }
    
    public static Quaternion GetRotate(Vector3 directionFrom,Vector3 directionTo)
    {
        var normalizedVector = (directionTo - directionFrom).normalized;
        var rotation = Quaternion.LookRotation(normalizedVector);
        return  rotation ;
    }
    
    public static Quaternion GetRotate(this Vector3 vector)
    {
       return Quaternion.LookRotation(vector.normalized);
    }

    public static Quaternion GetRotateQuaternion(Vector3 current, Vector3 target, float addDegree = -90)
    {
        Vector3 relativeVector = target - current;
        float rotationZ = Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0.0f, 0.0f, rotationZ + addDegree);
    }

    public static Quaternion GetRotateQuaternionY(Vector3 current, Vector3 target, float addDegree = -90)
    {
        Vector3 relativeVector = target - current;
        float rotationZ = Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0.0f, rotationZ + addDegree, 0);
    }

    public static Quaternion GetRotateTo(Quaternion current, Quaternion target, float maxDeltaDegree)
    {
        var quaternion = new Quaternion(0, 0, target.z, target.w);
        return Quaternion.RotateTowards(current, quaternion, maxDeltaDegree);
    }
}


public static class BitwiseExtensions
{
    public static List<Vector2Int> GetPath(this List<Vector2Int> edges, int from, int to, int max)
    {
        bool[] visited = new bool[max];
        visited[from] = true;

        return null;
    }

    public static IList<int> GetPath(this bool[,] matrix, int from, int to)
    {
        var weights = new int[matrix.GetLength(0),matrix.GetLength(1)];
        var xAxis = weights.GetLength(0);
        var yAxis = weights.GetLength(1);
        for (var i = 0; i < xAxis; i++)
            for (var j = 0; j < yAxis; j++)
            {
                if (matrix[i, j])
                    weights[i, j] = 1;
            }
        var result = DijkstrasAlgorithm.Dijkstra(weights, from, false);
        var path = result.GetPath(to);
        return path;
    }
    
    public static List<Vector2Int> CalculateEdges(this int[,] diagram)
    {
        var edges = new List<Vector2Int>();
        int col = diagram.GetLength(0);
        int row = diagram.GetLength(0);
        for (int i = 0; i < col; i++)
        for (int j = 0; j < row; j++)
        {
            // right
            for (int k = i; k < col; k++)
            {
                if ((diagram[k, j] & (int) PointDirection.Right) != 0)
                    edges.Add(new Vector2Int(Point.Create(i, j).ToIndex(col), Point.Create(k, j).ToIndex(col)));
                else
                    break;
            }

            // left
            for (int k = i; k >= 0; k--)
            {
                if ((diagram[k, j] & (int) PointDirection.Left) != 0)
                    edges.Add(new Vector2Int(Point.Create(i, j).ToIndex(col), Point.Create(k, j).ToIndex(col)));
                else
                    break;
            }

            // right
            for (int k = j; k < row; k++)
            {
                if ((diagram[i, k] & (int) PointDirection.Top) != 0)
                    edges.Add(new Vector2Int(Point.Create(i, j).ToIndex(col), Point.Create(i, k).ToIndex(col)));
                else
                    break;
            }

            // left
            for (int k = i; k >= 0; k--)
            {
                if ((diagram[k, j] & (int) PointDirection.Bottom) != 0)
                    edges.Add(new Vector2Int(Point.Create(i, j).ToIndex(col), Point.Create(i, k).ToIndex(col)));
                else
                    break;
            }
        }

        return edges;
    }

    public static bool[,] CalculateEdgesMatrix(this int[,] diagram)
    {
        var size = diagram.GetLength(0) * diagram.GetLength(1);
        var matrix = new bool[size, size];
        int col = diagram.GetLength(0);
        int row = diagram.GetLength(1);
        for (int i = 0; i < col; i++)
        for (int j = 0; j < row; j++)
        {
            // right
            for (int k = i; k < col - 1; k++)
            {
                if ((diagram[k, j] & (int) PointDirection.Right) != 0)
                {
                    var c = Point.Create(i, j).ToIndex(col);
                    var r = Point.Create(k + 1, j).ToIndex(col);
                    SetMatrix(c, r);
                }
                else
                    break;
            }

            // left
            for (int k = i; k >= 1; k--)
            {
                if ((diagram[k, j] & (int) PointDirection.Left) != 0)
                {
                    var c = Point.Create(i, j).ToIndex(col);
                    var r = Point.Create(k - 1, j).ToIndex(col);
                    SetMatrix(c, r);
                }
                else
                    break;
            }

            for (int k = j; k < row - 1; k++)
            {
                if ((diagram[i, k] & (int) PointDirection.Top) != 0)
                {
                    var c = Point.Create(i, j).ToIndex(col);
                    var r = Point.Create(i, k + 1).ToIndex(col);
                    SetMatrix(c, r);
                }
                else
                    break;
            }

            for (int k = j; k >= 1; k--)
            {
                if ((diagram[i, k] & (int) PointDirection.Bottom) != 0)
                {
                    var c = Point.Create(i, j).ToIndex(col);
                    var r = Point.Create(i, k - 1).ToIndex(col);
                    SetMatrix(c, r);
                }
                else
                    break;
            }
        }

        void SetMatrix(int c1, int r1)
        {
            matrix[c1, r1] = true;
            matrix[r1, c1] = true;
        }

        return matrix;
    }


    /// <summary>
    /// ex : block[1,1] = 15 is block at column=1, row=1 is blocked all 4 directions,
    /// this function means merge 2 block-diagram into one,
    /// like if start[5,2] is 14 (can move left,right,bottom) and block[5,2] = 4(bottom) => 10 left-right
    /// and block by limit of size too
    /// </summary>
    /// <param name="startDiagram"></param>
    /// <param name="blockDirectionDiagram"></param>
    public static void PopulateDiagramBlockDirection(ref int[,] startDiagram, int[,] blockDirectionDiagram)
    {
        for (int i = 0; i < startDiagram.GetLength(0); i++)
        for (int j = 0; j < startDiagram.GetLength(1); j++)
        {
            startDiagram[i, j] =
                // ReSharper disable once ShiftExpressionRealShiftCountIsZero
                Convert.ToInt16(j < startDiagram.GetLength(1) - 1 &&
                                (blockDirectionDiagram[i, j] & (int) PointDirection.Top) == 0) <<
                (int) PointDirectionBase.Top |
                Convert.ToInt16(i < startDiagram.GetLength(0) - 1 &&
                                (blockDirectionDiagram[i, j] & (int) PointDirection.Right) == 0) <<
                (int) PointDirectionBase.Right |
                Convert.ToInt16(j > 0 && (blockDirectionDiagram[i, j] & (int) PointDirection.Bottom) == 0) <<
                (int) PointDirectionBase.Bottom |
                Convert.ToInt16(i > 0 && (blockDirectionDiagram[i, j] & (int) PointDirection.Left) == 0) <<
                (int) PointDirectionBase.Left;
        }
    }
    
    public static int[,]  GetStartBlockDiagram(int col,int row)
    {
        int[,] startDiagram = new int[col,row];
        startDiagram.Populate(15);
        for (int i = 0; i < startDiagram.GetLength(0); i++)
        for (int j = 0; j < startDiagram.GetLength(1); j++)
        {
            startDiagram[i, j] =
                // ReSharper disable once ShiftExpressionRealShiftCountIsZero
                Convert.ToInt16(j < startDiagram.GetLength(1) - 1)  <<
                (int) PointDirectionBase.Top |
                Convert.ToInt16(i < startDiagram.GetLength(0) - 1)  <<
                (int) PointDirectionBase.Right |
                Convert.ToInt16(j > 0) <<
                (int) PointDirectionBase.Bottom |
                Convert.ToInt16(i > 0) <<
                (int) PointDirectionBase.Left;
        }

        return startDiagram;
    }

    /// <summary>
    ///  ex : block[1,1] = true(is filled) mean block[0,1] can't go up, block[1,0] can't go right...
    /// </summary>
    /// <param name="startDiagram"></param>
    /// <param name="fill"></param>
    public static void PopulateDiagramBlockFill(ref int[,] startDiagram, bool[,] fill)
    {
        for (int i = 0; i < fill.GetLength(0); i++)
        for (int j = 0; j < fill.GetLength(1); j++)
        {
            if (fill[i, j])
            {
                if (i > 0)
                {
                    startDiagram[i - 1, j] = startDiagram[i - 1, j] & ~(int) PointDirection.Right;
                    startDiagram[i, j] = startDiagram[i, j] & ~(int) PointDirection.Left;
                }

                if (i < fill.GetLength(0) - 1)
                {
                    startDiagram[i + 1, j] = startDiagram[i + 1, j] & ~(int) PointDirection.Left;
                    startDiagram[i, j] = startDiagram[i, j] & ~(int) PointDirection.Right;
                }

                if (j > 0)
                {
                    startDiagram[i, j - 1] = startDiagram[i, j - 1] & ~(int) PointDirection.Top;
                    startDiagram[i, j] = startDiagram[i, j] & ~(int) PointDirection.Bottom;
                }

                if (j < fill.GetLength(1) - 1)
                {
                    startDiagram[i, j + 1] = startDiagram[i, j + 1] & ~(int) PointDirection.Bottom;
                    startDiagram[i, j] = startDiagram[i, j] & ~(int) PointDirection.Top;
                }
            }
        }
    }

    ///  ex : block[1,1] = true(is filled) mean block[0,1] can't go up, block[1,0] can't go right...
    public static int[,] PopulateDiagramBlockFill(this int[,] initDiagram, IEnumerable<Point> allPoints,
        Point rootPoint)
    {
        var populateDiagram = initDiagram.Clone() as int[,];
        // ReSharper disable once PossibleNullReferenceException
        var humanMap = new bool[populateDiagram.GetLength(0), populateDiagram.GetLength(1)];
        humanMap.SetAtPoints(allPoints, true);
        // humanMap.SetAtPoint(rootPoint,false);
        PopulateDiagramBlockFill(ref populateDiagram, humanMap);

        populateDiagram.SetAtPoint(rootPoint, initDiagram.GetAtPoint(rootPoint));
        // if left of root point is empty
        if (rootPoint.col > 0 && humanMap[rootPoint.col - 1, rootPoint.row])
            populateDiagram[rootPoint.col, rootPoint.row] =
                populateDiagram[rootPoint.col, rootPoint.row] & ~ (int) PointDirection.Left;
        if (rootPoint.col < humanMap.GetLength(0) - 1 && humanMap[rootPoint.col + 1, rootPoint.row])
            populateDiagram[rootPoint.col, rootPoint.row] =
                populateDiagram[rootPoint.col, rootPoint.row] & ~ (int) PointDirection.Right;
        if (rootPoint.row > 0 && humanMap[rootPoint.col, rootPoint.row - 1])
            populateDiagram[rootPoint.col, rootPoint.row] =
                populateDiagram[rootPoint.col, rootPoint.row] & ~ (int) PointDirection.Bottom;
        if (rootPoint.row < humanMap.GetLength(1) - 1 && humanMap[rootPoint.col, rootPoint.row + 1])
            populateDiagram[rootPoint.col, rootPoint.row] =
                populateDiagram[rootPoint.col, rootPoint.row] & ~ (int) PointDirection.Top;

        return populateDiagram;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fillDiagram"></param>
    /// <param name="twoWayBlockFlag">mean if top is block, top block is blocked at bottom as well</param>
    /// <returns></returns>
    public static int[,] GetBlockDiagram(this bool[,] fillDiagram, int twoWayBlockFlag = 15)
    {
        var blockDiagram = new int[fillDiagram.GetLength(0), fillDiagram.GetLength(1)];
        for (int i = 0; i < fillDiagram.GetLength(0); i++)
        for (int j = 0; j < fillDiagram.GetLength(1); j++)
        {
            if (fillDiagram[i, j])
            {
                blockDiagram[i, j] |= twoWayBlockFlag;
                if (i < fillDiagram.GetLength(0) - 1 && (blockDiagram[i, j] & (int) PointDirection.Right) != 0)
                    blockDiagram[i + 1, j] |= (int) PointDirection.Left;
                if (j < fillDiagram.GetLength(1) - 1 && (blockDiagram[i, j] & (int) PointDirection.Top) != 0)
                    blockDiagram[i, j + 1] |= (int) PointDirection.Bottom;
                if (i > 0 && (blockDiagram[i, j] & (int) PointDirection.Left) != 0)
                    blockDiagram[i - 1, j] |= (int) PointDirection.Right;
                if (j > 0 && (blockDiagram[i, j] & (int) PointDirection.Bottom) != 0)
                    blockDiagram[i, j - 1] |= (int) PointDirection.Top;
            }
        }

        return blockDiagram;
    }

  

    public static int[,] GetMoveDiagram(this bool[,] fillDiagram)
    {
        var blockDiagram = new int[fillDiagram.GetLength(0), fillDiagram.GetLength(1)];
        for (int i = 0; i < fillDiagram.GetLength(0); i++)
        for (int j = 0; j < fillDiagram.GetLength(1); j++)
            if (fillDiagram[i, j])
                blockDiagram[i, j] = 15;
        
        for (int i = 0; i < fillDiagram.GetLength(0); i++)
        for (int j = 0; j < fillDiagram.GetLength(1); j++)
        {
            if (fillDiagram[i, j])
            {
                if (i >= fillDiagram.GetLength(0) - 1
                    || !fillDiagram[i + 1, j] )
                    blockDiagram[i, j] &= ~(int) PointDirection.Right;
                if (j >= fillDiagram.GetLength(1) - 1 || !fillDiagram[i, j + 1])
                    blockDiagram[i, j] &= ~(int) PointDirection.Top;
                if (i <= 0 || !fillDiagram[i - 1, j])
                    blockDiagram[i, j] &= ~(int) PointDirection.Left;
                if (j <= 0 || !fillDiagram[i, j - 1])
                    blockDiagram[i, j] &= ~(int) PointDirection.Bottom;
            }
        }

        return blockDiagram;
    }
}

public static class DuongExtensions
{
    private static string LevelKey => "level_save";
    private static string GameKey => "game_save";
    public static void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        ReactivePlayerData.DeleteFile();
        ReactivePlayerDailyData.DeleteFile();
        ReactivePlayerWeeklyData.DeleteFile();
        EventDayPlayerData.DeleteAllFile();
        File.Delete($"{Application.persistentDataPath}/CurrentGameData.json");
        BuilderPlayerData.DeleteFile();
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.DeleteKey(GameKey);
    }
    public static DateTime EndOfWeek(this DateTime dateTime)
    {
        DateTime start = StartOfWeek(dateTime);

        return start.AddDays(6);
    }
    
    public static bool[,] Clone(this bool[,] map)
    {
        var m = new bool[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                m[i, j] = map[i, j];
            }
        }

        return m;
    }

    public static string ToSnakeCase(this string inputStr)
    {
        StringBuilder output = new StringBuilder();

        bool first = false;
        foreach (char c in inputStr)
        {
            if (!first)
            {
                output.Append(Char.ToLower(c));
                first = true;
                continue;
            }
                
            if (char.IsUpper(c))
            {
                output.Append('_');
                output.Append(Char.ToLower(c));
            }
            else
            {
                output.Append(c);
            }
        }

        return output.ToString();
    }

    public static void ReturnAll<T>(this IList<T> list,bool clear = true) where T : PoolItem
    {
        foreach (var poolItem in list)
            poolItem.Return();
        if(clear)
            list.Clear();
    }
    
    public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
    
    public static bool IsListContains(IList<int> listValue,int compareValue,bool loopLastValue)
    {
        foreach (var i in listValue)
            if (compareValue == i)
                return true;

        if (loopLastValue && listValue.Count > 0)
        {
            var last = listValue[listValue.Count - 1];
            if (compareValue > last)
            {
                var repeat = listValue.GetRepeatValue();
                var sub = compareValue - last;
                if (sub % repeat == 0)
                    return true;
            }
        }

        return false;
    }

    public static Point FindNearBy(this Vector3 position, Vector3[,] mapPosition, bool[,] validMap, float minDistance)
    {
        float min = minDistance;
        Point nearest = null;
        for (int i = 0; i < mapPosition.GetLength(0); i++)
        {
            for (int j = 0; j < mapPosition.GetLength(1); j++)
                if (validMap == null || validMap[i, j])
                {
                    var magnitude = (position - mapPosition[i, j]).magnitude;
                    if (magnitude < min)
                    {
                        min = magnitude;
                        nearest = new Point(i, j);
                    }
                }
        }

        return nearest;
    }

    public static int Size(this Vector2Int t) => t.x * t.y;
    
    public static int ChildCountActive(this Transform t)
    {
        int k = 0;
        foreach (Transform c in t)
        {
            if (c.gameObject.activeSelf)
                k++;
        }
        return k;
    }

    public static Transform GetActiveChild(this Transform t, int i)
    {
        int k = 0;
        foreach (Transform c in t)
        {
            if (c.gameObject.activeSelf)
            {
                if (k == i)
                    return c;
                k++;
            }
        }

        return null;
    }
    
    public static bool IsOdd(this int i, int col)
    {
        var r = i / col;
        var c = i % col;
        if (r % 2 == 0)
            return c % 2 == 1;

        return c % 2 == 0;
    }

    public static int Int0(this Vector3 vec) => (int) vec.x;
    public static int Int1(this Vector3 vec) => (int) vec.y;
    public static int Int2(this Vector3 vec) => (int) vec.z;
    
    public static float Value0(this Vector3 vec) => vec.x;
    public static float Value1(this Vector3 vec) => vec.y;
    public static float Value2(this Vector3 vec) =>  vec.z;
    public static IntIntFloat IntIntFloat(this Vector3 vec) =>  new IntIntFloat(vec.Int0(),vec.Int1(),vec.Value2());
    public static IntFloatFloat IntFloatFloat(this Vector3 vec) =>  new IntFloatFloat(vec.Int0(),vec.Value1(),vec.Value2());

    public static bool InRange(this Vector2Int range, int value)
    {
        return value >= range.x && value <= range.y;
    }

    public static bool InRange<T>(this IList<T> arr, int value)
    {
        return value >= 0 && value < arr.Count;
    }
    
    public static bool InRange(this int value,Vector2Int range)
    {
        return value >= range.x && value <= range.y;
    }
    
    public static void DrawLine(this RectTransform thisRectTransform, Point point1, Point point2)
    {
        thisRectTransform.DrawLine(point1.GetPos(),point2.GetPos());
    }
    
    public static bool IsInDate(int epochDate, int startEpochDate, int activeDays)
    {
        return epochDate >= startEpochDate && epochDate < startEpochDate + activeDays;
    }

    public static void DrawLine(this RectTransform thisRectTransform, Vector3 pos1, Vector3 pos2)
    {
        thisRectTransform.localPosition = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2);
        var distance = pos1 - pos2;
        thisRectTransform.localEulerAngles = distance.Angle();
        thisRectTransform.sizeDelta = new Vector2(distance.magnitude, thisRectTransform.sizeDelta.y);
    }

    public static void RectDrawLine(this RectTransform thisRectTransform, RectTransform pos1, RectTransform pos2)
    {
        thisRectTransform.localPosition = new Vector3((pos1.localPosition.x + pos2.localPosition.x) / 2, (pos1.localPosition.y + pos2.localPosition.y) / 2);
        var distance = pos1.localPosition - pos2.localPosition;
        thisRectTransform.localEulerAngles = distance.Angle();
        thisRectTransform.sizeDelta = new Vector2(distance.magnitude, thisRectTransform.sizeDelta.y);
    }

    public static DateTime StartOfWeek(this DateTime dateTime)
    {
        int days = dateTime.DayOfWeek - DayOfWeek.Monday;

        if (days < 0)
            days += 7;

        return dateTime.AddDays(-1 * days).Date;
    }

    public static string RemoveWhiteSpace(this string source)
    {
        return Regex.Replace(source, @"s", "");
    }

    public static int ReverseMod(this int value, int mod)
    {
        return (mod - value % mod) % mod;
    }

    public static string GetLast(this string source, int tailLength)
    {
        if (string.IsNullOrEmpty(source))
            return "";
        if (tailLength > source.Length)
            return "";

        return source.Substring(source.Length - tailLength);
    }

    public static float GetArrangePos(int total, int i, float gap)
    {
        return i * gap -
               (total - 1) * gap / 2;
    }

    public static void ResetAllAnimatorTriggers(this Animator animator)
    {
        foreach (var trigger in animator.parameters)
            if (trigger.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(trigger.name);
    }

 
    
    public static IList<int> GetListFromFlag(this int flag)
    {
        var list = new List<int>();
        int temp = 0;
        int index = 0;
        while (temp < flag)
        {
            if((flag & 1 << index) != 0)
                list.Add(index);
            temp += 1 << index;
            index++;
        }

        return list;
    }
    
    public static int TranslationRotate(this int flag,int clamp = 4)
    {
        int temp = 0;
        int index = 0;
        int translation = 0;
        while (temp < flag)
        {
            if ((flag & 1 << index) != 0)
                translation |= 1 << ((index + 1) % clamp);
            temp += 1 << index;
            index++;
        }

        return translation;
    }
    
    public static int TogglePosition(this int flag,int position)
    {
        if (!flag.GetBit(position))
            return flag.Union(position);

        return flag.Remove(position);
    }

    public static Vector2 CopyXZ(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    
    public static float Lerp(this Vector2 vec,float f)
    {
        return Mathf.Lerp(vec.x, vec.y, f);
    }

    public static Quaternion GetRotateQuaternion(Vector3 current, Vector3 target, float addDegree = -90)
    {
        Vector3 relativeVector = target - current;
        float rotationZ = Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0.0f, 0.0f, rotationZ + addDegree);
    }

    public static Quaternion GetRotateQuaternionY(Vector3 current, Vector3 target, float addDegree = -90)
    {
        Vector3 relativeVector = target - current;
        float rotationZ = Mathf.Atan2(relativeVector.x, relativeVector.z) * Mathf.Rad2Deg;
        return Quaternion.Euler(0.0f, rotationZ + addDegree, 0.0f);
    }

    public static Vector3 GetMousePositionInWorld(Camera cam)
    {
        var position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        position.z = 0;
        return position;
    }

    public static Vector3 GetMousePositionInWorld(Camera cam, Vector3 mouseInput)
    {
        var position = cam.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y));
        position.z = 0;
        return position;
    }

    public static void DestroyAllChildrenExcept(this Transform transform, Transform except, bool immediately = false)
    {
        int childs = transform.childCount;
        int ignore = 0;
        for (int i = childs - 1; i >= 1 - ignore; i--)
        {
            if (transform.GetChild(i) == except)
                ignore--;

            if (!immediately)
                UnityEngine.Object.Destroy(transform.GetChild(i - ignore).gameObject);
            else
                UnityEngine.Object.DestroyImmediate(transform.GetChild(i - ignore).gameObject);
        }
    }


    public static bool AnyNull<T>(this IList<T> list, string listName, MonoBehaviour focus = null)
        where T : MonoBehaviour
    {
        bool flag = false;
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                if (focus != null)
                {
                    Debug.LogError(typeof(T) + " at position " + i + " in list " + listName + " is NULL",
                        focus.gameObject);
                }
                else
                {
                    Debug.LogError(typeof(T) + " at position " + i + " in list " + listName + "is NULL");
                }

                flag = true;
            }
        }

        return flag;
    }

    public static void DestroyAllChildren(this Transform transform, bool immediately = false)
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            if (!immediately)
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            else
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public static bool GetByRate(float rate)
    {
        int number = (int) (rate * 10000);
        return UnityEngine.Random.Range(0, 10000) <= number;
    }

    public static Vector3 ToEuler(this Vector2 vector, float addDegree = -90)
    {
        return new Vector3(0, 0, Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg + addDegree);
    }

    public static Vector3 ToEuler(this Vector3 vector, float addDegree = -90)
    {
        return new Vector3(0, 0, Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg + addDegree);
    }

    public static IntReactiveProperty GetIntReactiveProperty(this FieldInfo fi, object obj)
    {
        return (IntReactiveProperty) fi.GetValue(obj);
    }

    public static IntReactiveProperty GetIntReactiveProperty(this Type t, string propertyName, object obj)
    {
        return (IntReactiveProperty) t.GetField(propertyName).GetValue(obj);
    }

    public static Vector3 CenterOfVectors(this IList<Vector3> vectors)
    {
        if (vectors.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (Vector3 vec in vectors)
            sum += vec;

        return sum / vectors.Count;
    }


    public static bool HaveNotch()
    {
        return Screen.safeArea.height < Screen.height;
    }

    public static void Release(this Transform tf, bool deactivate = true)
    {
        tf.SetParent(null);
        if (deactivate)
            tf.gameObject.SetActive(false);
    }

    public static int GetRandom(int from, int to)
    {
        return UnityEngine.Random.Range(from, to);
    }

    public static int GetRandom(this Vector2Int random)
    {
        return UnityEngine.Random.Range(random.x, random.y);
    }

    public static float GetRandom(this Vector2 random)
    {
        return UnityEngine.Random.Range(random.x, random.y);
    }

    public static Vector3 RemoveZ(this Vector3 input, float newY = 0)
    {
        return new Vector3(input.x, input.y, newY);
    }


    public static Vector3 RemoveY(this Vector3 input, float newY = 0)
    {
        return new Vector3(input.x, newY, input.z);
    }

    public static Vector3 RemoveXZ(this Vector3 input, float newX = 0, float newZ = 0)
    {
        return new Vector3(newX, input.y, newZ);
    }

    public static Point GetRemotePoint(string colKey, string rowKey)
    {
        return new Point((int) RemoteConfigHelper.GetValueInt(colKey), (int) RemoteConfigHelper.GetValueInt(rowKey));
    }

    private static void SetParentAndExpand(this RectTransform rt, Transform parent, bool setFirstSibling,
        bool setActive)
    {
        rt.transform.SetParent(parent);
        if (setFirstSibling)
            rt.SetAsFirstSibling();
        rt.SetExpand();
        if (setActive)
            rt.gameObject.SetActive(true);
    }

    public static void SetExpand(this RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.localPosition = Vector3.zero;
        rt.anchoredPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.localEulerAngles = Vector3.zero;
    }

    public static Point ToNormalizedPoint(float x, float y, float threshold)
    {
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > threshold)
                return Point.Right;
            if (x < -threshold)
                return Point.Left;
        }
        else
        {
            if (y > threshold)
                return Point.Top;
            if (y < -threshold)
                return Point.Bottom;
        }

        return Point.Zero;
    }

    public static void SetParentAndExpand(this Transform tf, Transform parent, bool setFirstSibling, bool setActive) =>
        SetParentAndExpand(tf as RectTransform, parent, setFirstSibling, setActive);


    public static Vector3 CreateVec3(this float value)
    {
        return new Vector3(value, value, value);
    }

    public static Vector2 CreateVec2(this float value)
    {
        return new Vector2(value, value);
    }

    public static IObservable<Unit> TimeTweenObservable(float time, float delay, Action<float> onChange,
        bool invokeZeroOnStart = false)
    {
        return Observable.FromCoroutine(() => TimeTweenEnumerator(time, delay, onChange, invokeZeroOnStart));
    }

    public static IEnumerator TimeTweenEnumerator(float time, float delay, Action<float> onChange,
        bool invokeZeroOnStart = false)
    {
        if (invokeZeroOnStart)
            onChange.Invoke(0);

        if (delay > 0)
            yield return new WaitForSeconds(delay);
        float t = 0f;
        if (!invokeZeroOnStart)
            onChange.Invoke(0);
        yield return null;
        while (t < time)
        {
            t += Time.deltaTime;
            onChange.Invoke(t / time);
            yield return null;
        }

        onChange.Invoke(1);
    }

    public static Product TakeByWeight(this IList<Product> arrs, out int index)
    {
        if (arrs.Count == 1)
        {
            index = 0;
            return arrs[0];
        }

        int totalWeight = 0;
        int[] weightValues = new int[arrs.Count];
        for (var i = 0; i < arrs.Count; i++)
        {
            totalWeight += arrs[i].parameter;
            weightValues[i] = totalWeight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < weightValues.Length; i++)
            if (randomValue < weightValues[i])
            {
                index = i;
                return arrs[i];
            }

        Debug.LogError("Error:GetByWeight");
        {
            index = -1;
            return arrs[-1];
        }
    }


    public static T TakeByWeight<T>(this IList<T> arrs, IList<int> weights)
    {
        if (arrs.Count == 1)
            return arrs[0];

        int totalWeight = 0;
        int[] weightValues = new int[weights.Count];
        for (var i = 0; i < weights.Count; i++)
        {
            totalWeight += weights[i];
            weightValues[i] = totalWeight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < weightValues.Length; i++)
            if (randomValue < weightValues[i])
                return arrs[i];

        Debug.LogError("Error:GetByWeight");
        return arrs[-1];
    }

    public static T TakeByWeight<T>(this IList<T> arrs, IList<float> weights)
    {
        if (arrs.Count == 1)
            return arrs[0];

        float totalWeight = 0;
        float[] weightValues = new float[weights.Count];
        for (var i = 0; i < weights.Count; i++)
        {
            totalWeight += weights[i];
            weightValues[i] = totalWeight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        for (int i = 0; i < weightValues.Length; i++)
            if (randomValue < weightValues[i])
                return arrs[i];

        Debug.LogError("Error");
        return arrs[0];
    }


    public static int Pow(this int number, int power)
    {
        int result = 1;
        for (int i = 0; i < power; i++)
        {
            result *= number;
        }

        return result;
    }

    public static int GetNumberAtPlace(this int value, int place)
    {
        return value / DuongExtensions.Pow(10, place) % 10;
    }

    public static int GetEpochWeek()
    {
        DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
        Calendar cal = dfi.Calendar;
        var now = DateTime.Now;
        return cal.GetWeekOfYear(now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
    }

    public static Type GetType(string nameSpace, string className)
    {
//        var q = from t in Assembly.GetExecutingAssembly().GetTypes()
//                where t.Namespace == "MergeBlock" 
//            select t;
//            
//        Debug.Log(q.ToList().Count);
//            q.ToList().ForEach(t => Console.WriteLine());
//            
//        foreach (var type in q.OrderBy(x =>x.FullName).ToList())
//        {
//            Debug.Log(type.FullName);
//        }
//        
        return Type.GetType(nameSpace + "." + className);
    }

    public static Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos,
            parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point

        return movePos;
    }

    public static Vector2 FlipY(this Vector2 vec)
    {
        return new Vector2(vec.x, -vec.y);
    }

    public static void SetActive<T>(this IList<T> list, bool value) where T : MonoBehaviour
    {
        foreach (var x1 in list)
            x1.gameObject.SetActive(value);
    }

    public static void SetActive(this IList<Transform> list, bool value)
    {
        foreach (var x1 in list)
            x1.gameObject.SetActive(value);
    }


    public static Color Fade(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static Color GrayLevel(this Color color, float graylevel)
    {
        return new Color(color.r * graylevel, color.g * graylevel, color.b * graylevel, 1);
    }

    public static Vector3 CenterOfVectorsMaxSize(this IList<Vector3> vectors)
    {
        if (vectors.Count == 0) return Vector3.zero;

        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        foreach (var vector3 in vectors)
        {
            if (vector3.x < min.x)
                min.x = vector3.x;
            if (vector3.x > max.x)
                max.x = vector3.x;

            if (vector3.y < min.y)
                min.y = vector3.y;
            if (vector3.y > max.y)
                max.y = vector3.y;

            if (vector3.z < min.z)
                min.z = vector3.z;
            if (vector3.z > max.z)
                max.z = vector3.z;
        }

        return (min + max) / 2;
    }

    public static IEnumerable<T> GetComponentsInFirstBornChildren<T>(this Transform transform, bool ifActive = false)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            T find = transform.GetChild(i).GetComponent<T>();
            if (find != null && (!ifActive || transform.gameObject.activeSelf))
                yield return find;
        }
    }

#if UNITY_EDITOR
    public static IEnumerable<string> LoadTextAtPaths(string folder, string extensions = "*.json")
    {
        var info = new DirectoryInfo(folder);
        var files = info.GetFiles(extensions)
            .OrderBy(x => int.Parse(System.IO.Path.GetFileNameWithoutExtension(x.Name)));
        foreach (var file in files)
        {
            StreamReader reader = new StreamReader(file.FullName);
            yield return reader.ReadToEnd();
            reader.Close();
        }
    }

    public static string SaveResources(string path, string fileName, string serialized, bool log, bool refresh)
    {
        var folder = $"{path}/Resources";
        var filePath = $"{folder}/{fileName}";

        if (log)
            Debug.Log(filePath);
        Directory.CreateDirectory(folder);
        File.WriteAllText(filePath, serialized);

        if (refresh)
            UnityEditor.AssetDatabase.Refresh();
        return filePath;
    }


    public static string LoadTextAtPath(string path)
    {
        string readContents;
        using (StreamReader streamReader = new StreamReader(path))
        {
            readContents = streamReader.ReadToEnd();
        }

        return readContents;
    }
#endif
    
    public static string GetStringSha256Hash(string text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }

    public static void Merge(this List<Product> list, IList<Product> otherList)
    {
        foreach (var product in otherList)
        {
            bool found = false;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].quantity == product.quantity && list[i].index == product.index)
                {
                    list[i].amount += product.amount;
                    found = true;
                }
            }

            if (!found)
                list.Add(product.Clone());
        }
    }


    public static void Shuffle<T>(this IList<T> list, Random rng = null)
    {
        if (rng == null)
            rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public static int[] ShuffledIndexList<T>(this IList<T> list)
    {
        var rng = new System.Random();
        var arrayList = new int[list.Count];
        for (var i = 0; i < arrayList.Length; i++)
            arrayList[i] = i;

        arrayList.Shuffle(rng);
        return arrayList;
    }

    public static int[] ShuffledIndexList(int listCount)
    {
        var rng = new System.Random();
        var arrayList = new int[listCount];
        for (var i = 0; i < arrayList.Length; i++)
            arrayList[i] = i;

        arrayList.Shuffle(rng);
        return arrayList;
    }

    public static float GetPosFromCenter(int index, int total, float size)
    {
        return (index - (total - 1) / 2f) * size;
    }

    public static IDisposable Timer(this float timer, Action action)
    {
        return Observable.Timer(TimeSpan.FromSeconds(timer)).Subscribe(data => action.Invoke());
    }

    private static float[] _actionDelay = {-1,-1,-1,-1,-1};
    private static Action[] _delayAction = new Action[5];
    public static void ActionDelay(Action action,int index = 0,float delay = 0.5f)
    {
        if (_actionDelay[index] <= 0)
        {
            _delayAction[index] = action;
            _actionDelay[index] = delay;
            Observable.FromMicroCoroutine(() => IeSaveDelay(index)).Subscribe();
        }
        else
        {
            _actionDelay[index] = delay;
        }
    }

    private static IEnumerator IeSaveDelay(int index)
    {
        while (_actionDelay[index] > 0)
        {
            _actionDelay[index] -= Time.deltaTime;
            yield return null;
        }
        _delayAction[index].Invoke();
    }

    public static IDisposable ExecuteAfterInterstitialClosed(this Action action, float wait = 0.1f)
    {
        return Observable.FromMicroCoroutine(() => IeExecuteAfterInterstitialClosed(action, wait)).Subscribe();
        //.AddToScreenChangeDisposable();
    }

    private static IEnumerator IeExecuteAfterInterstitialClosed(this Action action, float wait = 0.1f)
    {
        yield return new WaitForSeconds(wait);
        var adsManager = Kernel.Resolve<AdsManager>();
        while (adsManager.IsIntersShowing())
            yield return null;
        action.Invoke();
    }

    public static IDisposable ExecuteInNextFrame(this Action action)
    {
        return IoExecuteInNextFrame(action).Subscribe();
    }
    
    public static IObservable<Unit> IoExecuteInNextFrame(this Action action)
    {
        return Observable.FromMicroCoroutine(() => IeExecuteInNextFrame(action));
    }

    private static IEnumerator IeExecuteInNextFrame(this Action action)
    {
        yield return null;
        if(action != null)
            action.Invoke();
    }
    
    public static IDisposable ExecuteInNextNFrame(this Action action,int n=1)
    {
        return IoExecuteInNextNFrame(action,n).Subscribe();
    }
    
    public static IObservable<Unit> IoExecuteInNextNFrame(this Action action,int n=1)
    {
        return Observable.FromMicroCoroutine(() => IeExecuteInNext2Frame(action,n));
    }

    private static IEnumerator IeExecuteInNext2Frame(this Action action,int n =1)
    {
        while (n > 0)
        {
            n--;
            yield return null;
        }
        if(action != null)
            action.Invoke();
    }
    
    public static IObservable<Unit> IoDelay(float delay)
    {
        return Observable.FromMicroCoroutine(() => IeDelay(delay));
    }

    public static IEnumerator IeDelay(float delay)
    {
       
        yield return new WaitForSeconds(delay);
    }
    
    public static IObservable<Unit> IoExecuteAction(this Action action)
    {
        return Observable.FromMicroCoroutine(() => IeExecuteAction(action));
    }

    public static IEnumerator IeExecuteAction(this Action action)
    {
        if(action != null)
            action.Invoke();
        yield return null;
    }

    public static IDisposable TimerUnscale(this float timer, Action action)
    {
        return Observable.Timer(TimeSpan.FromSeconds(timer), Scheduler.MainThreadIgnoreTimeScale)
            .Subscribe(data => action.Invoke());
    }


    public static IEnumerator IeTimer(this float timer, Action action)
    {
        yield return new WaitForSeconds(timer);
        action();
    }

    public static T RayCast<T>(Vector3 position, Func<T, bool> predicate = null) where T : class
    {
        var hits = Physics2D.RaycastAll(position, new Vector3(0, 0, 1), Mathf.Infinity);
        foreach (var raycastHit2D in hits)
        {
            var tile = raycastHit2D.collider.gameObject.GetComponent<T>();
            if (tile != null && (predicate == null || predicate(tile)))
                return tile;
        }

        return null;
    }
    
    public static IEnumerable<T> RayCastAll<T>(Vector3 position, Func<T, bool> predicate = null) where T : class
    {
        var hits = Physics2D.RaycastAll(position, new Vector3(0, 0, 1), Mathf.Infinity);
        foreach (var raycastHit2D in hits)
        {
            var tile = raycastHit2D.collider.gameObject.GetComponent<T>();
            if (tile != null && (predicate == null || predicate(tile)))
                yield  return tile;
        }

    }
    
    public static IEnumerable<T> CircleCastAll<T>(Vector3 position,float radius ,Func<T, bool> predicate = null) where T : class
    {
        var hits = Physics2D.CircleCastAll(position,radius, new Vector3(0, 0, 1), Mathf.Infinity);
        foreach (var raycastHit2D in hits)
        {
            var tile = raycastHit2D.collider.gameObject.GetComponent<T>();
            if (tile != null && (predicate == null || predicate(tile)))
                yield  return tile;
        }
    }

    
    public static int GetEpochDayOfYear()
    {
#if UNITY_EDITOR
        return DateTime.Today.DayOfYear + TestingSettings.EpochDayAdd;
#else
        return DateTime.Today.DayOfYear + PlayerPrefs.GetInt("EpochDayAdd");
#endif
    }

    public static double TotalSeconds(this DateTime dateTime)
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0);
        TimeSpan result = dateTime.Subtract(epochStart);
        return result.TotalSeconds;
    }

    public static Vector2 GetXzVector2(this Transform tf,bool world = false)
    {
        var pos = world ? tf.position : tf.localPosition;
        return new Vector2(pos.x,pos.z);
    }
    

    public static int GetEpochDayOfYear(int m, int d)
    {
        return new DateTime(2023, m, d).DayOfYear;
    }


    

    public static Point GetClosestPoint<T>(Transform dragTransform, T[] positions, float validDistance)
        where T : PointWithVector3
    {
        var position = dragTransform.position;
        Point point = null;
        var min = validDistance;
        for (var i = 0; i < positions.Length; i++)
        {
            var distance = Vector2.Distance(position, positions[i].value);
            if (distance < min)
                point = positions[i];
        }

        return point != null ? point.Clone() : null;
    }

    public static Point GetClosestPoint<T>(Transform dragTransform, T[,] positions, float validDistance)
        where T : PointWithVector3
    {
        var position = dragTransform.position;
        Point point = null;
        var min = validDistance;
        for (var i = 0; i < positions.GetLength(0); i++)
        for (int j = 0; j < positions.GetLength(1); j++)
        {
            var distance = Vector2.Distance(position, positions[i, j].value);
            if (distance < min)
                point = positions[i, j];
        }

        return point != null ? point.Clone() : null;
    }

    public static void SetActiveAll(this IList<GameObject> list, bool value)
    {
        foreach (var x1 in list)
            x1.SetActive(value);
    }


    public static void ReturnAll<T>(this T[,] map) where T : PoolItem
    {
        for (var i1 = 0; i1 < map.GetLength(0); i1++)
        for (var i2 = 0; i2 < map.GetLength(1); i2++)
            if (map[i1, i2] != null)
            {
                map[i1, i2].Return();
                map[i1, i2] = null;
            }
    }

    public static void SetActiveAll(this IList<Transform> list, bool value)
    {
        foreach (var x1 in list)
            x1.gameObject.SetActive(value);
    }


    public static void SetActiveAll<T>(this IList<T> list, bool value) where T : MonoBehaviour
    {
        foreach (var x1 in list)
            x1.gameObject.SetActive(value);
    }


    public static IEnumerator RxTweenPath(RxTweenEnumeratorData[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            var input = data[i];

            if (input.IsLocal)
                input.Target.localPosition = input.From;
            else
                input.Target.position = input.From;

            if (input.DelayBefore > 0)
                yield return new WaitForSeconds(input.DelayBefore);

            var from = (input.IsLocal) ? input.Target.localPosition : input.Target.position;

            float duration = input.Duration;
            if (input.UseSpeed)
                duration = Vector2.Distance(from, input.To) / input.Speed;

            float t = 0;
            while (t < duration)
            {
                var d = (input.UseCurve)
                    ? input.Curve.Evaluate(t / duration)
                    : EasingFunction.GetEasingFunction(input.Ease).Invoke(0, 1, t / duration);

                if (input.IsLocal)
                    input.Target.localPosition = Vector2.Lerp(from, input.To, d);
                else
                    input.Target.position = Vector2.Lerp(from, input.To, d);

                if (input.OnUpdate != null)
                    input.OnUpdate();

                yield return null;
                t += Time.deltaTime;
            }

            if (input.IsLocal)
                input.Target.localPosition = Vector2.Lerp(from, input.To, 1);
            else
                input.Target.position = Vector2.Lerp(from, input.To, 1);
            if (input.OnUpdate != null)
                input.OnUpdate();


            if (input.OnFinish != null)
                input.OnFinish();
        }
    }

    public static T RayCastNonAlloc<T>(Vector3 position, Func<T, bool> predicate = null, int take = 1) where T : class
    {
        RaycastHit2D[] hits = new RaycastHit2D[take];
        var size = Physics2D.RaycastNonAlloc(position, new Vector3(0, 0, 1), hits, Mathf.Infinity);
        foreach (var raycastHit2D in hits)
        {
            if (raycastHit2D.collider != null)
            {
                var tile = raycastHit2D.collider.gameObject.GetComponent<T>();
                if (tile != null && (predicate == null || predicate(tile)))
                    return tile;
            }
        }

        return null;
    }

    public static RaycastHit2D[] RayCastNonAlloc(Vector3 position, int take = 1)
    {
        RaycastHit2D[] hits = new RaycastHit2D[take];
        var size = Physics2D.RaycastNonAlloc(position, new Vector3(0, 0, 1), hits, Mathf.Infinity);
        return hits;
    }

    public static RaycastHit[] RayCast3DNonAlloc(Vector3 position, int take = 1)
    {
        RaycastHit[] hits = new RaycastHit[take];
        var size = Physics.RaycastNonAlloc(position, new Vector3(0, 0, 1), hits, Mathf.Infinity);
        return hits;
    }

    public static T RayCast3DNonAlloc<T>(Vector3 position, Func<T, bool> predicate = null, int take = 1) where T : class
    {
        RaycastHit[] hits = new RaycastHit[take];
        var size = Physics.RaycastNonAlloc(position, new Vector3(0, 0, 1), hits, Mathf.Infinity);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                var tile = hit.collider.gameObject.GetComponent<T>();
                if (tile != null && (predicate == null || predicate(tile)))
                    return tile;
            }
        }

        return null;
    }

    public static T RayCast3DNonAlloc<T>(Ray ray, Func<T, bool> predicate = null, int take = 1) where T : class
    {
        RaycastHit[] hits = new RaycastHit[take];
        var size = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                var tile = hit.collider.gameObject.GetComponent<T>();
                if (tile != null && (predicate == null || predicate(tile)))
                    return tile;
            }
        }

        return null;
    }

    public static T RayCast3DNonAlloc<T>(Ray ray, int mask, Func<T, bool> predicate = null, int take = 1)
        where T : class
    {
        RaycastHit[] hits = new RaycastHit[take];
        var size = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, mask);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                var component = hit.collider.gameObject.GetComponent<T>();
                if (component != null && (predicate == null || predicate(component)))
                    return component;
            }
        }

        return null;
    }

    
    /// <summary>
    /// all layer is ~0 , specific layer 1 << LayerMask.NameToLayer("vehicle")
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="mask"></param>
    /// <param name="predicate"></param>
    /// <param name="take"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T RayCast3DNonAllocMin<T>(Ray ray, int mask, Func<T, bool> predicate = null, int take = 1)
        where T : class
    {
        RaycastHit[] hits = new RaycastHit[take];
        var size = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, mask);
        float min = float.MaxValue;
        T result = null;
        foreach (var hit in hits)
            if (hit.collider != null)
            {
                var component = hit.collider.gameObject.GetComponent<T>();
                if (component != null && (predicate == null || predicate(component)) && hit.distance < min)
                {
                    result = component;
                    min = hit.distance;
                }
            }


        return result;
    }
    
    public static T RayCast3DNonAllocMinHitView3D<T>(Ray ray, int mask, Func<T, bool> predicate = null, int take = 1)
        where T : class
    {
        RaycastHit[] hits = new RaycastHit[take];
        var size = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, mask);
        float min = float.MaxValue;
        T result = null;
        foreach (var hit in hits)
            if (hit.collider != null)
            {
                var component = hit.collider.gameObject.GetComponent<T>();
                if (component != null && (predicate == null || predicate(component)) && hit.distance < min)
                {
                    result = component;
                    min = hit.distance;
                }
                else
                {
                    var hitView3D = hit.collider.gameObject.GetComponent<HitView3D>();
                    if (hitView3D != null && hitView3D.ParentView != null && hitView3D.ParentView is T)
                    {
                        component = hitView3D.ParentView as T;
                        if (component != null && (predicate == null || predicate(component)) && hit.distance < min)
                        {
                            result = component;
                            min = hit.distance;
                        }
                    }
                }
            }


        return result;
    }

    public static T RayCast3DFirstHit<T>(Ray ray, Func<T, bool> predicate = null, int take = 1) where T : class
    {
        var size = Physics.Raycast(ray, out RaycastHit hit, 100);
        if (hit.collider != null)
        {
            var tile = hit.collider.gameObject.GetComponent<T>();
            if (tile != null && (predicate == null || predicate(tile)))
                return tile;
        }

        return null;
    }


    public static void OnChanged<T>(this IEnumerable<T> scripts, bool value) where T : ToggleScript
    {
        foreach (var toggleScript in scripts)
        {
            if (toggleScript == null)
                Debug.LogError("toggle is null");
            else
                toggleScript.OnChanged(value);
        }
    }
    
 
    
    public static void OnChanged<T>(this IEnumerable<T> scripts, bool value, GameObject callFrom) where T : ToggleScript
    {
        foreach (var toggleScript in scripts)
        {
            if (toggleScript == null)
            {
                Debug.LogError("toggle is null on " + callFrom, callFrom);
            }
            else
                toggleScript.OnChanged(value);
        }
    }

    public static void OnChangedInt<T>(this IEnumerable<T> scripts, int value) where T : ToggleScript
    {
        foreach (var toggleScript in scripts)
        {
            if (toggleScript == null)
                Debug.LogError("toggle is null");
            else
                toggleScript.OnChanged(value);
        }
    }

    public static void OnChanged<T>(this IEnumerable<T> scripts, int value) where T : IndexBindingScriptBase
    {
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
                Debug.LogError("IndexBindingScript is null");
            else
                toggleScript.OnChanged(value);
    }

    public static void OnChanged<T>(this IEnumerable<T> scripts, int value, int value2) where T : IndexIndexBindingScriptBase
    {
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
                Debug.LogError("IndexBindingScript is null");
            else
                toggleScript.OnChanged(value,value2);
    }

    public static void OnChanged<T>(this IEnumerable<T> scripts, float value) where T : IndexBindingScriptBase
    {
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
            {
                Debug.LogError("IndexBindingScript is null");
            }
            else
            {
                toggleScript.OnChanged((int)value);
                toggleScript.OnChanged2(value);
            }
    }

    public static void OnChangedBoolean<T>(this IEnumerable<T> scripts, bool value) where T : IndexBindingScript
    {
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
                Debug.LogError("toggle is null");
            else
                toggleScript.OnChangedBoolean(value);
    }
    
    public static int GetRemapValue(this IList<Vector2Int> remaps,int a,int b)
    {
        for (var i = 0; i < remaps.Count; i++)
        {

            if (remaps[i].x == a && remaps[i].y == b)
                return i;
        }

        return -1;
    }
    
    public static int GetRemapValue(this IList<IntInt> remaps,int a,int b)
    {
        for (var i = 0; i < remaps.Count; i++)
        {

            if (remaps[i].value == a && remaps[i].value2 == b)
                return i;
        }

        return -1;
    }
    
    public static float Unlerp(float start, float end, float x) { return (x - start) / (end - start); }

    public static float Remap(float sourceFrom, float sourceTo, float desFrom, float desTo,float x)
    {
        return Mathf.Lerp(desFrom, desTo, Unlerp(sourceFrom, sourceTo, x));
    }
    
    public static float Remap(Vector4 vec,float x)
    {
        return Mathf.Lerp(vec.z, vec.w, Unlerp(vec.x, vec.y, x));
    }
    
    public static float Remap(Vector2Int source,Vector2 remapTo,float x)
    {
        return Mathf.Lerp(remapTo.x, remapTo.y, Unlerp(source.x, source.y, x));
    }
    
    

    
    public static float Remap(Vector4 vec,float x,AnimationCurve curve)
    {
        return Mathf.Lerp(vec.z, vec.w, curve.Evaluate(Unlerp(vec.x, vec.y, x)));
    }


    public static int ParentCount(this Transform tf)
    {
        if (tf.parent != null)
            return 1 + ParentCount(tf.parent);
        return 0;
    }

    
    //ex : [0,2] [2,0] i=2 => return 0, i=1 => return 1, i = 0 => return 2
    public static int GetExchangeValue(this IEnumerable<Vector2Int> exchangePositions,int index,bool twoWays)
    {
        if(twoWays)
            foreach (var remapPosition in exchangePositions)
            {
                if (remapPosition.x == index)
                    return remapPosition.y;
                if (remapPosition.y == index)
                    return remapPosition.x; 
            }
        else
        {
            var vector2Ints = exchangePositions as Vector2Int[] ?? exchangePositions.ToArray();
            foreach (var remapPosition in vector2Ints)
                if (remapPosition.x == index)
                    return remapPosition.y;
            foreach (var remapPosition in vector2Ints)
                if (remapPosition.y == index)
                    return remapPosition.x; 
        }

        return index;
    }
    
    public static int GetSumUpValue(this IEnumerable<Vector2Int> sumUpValues,int index)
    {
        int sumUp = 0;
        foreach (var remapPosition in sumUpValues)
            if (index >= remapPosition.x)
                sumUp += remapPosition.y;
        return sumUp;
    }



    public static Vector3 Bezier2(Vector3 s, Vector3 p, Vector3 e, float t)
    {
        float rt = 1 - t;
        return rt * rt * s + 2 * rt * t * p + t * t * e;
    }

    public static Vector3 GetAroundPos(float radius, int i, int total)
    {
        return new Vector3(
            radius * Mathf.Cos(2 * Mathf.PI * i / total),
            0,
            radius * Mathf.Sin(2 * Mathf.PI * i / total)
        );
    }

    public static int GetNumberFromString(this string input)
    {
        return int.Parse(Regex.Match(input, @"\d+").Value);
    }

    public static Vector2 WorldToAnchorPos(this Camera cam, Vector3 worldPos, Vector2 canvasSize)
    {
        Vector2 viewportPosition = cam.WorldToViewportPoint(worldPos);
        return new Vector2(
            viewportPosition.x * canvasSize.x - canvasSize.x * 0.5f,
            viewportPosition.y * canvasSize.y - canvasSize.y * 0.5f);
    }

    public static bool[,] CloneMatrix(this bool[,] fillMap)
    {
        var clone = new bool[fillMap.GetLength(0),fillMap.GetLength(1)];
        for (int i = 0; i < fillMap.GetLength(0); i++)
        {
            for (int j = 0; j < fillMap.GetLength(1); j++)
            {
                clone[i, j] = fillMap[i, j];
            }
        }

        return clone;
    }
    
    public static int[,] CloneMatrix(this int[,] fillMap)
    {
        var clone = new int[fillMap.GetLength(0),fillMap.GetLength(1)];
        for (int i = 0; i < fillMap.GetLength(0); i++)
        {
            for (int j = 0; j < fillMap.GetLength(1); j++)
            {
                clone[i, j] = fillMap[i, j];
            }
        }

        return clone;
    }


    // 1 == bottom left, 2 bottom right, 4 == top right, 8 = top left
    public static int[,] GetMaskMap(this bool[,] fillMap,bool reverse = false)
    {
        var clone = CloneMatrix(fillMap);
        if (reverse)
        {
            for (int i = 0; i < clone.GetLength(0); i++)
            {
                for (int j = 0; j < clone.GetLength(1); j++)
                {
                    clone[i, j] = !clone[i, j];
                }
            }
        }
        int cols = clone.GetLength(0);
        int rows = clone.GetLength(1);
        int[,] map = new int[cols + 1, rows + 1];

        for (int i = 0; i < cols + 1; i++)
        for (int j = 0; j < rows + 1; j++)
        {
            bool topL = i > 0 && j < rows && clone[i - 1, j];
            bool topR = i < cols && j < rows && clone[i, j];
            bool bottomL = i > 0 && j > 0 && clone[i - 1, j - 1];
            bool bottomR = i < cols && j > 0 && clone[i, j - 1];

            map[i, j] =
                Convert.ToInt32(bottomL) << 0 |
                Convert.ToInt32(bottomR) << 1 |
                Convert.ToInt32(topR) << 2 |
                Convert.ToInt32(topL) << 3;
        }

        return map;
    }

    public static Vector3Int[,] GetMergeMap(this int[,] fillMap)
    {
        Vector3Int[,] merge = new Vector3Int[fillMap.GetLength(0),fillMap.GetLength(1)];

        for (int i = 0; i < fillMap.GetLength(0); i++)
        for (int j = 0; j < fillMap.GetLength(1); j++)
        {
            merge[i,j] = new Vector3Int(fillMap[i,j],1,1);
        }
        
        for (int i = 0; i < merge.GetLength(0); i++)
        for (int j = 0; j < merge.GetLength(1); j++)
        {
            var start = merge[i, j].x;
            if(start == 15 || start == 9 || start == 6)
                for (int k = j+1; k < merge.GetLength(1); k++)
                {
                    if (merge[i, k].x == start)
                    {
                        merge[i, j].z += 1;
                        merge[i, k].x = 0;
                    }
                    else
                    {
                        break;
                    }
                }
        }
        
        for (int j = 0; j < merge.GetLength(1); j++)
        for (int i = 0; i < merge.GetLength(0); i++)
        {
            var start = merge[i, j].x;
            if (start == 15)
                for (int k = i + 1; k < merge.GetLength(0); k++)
                {
                    if (merge[k, j].x == start && merge[k, j].z ==  merge[i, j].z)
                    {
                        merge[i, j].y += 1;
                        merge[k, j].x = 0;
                    }
                    else
                    {
                        break;
                    }
                }
        }
        
        for (int j = 0; j < merge.GetLength(1); j++)
        for (int i = 0; i < merge.GetLength(0); i++)
        {
            var start = merge[i, j].x;
            if (start == 3 || start == 12)
                for (int k = i + 1; k < merge.GetLength(0); k++)
                {
                    if (merge[k, j].x == start)
                    {
                        merge[i, j].y += 1;
                        merge[k, j].x = 0;
                    }
                    else
                    {
                        break;
                    }
                }
        }

        return merge;
    }

    
    public static float GetCentered(int n, float size)
    {
        return -(Mathf.Max(n - 1, 0) * size) / 2;
    }
    public static Vector3 ChangeZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, z);
    }

    public static Vector3 ChangeY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }

    public static Vector3 ChangeX(this Vector3 vec, float x)
    {
        return new Vector3(x, vec.y, vec.z);
    }
    
    public static Vector3 AddZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y,vec.z+z);
    }

    public static Vector3 AddY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x,vec.y+ y, vec.z);
    }

    public static Vector3 AddX(this Vector3 vec, float x)
    {
        return new Vector3(vec.x+x, vec.y, vec.z);
    }

    public static void SetZ(this Transform tf, float z)
    {
        tf.position = new Vector3(tf.transform.position.x, tf.transform.position.y, z);
    }

    public static T TakeCenter<T>(this IList<T> arr, int adjust = 0)
    {
        return arr[Mathf.FloorToInt(arr.Count * 1f / 2) + adjust];
    }

    public static void SetY(this Transform tf, float y)
    {
        tf.position = new Vector3(tf.transform.position.x, y, tf.transform.position.z);
    }

    public static Vector3 Random(this Vector3 from, Vector3 to)
    {
        return new Vector3(
            UnityEngine.Random.Range(from.x, to.x),
            UnityEngine.Random.Range(from.y, to.y),
            UnityEngine.Random.Range(from.z, to.z)
        );
    }

    public static Vector2 Random(this Vector2 from, Vector2 to)
    {
        return new Vector2(
            UnityEngine.Random.Range(from.x, to.x),
            UnityEngine.Random.Range(from.y, to.y)
        );
    }

    public static float CalculateTime(Vector3 from, Vector3 to, float speed)
    {
        return Vector3.Distance(from, to) / speed;
    }


    public static void SetX(this Transform tf, float x)
    {
        tf.position = new Vector3(x, tf.transform.position.y, tf.transform.position.z);
    }

    public static void SetLocalZ(this Transform tf, float z)
    {
        tf.localPosition = new Vector3(tf.transform.localPosition.x, tf.transform.localPosition.y, z);
    }

    public static void SetLocalY(this Transform tf, float y)
    {
        tf.localPosition = new Vector3(tf.transform.localPosition.x, y, tf.transform.localPosition.z);
    }

    public static int GetRepeatValue(this IList<int> arr)
    {
        if (arr.Count == 0)
            return 0;
        if (arr.Count == 1)
            return arr[0];
        return arr[arr.Count - 1] - arr[arr.Count - 2];
    }

    public static void SetLocalX(this Transform tf, float x)
    {
        var localPosition = tf.localPosition;
        tf.localPosition = new Vector3(x, localPosition.y, localPosition.z);
    }
    
    public static void AddLocalX(this Transform tf, float x)
    {
        var localPosition = tf.localPosition;
        tf.localPosition = new Vector3(localPosition.x + x, localPosition.y, localPosition.z);
    }
    
    public static void AddLocalZ(this Transform tf, float z)
    {
        var localPosition = tf.localPosition;
        tf.localPosition = new Vector3(localPosition.x , localPosition.y, localPosition.z+z);
    }
    
    public static void SetAnchorX(this RectTransform tf, float x)
    {
        var localPosition = tf.anchoredPosition;
        tf.anchoredPosition = new Vector3(x, localPosition.y);
    }
    
    public static void AddAddX(this RectTransform tf, float x)
    {
        var localPosition = tf.anchoredPosition;
        tf.anchoredPosition = new Vector3(localPosition.x + x, localPosition.y);
    }


    public static IEnumerator IEDoValue(float from, float to, float time, Action<float> onUpdate, AnimationCurve curve,
        float delay = 0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        var t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            onUpdate(Mathf.Lerp(from, to, t / time));
            yield return null;
        }
    }

    public static IEnumerator IeDoValueLoopYoYo(float from, float to, float moveTime, Action<float> onUpdate,
        Action<float> onUpdateYoYo)
    {
        var timeMove = 0f;
        bool flipMove = false;
        while (true)
        {
            timeMove += Time.deltaTime;
            if (timeMove > moveTime)
            {
                flipMove = !flipMove;
                timeMove = timeMove % moveTime;
            }

            var d = timeMove / moveTime;
            onUpdate(Mathf.Lerp(from, to, d));
            onUpdateYoYo(Mathf.Lerp(from, to, flipMove ? 1 - d : d));
            yield return null;
        }
    }

    public static IEnumerator IeDoValueLoopYoYo(float from, float to, float moveTime, Action<float> onUpdateYoYo)
    {
        var timeMove = 0f;
        bool flipMove = false;
        while (true)
        {
            timeMove += Time.deltaTime;
            if (timeMove > moveTime)
            {
                flipMove = !flipMove;
                timeMove = timeMove % moveTime;
            }

            var d = timeMove / moveTime;
            onUpdateYoYo(Mathf.Lerp(from, to, flipMove ? 1 - d : d));
            yield return null;
        }
    }

    public static IEnumerator IEDoValue(float from, float to, float time, Action<float> onUpdate, float delay = 0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        var t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            onUpdate(Mathf.Lerp(from, to, t / time));
            yield return null;
        }
    }

    public static bool IsEqual<T>(this IList<T> list, IList<T> listCompare)
    {
        if (list.Count != listCompare.Count) return false;
        for (var i = 0; i < listCompare.Count; i++)
            if (list[i].Equals(listCompare))
                return false;
        return true;
    }

    
}

public static class DuongSerializationExtensions2
{
    public static int Repeat(this int t, int length)
    {
        if (t < 0)
        {
            return (length+1 - Mathf.Abs(t % (length+1))) % (length+1);
        }
        return t % (length + 1);
    }

    public static int RepeatDistance(this int current, int next, int length)
    {
        if (Mathf.Abs(current - next) <= (length + 1) / 2)
            return next - current;
        if (current < next)
            return (length + 1) - Mathf.Abs(current - next);
        return -(length + 1 - Mathf.Abs(current - next));
    }

    // PingPongs the value t, so that it is never larger than length and never smaller than 0.
    public static int PingPong(this int t, int length)
    {
        if (length == 0)
            return 0;
        t = Mathf.Abs(t);
        if (t / length % 2 == 0)
            return t % length;

        return length - t % length;
    }
    
    
    public static IEnumerable<Product> ListProductFromString(string str)
    {
        var list = str.Split(',').Select(int.Parse).ToArray();
        for (var i = 0; i < list.Length / 5; i++)
            yield return new Product((Quantity) list[i * 3 + 0], list[i * 3 + 1], list[i * 3 + 2], list[i * 3 + 3],
                list[i * 3 + 4]);
    }

    public static string ListProductToString<T>(IEnumerable<T> list) where T : Product
    {
        var list2 = list.Select(x => $"{(int) x.quantity},{x.amount},{x.index},{x.parameter},{x.parameter2}").ToArray();
        return string.Join(",", list2);
    }
}