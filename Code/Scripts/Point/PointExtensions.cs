using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public static class MyTransformExtensions
{
    public static IObservable<Unit> RxMoveAsObservable(this Transform transform, Vector3 destination, float speed)
    {
        return Observable.FromCoroutine(() => transform.RxMove(destination, speed));
    }

    public static int GetClosestChildrenIndex(this Transform target,Vector3 pos,out float closest)
    {
        int newSiblingIndex = -1;
         closest = Single.MaxValue;
        for (int i = 0; i < target.childCount; i++)
        {
            if (Vector3.Distance(target.GetChild(i).position, pos) < closest)
            {
                closest = Vector3.Distance(target.GetChild(i).position, pos);
                newSiblingIndex = i;
            }
        }

        return newSiblingIndex;
    }


    public static IEnumerator RxMove(this Transform transform, Vector3 destination, float speed,
        AnimationCurve curve = null)
    {
        var start = transform.position;
        var distance = Vector2.Distance(transform.position, destination);
        var duration = distance * speed;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 pos = (curve == null)
                ? Vector3.Lerp(start, destination, t / duration)
                : Vector3.Lerp(start, destination, curve.Evaluate(t / duration));
            transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        transform.position = destination;
    }

    public static IObservable<Unit> RxLocalMoveAsObservable(this Transform transform, Vector3 destination, float speed)
    {
        return Observable.FromCoroutine(() => transform.RxMove(destination, speed));
    }

    public static IEnumerator RxLocalMove(this Transform transform, Vector3 destination, float speed,
        AnimationCurve curve = null)
    {
        var start = transform.localPosition;
        var distance = Vector2.Distance(transform.localPosition, destination);
        var duration = distance * speed;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 pos = (curve == null)
                ? Vector3.Lerp(start, destination, t / duration)
                : Vector3.Lerp(start, destination, curve.Evaluate(t / duration));
            transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = destination;
    }
}


public static class MyArrayExtensions
{
    public static void MyRandomForeach<T>(this T[,] arrs, Action<T> action)
    {
        List<int> cols = Enumerable.Range(0, arrs.GetLength(0)).ToList();
        List<int> rows = Enumerable.Range(0, arrs.GetLength(1)).ToList();

        cols.Shuffle();
        rows.Shuffle();
        for (int i = 0; i < cols.Count; i++)
        for (int j = 0; j < rows.Count; j++)
        {
            action(arrs[cols[i], rows[j]]);
        }
    }


    public static void MyForeach2D<T>(this T[,] arrs, Action<int, int, T> action)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
        {
            action(i, j, arrs[i, j]);
        }
    }

    public static void MyForeach2D<T>(this T[] arrs, int nCols, Action<int, int, T> action)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        {
            int col = i % nCols;
            int row = Mathf.FloorToInt(i * 1f / nCols);
            action(col, row, arrs[i]);
        }
    }

    public static void MyForeach2D<T>(this T[] arrs, int nCols, Action<Point, T> action)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        {
            int col = i % nCols;
            int row = Mathf.FloorToInt(i * 1f / nCols);
            action(new Point(col, row), arrs[i]);
        }
    }

    public static T[] ArrayAddDistinct<T>(this T[] list, T add)
    {
        var temp = list.ToList();
        list.AddDistinct(add);
        return temp.ToArray();
    }

    public static IList<T> ArrayAdd<T>(this IList<T> list, T add)
    {
        return IeArrayAdd(list, add).ToArray();
    }

    public static IEnumerable<T> IeArrayAdd<T>(this IEnumerable<T> list, T add)
    {
        foreach (var x1 in list)
            yield return x1;
        yield return add;
    }


    public static string ConvertToString<T>(this List<T> list, string introdude)
    {
        introdude += ":";
        for (var i = 0; i < list.Count; i++)
        {
            if (i < list.Count - 1)
                introdude += list[i].ToString() + ",";
            else
                introdude += list[i].ToString();
        }

        return introdude;
    }

    public static void PrintList<T>(this List<T> list, string introdude = "")
    {
        Debug.Log(list.ConvertToString(list.ToString()));
    }

    public static int ToEpoch(this DateTime dateTime)
    {
        return dateTime.Year * 1000 + dateTime.DayOfYear;
    }

    public static PointDirection Remove(this PointDirection direction, PointDirection remove)
    {
        return direction & (~remove);
    }

    public static PointDirection Add(this PointDirection direction, PointDirection add)
    {
        return direction | (~add);
    }

    public static bool Contain(this PointDirection direction, PointDirection key)
    {
        return (direction & key) == key;
    }

    public static bool NotNull(this PointDirection direction, PointDirection key)
    {
        return (direction & key) != 0;
    }

    public static string toJoinString(this IList<string> arrs)
    {
        string join = "";

        for (var i = 0; i < arrs.Count; i++)
        {
            if (i < arrs.Count - 1)
                join += arrs[i] + ',';
            else
                join += arrs[i];
        }

        return join;
    }

    public static void Remove<T>(this T[,] arrs, Func<T, bool> predicate) where T : class
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            if (predicate(arrs[i, j]))
                arrs[i, j] = null;
    }

    public static List<Point> MyPoints<T>(this T[,] arrs)
    {
        var points = new List<Point>();
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            points.Add(new Point(i, j));

        return points;
    }
    
    public static bool[,] MyReverse(this bool[,] arrs)
    {
        var ar = new bool[arrs.GetLength(0), arrs.GetLength(1)];
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            ar[i, j] = !arrs[i, j];
        return ar;
    }

    public static List<Point> MyPoints<T>(this T[,] arrs, Func<T, bool> predicate)
    {
        var points = new List<Point>();
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            if (predicate(arrs[i, j]))
                points.Add(new Point(i, j));

        return points;
    }

    public static int Count<T>(this T[,] arrs, Func<T, bool> predicate)
    {
        int c = 0;
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            if (predicate(arrs[i, j]))
                c++;

        return c;
    }

    public static bool[,] ToBool<T>(this T[,] arrs)
    {
        return new bool[arrs.GetLength(0), arrs.GetLength(1)];
    }


    public static void MyForeach<T>(this T[,] arrs, Action<T> action)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
        {
            action(arrs[i, j]);
        }
    }

    public static void MyForeach<T>(this T[] arrs, Action<T> action)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        {
            action(arrs[i]);
        }
    }

    public static bool MyAll<T>(this T[] arrs, Func<T, bool> predicate)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
            if (!predicate(arrs[i]))
                return false;
        return true;
    }

    public static bool MyAny<T>(this T[] arrs, Func<T, bool> predicate)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
            if (predicate(arrs[i]))
                return true;
        return false;
    }

    public static bool MyAll<T>(this T[,] arrs, Func<T, bool> predicate)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            if (!predicate(arrs[i, j]))
                return false;

        return true;
    }


    public static bool MyAny<T>(this T[,] arrs, Func<T, bool> predicate)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            if (predicate(arrs[i, j]))
                return true;
        return false;
    }

    public static int MyCount<T>(this T[] arrs, Func<T, bool> predicate)
    {
        int count = 0;
        for (int i = 0; i < arrs.GetLength(0); i++)
            if (predicate(arrs[i]))
                count++;
        return count;
    }

    public static int MyCount<T>(this T[,] arrs, Func<T, bool> predicate)
    {
        int count = 0;
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
            if (predicate(arrs[i, j]))
                count++;
        return count;
    }


    public static int MyCount<T>(this T[,] arrs, Func<T, bool> predicate, Func<T, bool> where)
    {
        int count = 0;
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
        {
            if (!where(arrs[i, j]))
                continue;
            if (predicate(arrs[i, j]))
                count++;
        }

        return count;
    }

    public static T[] GetCol<T>(this T[,] arrs, int col)
    {
        var value = new T[arrs.GetLength(1)];
        for (int i = 0; i < arrs.GetLength(1); i++)
        {
            value[i] = arrs[col, i];
        }

        return value;
    }

    public static T[] GetRow<T>(this T[,] arrs, int row)
    {
        var value = new T[arrs.GetLength(0)];
        for (int i = 0; i < arrs.GetLength(1); i++)
        {
            value[i] = arrs[i, row];
        }

        return value;
    }

    public static IEnumerable<T> GetHexaCol1<T>(this T[,] arrs, bool[,] checkMap, int col)
    {
        int middleRow = Mathf.FloorToInt(arrs.GetLength(1) / 2f);
        for (int i = 0; i < arrs.GetLength(1); i++)
        {
            int x = col + Mathf.FloorToInt(i / 2f) - Mathf.FloorToInt(middleRow / 2f);
            if (arrs.Contains(x, i) && checkMap[x, i])
                yield return arrs[x, i];
        }
    }

    public static IEnumerable<T> GetHexaCol2<T>(this T[,] arrs, bool[,] checkMap, int col)
    {
        int middleRow = Mathf.FloorToInt(arrs.GetLength(1) / 2f);
        for (int i = 0; i < arrs.GetLength(1); i++)
        {
            int x = col - Mathf.CeilToInt(i / 2f) + Mathf.FloorToInt(middleRow / 2f);
            if (arrs.Contains(x, i) && checkMap[x, i])
                yield return arrs[x, i];
        }
    }

    public static IEnumerable<T> GetHexaRow<T>(this T[,] arrs, bool[,] checkMap, int row)
    {
        for (int i = 0; i < arrs.GetLength(0); i++)
        {
            if (arrs.Contains(i, row) && checkMap[i, row])
                yield return arrs[i, row];
        }
    }

    public static bool MyAll<T>(this IEnumerable<T> arrs, Func<T, bool> predicate)
    {
        foreach (var arr in arrs)
            if (!predicate(arr))
                return false;
        return true;
    }

    public static bool MyAny<T>(this IEnumerable<T> arrs, Func<T, bool> predicate)
    {
        foreach (var arr in arrs)
            if (!predicate(arr))
                return true;
        return false;
    }


    public static int Sum(this int[] arr)
    {
        var sum = 0;
        for (var i = 0; i < arr.Length; i++)
            sum += arr[i];

        return sum;
    }

    public static T[,] MyClone<T>(this T[,] arrs)
    {
        var arr2 = new T[arrs.GetLength(0), arrs.GetLength(1)];
        for (int i = 0; i < arrs.GetLength(0); i++)
        for (int j = 0; j < arrs.GetLength(1); j++)
        {
            arr2[i, j] = arrs[i, j];
        }

        return arr2;
    }

    public static int MyCount<T>(this T[,] arrs, Point root, Point[] aroundPoints, Func<T, bool> predicate)
    {
        int count = 0;
        var arounds = arrs.GetArounds(root, aroundPoints);
        if (arounds != null)
            foreach (var around in arounds)
                if (predicate(around))
                    count++;

        return count;
    }

    public static int CountNull<T>(this T[,] arrs, Point root, Point[] aroundPoints)
    {
        int countNull = 0;
        foreach (var aroundPoint in aroundPoints)
        {
            var p = aroundPoint + root;
            if (!arrs.OutOfRange(p))
            {
                var get = arrs.GetAtPoint(p);
                if (get == null)
                    countNull++;
            }
        }

        return countNull;
    }


    public static IEnumerable<T> GetArounds<T>(this T[,] arrs, Point root, Point[] aroundPoints)
    {
        foreach (var aroundPoint in aroundPoints)
        {
            var p = aroundPoint + root;
            if (!arrs.OutOfRange(p))
            {
                var get = arrs.GetAtPoint(p);
                if (get != null)
                    yield return get;
            }
        }
    }

    public static T[] GetDiagonal1<T>(this T[,] arrs, Point point)
    {
        List<T> diagonals = new List<T>();
        diagonals.Add(arrs.GetAtPoint(point));
        var pre = point.Clone(-1, -1);
        while (arrs.Contains(pre))
        {
            diagonals.Insert(0, arrs.GetAtPoint(pre));
            pre.col--;
            pre.row--;
        }

        var next = point.Clone(+1, +1);
        while (arrs.Contains(next))
        {
            diagonals.Add(arrs.GetAtPoint(next));
            next.col++;
            next.row++;
        }

        return diagonals.ToArray();
    }

    public static T[] GetDiagonal2<T>(this T[,] arrs, Point point)
    {
        List<T> diagonals = new List<T>();
        diagonals.Add(arrs.GetAtPoint(point));
        var pre = point.Clone(-1, +1);
        while (arrs.Contains(pre))
        {
            diagonals.Insert(0, arrs.GetAtPoint(pre));
            pre.col--;
            pre.row++;
        }

        var next = point.Clone(+1, -1);
        while (arrs.Contains(next))
        {
            diagonals.Add(arrs.GetAtPoint(next));
            next.col++;
            next.row--;
        }

        return diagonals.ToArray();
    }

    public static List<T[]> GetCols<T>(this T[,] arrs)
    {
        var list = new List<T[]>();

        for (int j = 0; j < arrs.GetLength(0); j++)
        {
            var value = new T[arrs.GetLength(1)];
            for (int i = 0; i < arrs.GetLength(1); i++)
            {
                value[i] = arrs[j, i];
            }

            list.Add(value);
        }

        return list;
    }

    public static List<T[]> GetRows<T>(this T[,] arrs)
    {
        var list = new List<T[]>();
        for (int j = 0; j < arrs.GetLength(0); j++)
        {
            var value = new T[arrs.GetLength(0)];
            for (int i = 0; i < arrs.GetLength(1); i++)
            {
                value[i] = arrs[i, j];
            }

            list.Add(value);
        }

        return list;
    }

    // like operator OR
    public static bool Merge(this bool[,] Map, bool[,] miniMap, Point startPoint)
    {
        if (Map.ConflictWith(miniMap, startPoint) == false)
        {
            for (int i = 0; i < miniMap.GetLength(0); i++)
            for (int j = 0; j < miniMap.GetLength(1); j++)
            {
                int col = startPoint.col + i;
                int row = startPoint.row + j;
                if (Map[col, row] == false && miniMap[i, j])
                    Map[col, row] = true;
            }

            return true;
        }

        return false;
    }

    // map[x,y] must be empty to fill
    public static bool AddMap(this bool[,] map, bool[,] miniMap, Point startPoint)
    {
        for (int i = 0; i < miniMap.GetLength(0); i++)
        for (int j = 0; j < miniMap.GetLength(1); j++)
        {
            int col = startPoint.col + i;
            int row = startPoint.row + j;
            if (col < 0 || row < 0 || col >= map.GetLength(0) || row >= map.GetLength(1))
                return false;
            if (map[col, row] && miniMap[i, j])
                return false;
            if (map[col, row] == false && miniMap[i, j])
                map[col, row] = true;
        }

        return true;
    }

    public static bool ConflictWith(this bool[,] Map, bool[,] miniMap, Point startPoint)
    {
        for (int i = 0; i < miniMap.GetLength(0); i++)
        for (int j = 0; j < miniMap.GetLength(1); j++)
        {
            int col = startPoint.col + i;
            int row = startPoint.row + j;
            if (col < 0 || row < 0 || col >= Map.GetLength(0) || row >= Map.GetLength(1))
                return true;
            if (Map[col, row] && miniMap[i, j])
                return true;
        }

        return false;
    }

    // like operator OR
    public static bool Merge(this int[,] map, int[,] miniMap, Point startPoint)
    {
        if (map.ConflictWith(miniMap, startPoint) == false)
        {
            for (int i = 0; i < miniMap.GetLength(0); i++)
            for (int j = 0; j < miniMap.GetLength(1); j++)
            {
                int col = startPoint.col + i;
                int row = startPoint.row + j;
                if (map[col, row] <= 0 && miniMap[i, j] > 0)
                    map[col, row] = miniMap[i, j];
            }

            return true;
        }

        return false;
    }

    // map[x,y] must be empty to fill
    public static bool AddMap(this int[,] map, int[,] miniMap, Point startPoint)
    {
        for (int i = 0; i < miniMap.GetLength(0); i++)
        for (int j = 0; j < miniMap.GetLength(1); j++)
        {
            int col = startPoint.col + i;
            int row = startPoint.row + j;
            if (col < 0 || row < 0 || col >= map.GetLength(0) || row >= map.GetLength(1))
                return false;
            if (map[col, row] > 0 && miniMap[i, j] > 0)
                return false;
            if (map[col, row] <= 0 && miniMap[i, j] > 0)
                map[col, row] = miniMap[i, j];
        }

        return true;
    }

    public static bool ConflictWith(this int[,] map, int[,] miniMap, Point startPoint)
    {
        for (int i = 0; i < miniMap.GetLength(0); i++)
        for (int j = 0; j < miniMap.GetLength(1); j++)
        {
            int col = startPoint.col + i;
            int row = startPoint.row + j;
            if (col < 0 || row < 0 || col >= map.GetLength(0) || row >= map.GetLength(1))
                return true;
            if (map[col, row] != 0 && miniMap[i, j] > 0)
                return true;
        }

        return false;
    }

    public static void SetAtPoint<T>(this T[,] arr, Point point, T value)
    {
        arr[point.col, point.row] = value;
    }

    public static void SetByPointDirection<T>(this T[,] arr, Point point, T value, PointDirection pointDirection)
    {
        arr[point.col, point.row] = value;
        if ((pointDirection & PointDirection.Left) != 0)
            for (int i = 0; i < point.col; i++)
                arr[i, point.row] = value;
        if ((pointDirection & PointDirection.Right) != 0)
            for (int i = point.col + 1; i < arr.GetLength(0); i++)
                arr[i, point.row] = value;
        if ((pointDirection & PointDirection.Bottom) != 0)
            for (int i = 0; i < point.row; i++)
                arr[point.col, i] = value;
        if ((pointDirection & PointDirection.Top) != 0)
            for (int i = point.row + 1; i < arr.GetLength(1); i++)
                arr[point.col, i] = value;
    }
    
    public static void SetByPointDirection<T>(this T[,] arr, Point point, T value, PointDirection pointDirection,int length)
    {
        if (length == 0)
        {
            SetByPointDirection(arr,point,value,pointDirection);
            return;
        }
        
        arr[point.col, point.row] = value;
        int c = 0;
        if ((pointDirection & PointDirection.Left) != 0)

            for (int i = point.col - 1; i >= 0; i--)
            {
                arr[i, point.row] = value;
                c++;
                if(c >= length)
                    break;
            }
        c = 0;
        if ((pointDirection & PointDirection.Right) != 0)
            for (int i = point.col + 1; i < arr.GetLength(0); i++)
            {
                arr[i, point.row] = value;
                c++;
                if(c >= length)
                    break;
            }
        c = 0;
        if ((pointDirection & PointDirection.Bottom) != 0)
            for (int i = point.row - 1; i >= 0; i--)
            {
                arr[point.col, i] = value;
                c++;
                if(c >= length)
                    break;
            }
        c = 0;
        if ((pointDirection & PointDirection.Top) != 0)
            for (int i = point.row + 1; i < arr.GetLength(1); i++)
            {
                arr[point.col, i] = value;
                c++;
                if(c >= length)
                    break;
            }
    }

    public static void SetAtCol<T>(this T[,] arr, Point point, T value)
    {
        for (int i = 0; i < arr.GetLength(1); i++)
        {
            arr[point.col, i] = value;
        }
    }
    
    
    public static void SetAtColNext<T>(this T[,] arr, Point point, T value,bool nextOrPre)
    {
        for (int i = 0; i < arr.GetLength(1); i++)
        {
            if(nextOrPre && i >= point.row)
                arr[point.col, i] = value;
            if(!nextOrPre && i <= point.row)
                arr[point.col, i] = value;
        }
    }
    
    public static void SetAtRowNext<T>(this T[,] arr, Point point, T value,bool nextOrPre)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            if(nextOrPre && i >= point.col)
                arr[i, point.row] = value;
            if(!nextOrPre && i <= point.col)
                arr[i, point.row] = value;
        }
    }

    
    
    public static void SetAtRow<T>(this T[,] arr, Point point, T value)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            arr[i, point.row] = value;
        }
    }

    public static void SetAtPoint<T, T2>(this T[,] arr, T2 value) where T2 : IPointItem, T
    {
        arr[value.Point.col, value.Point.row] = value;
    }

//    public static void RemoveAtPoint<T,T2>(this T[,] arr, T2 value)where T2 : IPointItem, T
//    {
//        arr[value.Point.col, value.Point.row] = default(T);
//    }

    public static void SetAtPoints<T>(this T[,] arr, IEnumerable<Point> points, T value)
    {
        foreach (var point in points)
            arr[point.col, point.row] = value;
    }
    
    public static void SetAtPoints<T,T2>(this T[,] arr, IEnumerable<T2> points, T value) where T2 : Point
    {
        foreach (var point in points)
            arr[point.col, point.row] = value;
    }

    public static void SetAtPoints<T>(this T[,] arr, Point point, int width, T value)
    {
        for (int i = 0; i < width; i++)
        {
            arr[point.col + i, point.row] = value;
        }
    }

    public static T GetAtPoint<T>(this T[,] arr, Point point)
    {
        if (point.col >= arr.GetLength(0) || point.row >= arr.GetLength(1) || point.col < 0 || point.row < 0)
            Debug.LogError(point.toString() + " out of array");
        return arr[point.col, point.row];
    }

    public static T GetAtPointNull<T>(this T[,] arr, Point point) where T : MonoBehaviour
    {
        if (point.col >= arr.GetLength(0) || point.row >= arr.GetLength(1) || point.col < 0 || point.row < 0)
            return null;
        return arr[point.col, point.row];
    }

    public static bool Contains<T>(this T[,] arr, Point point)
    {
        if (point.col < 0 || point.row < 0 || point.col >= arr.GetLength(0) || point.row >= arr.GetLength(1))
            return false;
        return true;
    }

    public static IEnumerable<Point> GetPoints<T>(this T[,] arr)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        for (int j = 0; j < arr.GetLength(1); j++)
            yield return new Point(i, j);
    }


    public static bool Contains<T>(this T[,] arr, int x, int y)
    {
        if (x < 0 || y < 0 || x >= arr.GetLength(0) || y >= arr.GetLength(1))
            return false;
        return true;
    }

    public static bool OutOfRange<T>(this T[,] arr, Point point)
    {
        bool outOfRange = point.col < 0 || point.row < 0 || point.col >= arr.GetLength(0) ||
                          point.row >= arr.GetLength(1);
        return outOfRange;
    }
    
    public static bool OnBorder<T>(this T[,] arr, Point point)
    {
        return point.col == 0 || point.row == 0 || point.col == arr.GetLength(0) - 1 ||
               point.row == arr.GetLength(1) - 1;
    }
    
    public static bool OutOfRange(this Vector2Int arr, Point point)
    {
        bool outOfRange = point.col < 0 || point.row < 0 || point.col >= arr.x ||
                          point.row >= arr.y;
        return outOfRange;
    }

    public static bool GetAtPoint(this bool[,] arr, Point point)
    {
        return arr[point.col, point.row];
    }


    public static TResult[,] MySelect<TSource, TResult>(this TSource[,] arr, Func<TSource, TResult> func)
    {
        TResult[,] result = new TResult[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        for (int j = 0; j < arr.GetLength(1); j++)
            result[i, j] = func(arr[i, j]);

        return result;
    }

    public static TResult[,] MySelect<TSource, TResult>(this TSource[,] arr, Func<TSource, TResult> func,
        TResult defaultWhenNull)
    {
        TResult[,] result = new TResult[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        for (int j = 0; j < arr.GetLength(1); j++)
            if (arr[i, j] == null)
                result[i, j] = defaultWhenNull;
            else
                result[i, j] = func(arr[i, j]);

        return result;
    }

    public static T[] To1DArray<T>(this T[,] arr)
    {
        var array1D = new T[arr.GetLength(0) * arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        for (int j = 0; j < arr.GetLength(1); j++)
            array1D[i + arr.GetLength(0) * j] = arr[i, j];

        return array1D;
    }

    public static T[,] To2DArray<T>(this T[] arr, int nCols)
    {
        if (arr.Length % nCols != 0)
            Debug.LogError("Array size is not valid - Must be mutiple of " + nCols + " :" + arr.Length);

        int nRows = arr.Length / nCols;
        var array2D = new T[nCols, nRows];

        for (int i = 0; i < arr.Length; i++)
        {
            int col = i % nCols;
            int row = Mathf.FloorToInt(i * 1f / nCols);
            array2D[col, row] = arr[i];
        }

        return array2D;
    }

    public static int[,] ToInt(this bool[,] arr, int setValue, int defaultValue)
    {
        int[,] map = new int[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                if (arr[i, j])
                {
                    map[i, j] = setValue;
                }
                else
                {
                    map[i, j] = defaultValue;
                }
            }
        }

        return map;
    }

    public static T[,] To2DArray<T>(this List<T> arr, int nCols)
    {
        if (arr.Count % nCols != 0)
            Debug.LogError("Array size is not valid - Must be mutiple of " + nCols + " :" + arr.Count);

        int nRows = arr.Count / nCols;
        var array2D = new T[nCols, nRows];

        for (int i = 0; i < arr.Count; i++)
        {
            int col = i % nCols;
            int row = Mathf.FloorToInt(i * 1f / nCols);
            array2D[col, row] = arr[i];
        }

        return array2D;
    }

    public static T TakeRandom<T>(this IEnumerable<T> list)
    {
        return list.ToArray()[UnityEngine.Random.Range(0, list.Count())];
    }

    public static IEnumerable<T> TakeRandom<T>(this IList<T> list, int amount)
    {
        if (amount > list.Count)
            Debug.LogError("Out of Array");
        var copy = list.ToList();
        for (int i = 0; i < amount; i++)
        {
            var index = UnityEngine.Random.Range(0, copy.Count);
            var value = copy[index];
            copy.RemoveAt(index);
            yield return value;
        }
    }


    public static T TakeOutOne<T>(this List<T> list)
    {
        var index = UnityEngine.Random.Range(0, list.Count);
        var value = list[index];
        list.RemoveAt(index);
        return value;
    }

    public static void Populate<T>(this List<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = value;
        }
    }
    
    public static void Translate<T>(this IList<T> list, int translate)
    {
        IList<T> ts = new T[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            ts[i] = list[(i + translate) % list.Count];
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = ts[i];
        }
    }

    public static void Populate<T>(this T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = value;
        }
    }


    public static Vector2Int GetVectorSize<T>(this T[,] arr)
    {
        return new Vector2Int(arr.GetLength(0), arr.GetLength(1));
    }

    public static void Populate<T>(this T[,] arr, T value)
    {
        for (int i = 0; i < arr.GetLength(0); i++)
        for (int j = 0; j < arr.GetLength(1); j++)
            arr[i, j] = value;
    }

    public static void Populate<T>(this T[,] arr, int Col, int Row, T value)
    {
        if (Col >= 0)
            for (int j = 0; j < arr.GetLength(1); j++)
                arr[Col, j] = value;

        if (Row >= 0)
            for (int i = 0; i < arr.GetLength(0); i++)
                arr[i, Row] = value;
    }

    public static T[,] To2D<T>(this T[] map, int nCols, int nRows)
    {
        if (map.Length != nCols * nRows)
        {
            Debug.LogError(String.Format("Length of array is not equal {0} != {1} * {2}", map.Length, nCols, nRows));
            return null;
        }

        var map2D = new T[nCols, nRows];
        for (int i = 0; i < map.Length; i++)
        {
            int col = i % nCols;
            int row = Mathf.CeilToInt(i / nCols);
            map2D[col, row] = map[i];
        }

        return map2D;
    }

    public static bool[,] To2D(this bool[] Map, int nCols, int nRows)
    {
        if (Map.Length != nCols * nRows)
        {
            Debug.LogError(String.Format("Length of array is not equal {0} != {1} * {2}", Map.Length, nCols, nRows));
            return null;
        }

        bool[,] map2D = new bool[nCols, nRows];
        for (int i = 0; i < Map.Length; i++)
        {
            int col = i % nCols;
            int row = Mathf.CeilToInt(i / nCols);
            map2D[i % nCols, Mathf.CeilToInt(i / nCols)] = Map[i];
        }

        return map2D;
    }

    public static bool[,] ToFillMap(this List<Point> points)
    {
        var minCol = points.Min(x => x.col);
        var maxCol = points.Max(x => x.col);
        var minRow = points.Min(x => x.row);
        var maxRow = points.Max(x => x.row);

        bool[,] map = new bool[maxCol - minCol + 1, maxRow - minRow + 1];
        foreach (var point in points)
            map[point.col - minCol, point.row - minRow] = true;

        return map;
    }
}


public static class MyUIExtensions
{
    public static void CopySizeOf(this RectTransform thisRectTransform, RectTransform copyOf)
    {
        var center = new Vector2(0.5f, 0.5f);
        thisRectTransform.pivot = center;
        thisRectTransform.anchorMin = center;
        thisRectTransform.anchorMax = center;
        var size = copyOf.rect.size;
        thisRectTransform.sizeDelta = size;
        thisRectTransform.position = copyOf.position;
        thisRectTransform.localPosition = thisRectTransform.localPosition +
                                          new Vector3((center.x - copyOf.pivot.x) * size.x,
                                              (center.y - copyOf.pivot.y) * size.y);
    }

    public static void FadeIn(this Image img, float time, float delay = 0f, TweenCallback onComplete = null,
        Ease ease = Ease.Linear)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
        if (onComplete != null)
            img.DOFade(1f, time).SetEase(ease).SetDelay(delay).OnComplete(onComplete);
        else
            img.DOFade(1f, time).SetEase(ease).SetDelay(delay);
    }

    public static void FadeOut(this Image img, float time, float delay = 0f, TweenCallback onComplete = null,
        Ease ease = Ease.Linear)
    {
        if (onComplete != null)
            img.DOFade(0f, time).SetEase(ease).SetDelay(delay).OnComplete(onComplete);
        else
            img.DOFade(0f, time).SetEase(ease).SetDelay(delay);
    }

    public static void MoveTo(this Transform transform, Point to, float speed, float delay = 0f,
        TweenCallback onComplete = null, Ease ease = Ease.Linear)
    {
        var from = transform.localPosition;
        var des = to.GetPos();
        var duration = Vector2.Distance(from, des) / Point.PointSize * speed;

        if (onComplete != null)
            transform.DOLocalMove(des, duration).SetEase(ease).SetDelay(delay).OnComplete(onComplete);
        else
            transform.DOLocalMove(des, duration).SetDelay(delay).SetEase(ease);
    }

    public static void MoveTo(this Transform transform, Point from, Point to, float speed, float delay = 0f,
        TweenCallback onComplete = null, Ease ease = Ease.Linear)
    {
        var fro = from.GetPos();
        var des = to.GetPos();
        var duration = Vector2.Distance(fro, des) / Point.PointSize * speed;

        transform.localPosition = fro;
        if (onComplete != null)
            transform.DOLocalMove(des, duration).SetDelay(delay).SetEase(ease).OnComplete(onComplete);
        else
            transform.DOLocalMove(des, duration).SetDelay(delay).SetEase(ease);
    }

    public static void MoveTo(this Transform transform, Point to, Vector2 scaleTo, float speed, float delay = 0f,
        TweenCallback onComplete = null, Ease ease = Ease.Linear)
    {
        var from = transform.localPosition;
        var des = to.GetPos();
        var duration = Vector2.Distance(from, des) / Point.PointSize * speed;

        transform.DOScale(scaleTo, duration);
        if (onComplete != null)
            transform.DOLocalMove(des, duration).SetEase(ease).SetDelay(delay).OnComplete(onComplete);
        else
            transform.DOLocalMove(des, duration).SetDelay(delay).SetEase(ease);
    }

    public static void MoveTo(this Transform transform, Point from, Point to, Vector2 scaleTo, float speed,
        float delay = 0f, TweenCallback onComplete = null, Ease ease = Ease.Linear)
    {
        var fro = from.GetPos();
        var des = to.GetPos();
        var duration = Vector2.Distance(fro, des) / Point.PointSize * speed;

        transform.localPosition = fro;
        transform.DOScale(scaleTo, duration);
        if (onComplete != null)
            transform.DOLocalMove(des, duration).SetDelay(delay).SetEase(ease).OnComplete(onComplete);
        else
            transform.DOLocalMove(des, duration).SetDelay(delay).SetEase(ease);
    }
}

public static class MyIEnumeratorExtensions
{
    public static IObservable<Unit> IoValue(float from, float to, float speed, Action<float> onUpdate, float delay = 0f)
    {
        return Observable.FromCoroutine(() => IeValue(from, to, speed, onUpdate, delay));
    }

    public static IObservable<Unit> IoValue(float from, float to, float time, AnimationCurve curve,
        Action<float> onUpdate, float delay = 0f)
    {
        return Observable.FromCoroutine(() => IeValue(from, to, time, curve, onUpdate, delay));
    }

    public static IObservable<Unit> IoValue(Vector3 from, Vector3 to, float speed, Action<Vector3> onUpdate,
        float delay = 0f)
    {
        return Observable.FromCoroutine(() => IeValue(from, to, speed, onUpdate, delay));
    }

    public static IObservable<Unit> IoValue(Vector3 from, Vector3 to, float time, AnimationCurve curve,
        Action<Vector3> onUpdate, float delay = 0f)
    {
        return Observable.FromCoroutine(() => IeValue(from, to, time, curve, onUpdate, delay));
    }

    private static IEnumerator IeValue(float from, float to, float speed, Action<float> onUpdate, float delay = 0f)
    {
        var s = (to - from);
        var time = s / speed;

        float t = 0f;

        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < time)
        {
            onUpdate(Mathf.Lerp(from, to, t / time));
            t += Time.deltaTime;
            yield return null;
        }

        onUpdate(to);
    }

    private static IEnumerator IeValue(float from, float to, float time, AnimationCurve curve, Action<float> onUpdate,
        float delay = 0f)
    {
        float t = 0f;
        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < time)
        {
            onUpdate(Mathf.Lerp(from, to, curve.Evaluate(t / time)));
            t += Time.deltaTime;
            yield return null;
        }

        onUpdate(to);
    }

    private static IEnumerator IeValue(Vector3 from, Vector3 to, float speed, Action<Vector3> onUpdate,
        float delay = 0f)
    {
        var s = (to - from).magnitude;
        var time = s / speed;
        float t = 0f;
        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < time)
        {
            onUpdate(Vector3.Lerp(from, to, t / time));
            t += Time.deltaTime;
            yield return null;
        }

        onUpdate(to);
    }

    private static IEnumerator IeValue(Vector3 from, Vector3 to, float time, AnimationCurve curve,
        Action<Vector3> onUpdate, float delay = 0f)
    {
        var s = (to - from);

        float t = 0f;
        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        while (t < time)
        {
            onUpdate(Vector3.Lerp(from, to, curve.Evaluate(t / time)));
            t += Time.deltaTime;
            yield return null;
        }

        onUpdate(to);
    }

    public static IObservable<Unit> ToObservable(this Action action)
    {
        return Observable.FromCoroutine(() => ActionAsIEnumerator(action));
    }

    static IEnumerator ActionAsIEnumerator(Action action)
    {
        action.Invoke();
        yield return null;
    }


    public static IObservable<Unit> SetActiveAsObservable(this GameObject gameObject, bool active)
    {
        Debug.Log("SetActiveAsObservable");

        return Observable.FromCoroutine(() => IESetActive(gameObject, active));
    }

    private static IEnumerator IESetActive(this GameObject gameObject, bool active)
    {
        gameObject.SetActive(active);
        Debug.Break();
        yield return null;
    }

    public static IObservable<Unit> DoValue(this FloatReactiveProperty property, float to, float time, float delay = 0f)
    {
        return Observable.FromCoroutine(() => IEDoValue(property, to, time, delay));
    }

    private static IEnumerator IEDoValue(this FloatReactiveProperty property, float to, float time, float delay = 0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        var t = 0f;
        while (t < time)
        {
            var lastDeltaTime = t;
            t += Time.deltaTime;
            if (t > time)
                t = time;
            property.Value += to * ((t - lastDeltaTime) / time);
            yield return new WaitForEndOfFrame();
        }
    }

    public static IObservable<Unit> DoValue(this IntReactiveProperty property, int to, float time, float delay = 0f)
    {
        return Observable.FromCoroutine(() => IEDoValue(property, to, time, delay));
    }

    private static IEnumerator IEDoValue(this IntReactiveProperty property, int to, float time, float delay = 0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        var startValue = property.Value;
        var t = 0f;
        while (t < time)
        {
            if (t > time)
                t = time;
            property.Value = startValue + Mathf.FloorToInt((t / time) * to);
            Debug.Log(property.Value);
            yield return new WaitForEndOfFrame();
        }
    }

    public static IObservable<bool> WaitTo(this BoolReactiveProperty property, bool value)
    {
        return property.DistinctUntilChanged().Where(x => x == value).Take(1);
    }

    public static IObservable<Unit> WaitTo(this List<BoolReactiveProperty> properties, bool value)
    {
        return Observable.FromCoroutine(() => IEWaitTo(properties, value));
    }

    private static IEnumerator IEWaitTo(List<BoolReactiveProperty> properties, bool value)
    {
        yield return new WaitForEndOfFrame();
        while (properties.Any(x => x.Value != value))
        {
            yield return new WaitForEndOfFrame();
        }
    }
}

public static class MyViewExtensions
{
}

public static class PointExtension
{
    public static int[] CountBitMask = new int[]
    {
        0, // 0
        1, // 1
        1, // 2
        2, // 3
        1, // 4
        1, // 5
        2, // 6
        3, // 7
        1, // 8
        2, // 9
        2, // 10
        3, // 11
        2, // 12
        3, // 13
        3, // 14
        4, // 15
    };

    private static readonly PointDirectionBase[] PointDirectionBaseConvert =
    {
        PointDirectionBase.Undefined, // 0
        PointDirectionBase.Top, // 1
        PointDirectionBase.Right, // 2
        PointDirectionBase.Undefined, // 3
        PointDirectionBase.Bottom, // 4
        PointDirectionBase.Right, // 5
        PointDirectionBase.Right, // 6
        PointDirectionBase.Right, // 7
        PointDirectionBase.Left, // 8
    };

    private static readonly PointDirectionBase[] ReversePointDirectionBase =
    {
        PointDirectionBase.Bottom, // 1
        PointDirectionBase.Left, // 2
        PointDirectionBase.Top, // 3
        PointDirectionBase.Right, // 4
    };

    public static PointDirectionBase Reverse(this PointDirectionBase point)
    {
        return ReversePointDirectionBase[(int) point];
    }

    public static PointDirectionBase ToBaseDirection(this PointDirection point)
    {
        return PointDirectionBaseConvert[(int) point];
    }
    
    public static PointDirection Reverse(this PointDirection point)
    {
        switch (point)
        {
            case PointDirection.None:
                return PointDirection.None;
            case PointDirection.Top:
                return PointDirection.Bottom;
            case PointDirection.Right:
                return PointDirection.Left;
            case PointDirection.Bottom:
                return PointDirection.Top;
            case PointDirection.Left:
                return PointDirection.Right;
            case PointDirection.Vertical:
                return PointDirection.Vertical;
            case PointDirection.Horizontal:
                return PointDirection.Horizontal;
            case PointDirection.TopRight:
                return PointDirection.BottomLeft;
            case PointDirection.BottomRight:
                return PointDirection.TopLeft;
            case PointDirection.TopLeft:
                return PointDirection.BottomRight;
            case PointDirection.BottomLeft:
                return PointDirection.TopRight;
            case PointDirection.All4Direction:
                return PointDirection.All4Direction;
            default:
                throw new ArgumentOutOfRangeException(nameof(point), point, null);
        }
    }

//    None = 0,
//    Top = 1,
//    Right = 2,
//    Bottom = 4,
//    Left = 8,
//    Vertical = 5,
//    Horizontal = 10,
//    TopRight = 3,
//    BottomRight = 6,
//    TopLeft = 11,
//    BottomLeft = 12,

    public static (Point, Point) Divide(this Point p, int divide)
    {
        var original = new Point(Mathf.FloorToInt(p.col * 1f / divide), Mathf.FloorToInt(p.row * 1f / divide));
        var adjust = new Point(p.col % divide, p.col % divide);
        if (adjust.col > divide / 2f)
        {
            original.col += 1;
            adjust.col -= divide;
        }
        if (adjust.row > divide / 2f)
        {
            original.row += 1;
            adjust.row -= divide;
        }
        return (original, adjust);
    }
    
    public static (PointParameterBase, Point) Divide(this PointParameterBase p, int divide)
    {
        var original = new PointParameterBase(Mathf.RoundToInt(p.col * 1f / divide), Mathf.RoundToInt(p.row * 1f / divide),p.value,p.value2,p.value3,p.value4);
        
        var adjust = new Point(p.col - original.col * divide, p.row - original.row * divide);
//        if (adjust.col < 0)
//        {
//            adjust.col += divide;
//            original.col--;
//        }
//        if (adjust.row < 0)
//        {
//            adjust.row += divide;
//            original.row--;
//        }
//        if (adjust.col > divide / 2f)
//        {
//            original.col += 1;
//            adjust.col -= divide;
//        }
//        if (adjust.row > divide / 2f)
//        {
//            original.row += 1;
//            adjust.row -= divide;
//        }
        return (original, adjust);
    }

    public static int GetLength(this IList<Point> points)
    {
        int sum = 0;
        for (var index = 1; index < points.Count; index++)
            sum += points[index - 1].StepDistance(points[index]);
        return sum;
    }
    
    public static bool OutOfRange(this Point point, int range)
    {
        bool outOfRange = point.col < 0 || point.row < 0 || point.col >= range || point.row >= range;
        return outOfRange;
    }

    public static bool OutOfRange<T>(Point point, T[,] arr)
    {
        bool outOfRange = point.col < 0 || point.row < 0 || point.col >= arr.GetLength(0) ||
                          point.row >= arr.GetLength(1);
        return outOfRange;
    }


    public static Point ClosestPoint(this Dictionary<Point, Vector3> dictionary, Vector3 position, float distance)
    {
        Point point = null;
        float min = distance;

        foreach (var keyValuePair in dictionary)
        {
            var d = Vector3.Distance(keyValuePair.Value, position);
            if (d < min)
            {
                point = keyValuePair.Key;
                min = d;
            }
        }

        return point;
    }

    public static PointDirectionMove GetDirectionMove(this Point thisPoint, Point previousPoint, Point nextPoint,
        float curveAnchorRate = -0.05f)
    {
        if (previousPoint == null && nextPoint == null)
        {
            Debug.LogError("Invalid input");
            return new PointDirectionMove(thisPoint, PointDirection.None);
        }

        var inDirection = previousPoint == null ? PointDirection.None : (previousPoint - thisPoint).ToPointDirection();
        var outDirection = nextPoint == null ? PointDirection.None : (nextPoint - thisPoint).ToPointDirection();

        if (inDirection == PointDirection.None)
            return new PointDirectionMove(thisPoint, outDirection, false, curveAnchorRate);
        if (outDirection == PointDirection.None)
            return new PointDirectionMove(thisPoint, inDirection, true, curveAnchorRate);

        if (inDirection == PointDirection.Top && outDirection == PointDirection.Right)
            return new PointDirectionMove(thisPoint, PointDirection.TopRight, false, curveAnchorRate);
        if (inDirection == PointDirection.Right && outDirection == PointDirection.Top)
            return new PointDirectionMove(thisPoint, PointDirection.TopRight, true, curveAnchorRate);
        if (inDirection == PointDirection.Bottom && outDirection == PointDirection.Right)
            return new PointDirectionMove(thisPoint, PointDirection.BottomRight, false, curveAnchorRate);
        if (inDirection == PointDirection.Right && outDirection == PointDirection.Bottom)
            return new PointDirectionMove(thisPoint, PointDirection.BottomRight, true, curveAnchorRate);
        if (inDirection == PointDirection.Top && outDirection == PointDirection.Left)
            return new PointDirectionMove(thisPoint, PointDirection.TopLeft, false, curveAnchorRate);
        if (inDirection == PointDirection.Left && outDirection == PointDirection.Top)
            return new PointDirectionMove(thisPoint, PointDirection.TopLeft, true, curveAnchorRate);
        if (inDirection == PointDirection.Bottom && outDirection == PointDirection.Left)
            return new PointDirectionMove(thisPoint, PointDirection.BottomLeft, false, curveAnchorRate);
        if (inDirection == PointDirection.Left && outDirection == PointDirection.Bottom)
            return new PointDirectionMove(thisPoint, PointDirection.BottomLeft, true, curveAnchorRate);
        if (inDirection == PointDirection.Left && outDirection == PointDirection.Right)
            return new PointDirectionMove(thisPoint, PointDirection.Horizontal, false, curveAnchorRate);
        if (inDirection == PointDirection.Right && outDirection == PointDirection.Left)
            return new PointDirectionMove(thisPoint, PointDirection.Horizontal, true, curveAnchorRate);
        if (inDirection == PointDirection.Top && outDirection == PointDirection.Bottom)
            return new PointDirectionMove(thisPoint, PointDirection.Vertical, true, curveAnchorRate);
        if (inDirection == PointDirection.Bottom && outDirection == PointDirection.Top)
            return new PointDirectionMove(thisPoint, PointDirection.Vertical, false, curveAnchorRate);

        Debug.LogError("Invalid input");
        return new PointDirectionMove(thisPoint, PointDirection.None);
    }

    public static bool IsOpposite(this PointDirection point, PointDirection point2)
    {
        return  (point | point2) == PointDirection.Horizontal ||
                (point | point2) == PointDirection.Vertical;
    }
    
    public static bool IsSameLine(this PointDirection point, PointDirection point2)
    {
        if (point == point2)
            return true;
        if ((point | point2) == PointDirection.Horizontal ||
            (point | point2) == PointDirection.Vertical)
            return true;

        return false;
    }

    public static PointDirection ToPointDirection(this Point thisPoint)
    {
        if (thisPoint.col == 0 && thisPoint.row == 1)
            return PointDirection.Top;
        if (thisPoint.col == 0 && thisPoint.row == -1)
            return PointDirection.Bottom;
        if (thisPoint.col == 1 && thisPoint.row == 0)
            return PointDirection.Right;
        if (thisPoint.col == -1 && thisPoint.row == 0)
            return PointDirection.Left;
        if (thisPoint.col == 1 && thisPoint.row == 1)
            return PointDirection.TopRight;
        if (thisPoint.col == -1 && thisPoint.row == 1)
            return PointDirection.TopLeft;
        if (thisPoint.col == 1 && thisPoint.row == -1)
            return PointDirection.BottomRight;
        if (thisPoint.col == -1 && thisPoint.row == -1)
            return PointDirection.BottomLeft;
        if (thisPoint.col == 0 && thisPoint.row == 0)
            return PointDirection.None;

        Debug.LogError("Undefined Direction");
        return PointDirection.None;
    }

    public static Point PointInDirection(this Point point, PointDirection direction)
    {
        if (direction == PointDirection.Top)
            return point.Clone(0, 1);
        if (direction == PointDirection.Bottom)
            return point.Clone(0, -1);
        if (direction == PointDirection.Left)
            return point.Clone(-1, 0);
        if (direction == PointDirection.Right)
            return point.Clone(1, 0);

        Debug.LogError("direction it not valid");
        return null;
    }

    public static PointMove ToPointMove(this Point point, PointMoveType moveType, Point from = null, float delay = 0f)
    {
        return new PointMove {To = point, MoveType = moveType, From = from, Delay = delay};
    }

    public static PointParameter ToPointParameter(this Point point, int value, int value2)
    {
        return new PointParameter(point, value, value2);
    }
    
    public static PointParameter ToPointParameter(this Point point, int value, int value2, int value3, int value4)
    {
        return new PointParameter(point, value, value2,value3,value4);
    }
    
    public static PointParameter ToPointParameter(this Point point, int value)
    {
        return new PointParameter(point, value);
    }


    public static PointBoolean ToPointBoolean(this Point point, bool value)
    {
        return new PointBoolean {col = point.col, row = point.row, value = value};
    }

    public static ValuesPoint ToValuesPoint(this Point point, int[] values)
    {
        return new ValuesPoint {col = point.col, row = point.row, values = values};
    }

    // is even
    public static bool ContainsPoint(this List<Point> points, Point other)
    {
        foreach (var point in points)
        {
            if (point == other)
                return true;
        }

        return false;
    }

    // is even
    public static bool IsEven(this Point point)
    {
        if (point == null)
            return true;
        if (point.row % 2 == 0)
            return (point.col % 2 == 0);
        return (point.col % 2 == 1);
    }

    public static float GetDuration(this Point point, Vector2 pos, float timePerStep)
    {
        if (Math.Abs(Point.PointSize) > 0)
            return (Vector2.Distance(pos, point.GetPos()) / Point.PointSize) * timePerStep;
        return 0f;
    }

    public static bool GetBit(this byte byt, int index)
    {
        // Get a single bit in the proper position.
        byte bitMask = (byte) (1 << index);

        // Mask out the appropriate bit.
        byte masked = (byte) (byt & bitMask);

        // If masked != 0, then the masked out bit is 1.
        // Otherwise, masked will be 0.
        return masked != 0;
    }

    public static bool GetBit(this int byt, int index)
    {
        // Get a single bit in the proper position.
        int bitMask = (int) (1 << index);

        // Mask out the appropriate bit.
        int masked = (int) (byt & bitMask);

        // If masked != 0, then the masked out bit is 1.
        // Otherwise, masked will be 0.
        return masked != 0;
    }
    
    public static int Union(this int byt, int index)
    {
        return byt | 1 << index;
    }
    
    public static int Remove(this int byt, int index)
    {
       return byt & ~ (1<< index);
    }

    public static int GetBitAsInt(this byte byt, int index)
    {
        return GetBit(byt, index) ? 1 : 0;
    }

    public static Point ToPoint(this int index, int nCols)
    {
        return new Point(index % nCols, Mathf.FloorToInt(index * 1f / nCols));
    }
    public static PointParameter ToPoint3(this int i, int column,int row)
    {
        var z = Mathf.FloorToInt(i *1f/ (column * row));
        var index = i % (column * row);
        return new PointParameter(index % column,Mathf.FloorToInt(index * 1f / column),z);
    }

    public static PointParameter ToPoint3(this int i, Vector2Int size) => ToPoint3(i, size.x, size.y);


    // for hint ToIndex int 2D Array
    public static int ToIndex(this Point point, int nCols)
    {
        return point.row * nCols + point.col;
    }

    public static int ToIndex(int col, int row, int nCols)
    {
        return row * nCols + col;
    }

    public static int RotateBitmask(int originBitmask, int rotateAngle)
    {
        if (rotateAngle == 1)
        {
            return
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 1)) << 0 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 2)) << 1 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 3)) << 2 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 0)) << 3;
        }
        else if (rotateAngle == 2)
        {
            return
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 2)) << 0 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 3)) << 1 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 0)) << 2 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 1)) << 3;
        }
        else if (rotateAngle == 3)
        {
            return
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 3)) << 0 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 0)) << 1 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 1)) << 2 |
                Convert.ToInt32(GetBit(Convert.ToByte(originBitmask), 2)) << 3;
        }
        else
        {
            return originBitmask;
        }
    }

    /// <summary>
    /// Rotate a Point'sIndex in Board
    /// </summary>
    /// <param name="originIndex">orgin index</param>
    /// <param name="RotateAngle">0 = 0',1 = 90',2 = 180',3 = 270'</param>
    /// <param name="nUnit">nUnit = nCols,nRows</param>
    /// <returns></returns>
    public static int RotateIndex(int originIndex, int RotateAngle, int nUnit)
    {
        var originPoint = originIndex.ToPoint(nUnit);

        if (RotateAngle == 1)
            return new Point(originPoint.row, nUnit - 1 - originPoint.col).ToIndex(nUnit); // +270 == -90
        else if (RotateAngle == 2)
            return new Point(nUnit - 1 - originPoint.col, nUnit - 1 - originPoint.row).ToIndex(nUnit); // 180 = -180
        else if (RotateAngle == 3)
            return new Point(nUnit - 1 - originPoint.row, originPoint.col).ToIndex(nUnit); // +90 = 270
        else
            return originPoint.ToIndex(nUnit);
    }

    /// <summary>
    /// Rotate a Point in Board
    /// </summary>
    /// <param name="originPoint">origin Point</param>
    /// <param name="RotateAngle">0 = 0',1 = 90',2 = 180',3 = 270'</param>
    /// <param name="nUnit">nUnit = nCols,nRows</param>
    /// <returns></returns>
    public static Point RotatePoint(Point originPoint, int RotateAngle, int nUnit)
    {
        if (RotateAngle == 1)
            return new Point(originPoint.row, nUnit - 1 - originPoint.col); // +270 == -90
        else if (RotateAngle == 2)
            return new Point(nUnit - 1 - originPoint.col, nUnit - 1 - originPoint.row); // 180 = -180
        else if (RotateAngle == 3)
            return new Point(nUnit - 1 - originPoint.row, originPoint.col); // +90 = 270
        else return originPoint;
    }

    public static void Rotate90(this Point point, int nRotate)
    {
        for (int j = 0; j < nRotate % 4; j++)
        {
            int row = -point.col;
            int col = point.row;
            point.col = col;
            point.row = row;
        }
    }

    public static void Rotate90(this PointPoint point, int nRotate)
    {
        for (int j = 0; j < nRotate % 4; j++)
        {
            int row = -point.col;
            int col = point.row;
            point.col = col;
            point.row = row;

            row = -point.col2;
            col = point.row2;
            point.col2 = col;
            point.row2 = row;
        }
    }


    public static Vector3 Rotate90Xz(this Vector3 point, int nRotate)
    {
        float z = point.z;
        float x = point.x;
        for (int j = 0; j < nRotate % 4; j++)
        {
            float row = -x;
            float col = z;

            z = row;
            x = col;
        }

        return new Vector3(x, 0, z);
    }

    public static bool LineContains(Point @from, Point to, Point vehicleViewPoint)
    {
        if (from.col == to.col && from.col == vehicleViewPoint.col)
        {
            if (from.col > to.col)
            {
                if (vehicleViewPoint.col <= to.col && vehicleViewPoint.col >= from.col)
                    return true;
            }
            else
            {
                if (vehicleViewPoint.col >= to.col && vehicleViewPoint.col <= from.col)
                    return true;
            }
        }
        if (from.row == to.row && from.row == vehicleViewPoint.row)
        {
            if (from.row > to.row)
            {
                if (vehicleViewPoint.row <= to.row && vehicleViewPoint.row >= from.row)
                    return true;
            }
            else
            {
                if (vehicleViewPoint.row >= to.row && vehicleViewPoint.row <= from.row)
                    return true;
            }
        }

        return false;
    }
}