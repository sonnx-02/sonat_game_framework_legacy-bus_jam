using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class OilSpillAlgorithm
{
    public static List<Point3> GetOilSpill(bool[,] map, Point root)
    {
        int level = 0;
        var list = new List<Point3>()
        {
            new Point3(root.col, root.row, level)
        };

        var checklist = list.ToList();
        bool[,] checkMap = new bool[map.GetLength(0), map.GetLength(1)];

        while (checklist.Count > 0)
        {
            level++;
            var newCheckList = new List<Point3>();
            foreach (var point3 in checklist)
            {
                var oilSpill = OilSpill(map,  checkMap, point3, level).ToArray();
                newCheckList.AddRange(oilSpill);
                list.AddRange(oilSpill);
            }
            checklist = newCheckList;
        }

        return list;
    }
    
    public static bool GetOilSpill(bool[,] map, Point root, Func<Point, bool> predicate)
    {
        int level = 0;
        var list = new List<Point3>()
        {
            new Point3(root.col, root.row, level)
        };

        if (predicate(root))
            return true;
        
        var checklist = list.ToList();
        bool[,] checkMap = new bool[map.GetLength(0), map.GetLength(1)];
        while (checklist.Count > 0)
        {
            level++;
            var newCheckList = new List<Point3>();
            foreach (var point3 in checklist)
            {
                var oilSpill = OilSpill(map, checkMap, point3, level).ToArray();
                newCheckList.AddRange(oilSpill);
                list.AddRange(oilSpill);
                foreach (var point4 in oilSpill)
                {
                    if (predicate(point4))
                        return true;
                }
            }
            checklist = newCheckList;
        }
        return false;
    }

    private static Point[] adjacent = new[] {Point.Bottom, Point.Left, Point.Right, Point.Top,};
    private static IEnumerable<Point3> OilSpill(bool[,] map, bool[,] check, Point root, int step)
    {
        foreach (var point in adjacent)
        {
            var adj = root + point;
            if (!map.OutOfRange(adj) && !map.GetAtPoint(adj) && !check.GetAtPoint(adj))
            {
                check.SetAtPoint(adj,true);
                yield return new Point3(adj.col,adj.row,step);
            }
        }
      
    }
}