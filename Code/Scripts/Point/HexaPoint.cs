using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
// ReSharper disable CheckNamespace


public partial class Point
{
    public static bool[,] CreateIsoscelesHexangonMap(int Unit)
    {
        bool[,] map = new bool[Unit * 2 - 1, Unit * 2 - 1];
        var centerPoint = new Point(Unit - 1, Unit - 1);
        var points = new List<Point>();
        points.Add(centerPoint);
        var adjacents = centerPoint.GetHorizonalHexaAdjacents(map.GetLength(0),map.GetLength(1));

        int count = 0;
        do
        {
            points.AddRange(adjacents);
            var newAdjacents = new List<Point>();
            foreach (var point in adjacents)
            {
                var adjacentsOfPoint = point.GetHorizonalHexaAdjacents(map.GetLength(0), map.GetLength(1));
                foreach (var newAdjacent in adjacentsOfPoint)
                {
                    if (points.Find(x => x.col == newAdjacent.col && x.row == newAdjacent.row) == null && newAdjacents.Find(x => x.col == newAdjacent.col && x.row == newAdjacent.row) == null)
                        newAdjacents.Add(newAdjacent);
                }
            }
            adjacents = newAdjacents;
            count++;
        }
        while (adjacents.Count > 0 && count < Unit - 1);

        foreach (var hexaPoint in points)
            map[hexaPoint.col, hexaPoint.row] = true;

        return map;
    }

    // horizontal
    public virtual List<Point> GetHorizonalHexaAdjacents<T>(T[,] map, Func<T, bool> predicate)
    {
        var adjacents = new List<Point>();
        if (row % 2 == 0)
        {
            if (col > 0 && predicate(map[col - 1, row])) 
                adjacents.Add(new Point(col - 1, row));
            if (col < map.GetLength(0) - 1 && predicate(map[col + 1, row])) 
                adjacents.Add(new Point(col + 1, row));
            if (row > 0 && predicate(map[col, row - 1])) 
                adjacents.Add(new Point(col, row - 1));
            if (row < map.GetLength(1) - 1 && predicate(map[col, row + 1]))
                adjacents.Add(new Point(col, row + 1));
            if (col > 0 && row > 0 && predicate(map[col - 1, row - 1])) 
                adjacents.Add(new Point(col - 1, row - 1));
            if (col > 0 && row < map.GetLength(1) - 1 && predicate(map[col - 1, row + 1]))
                adjacents.Add(new Point(col - 1, row + 1));
        }
        else
        {
            if (col > 0 && predicate(map[col - 1, row])) // left
                adjacents.Add(new Point(col - 1, row));
            if (col < map.GetLength(0) - 1 && predicate(map[col + 1, row])) // right
                adjacents.Add(new Point(col + 1, row));
            if (row > 0 && predicate(map[col, row - 1])) // bottom right
                adjacents.Add(new Point(col, row - 1));
            if (row < map.GetLength(1) - 1 && predicate(map[col, row + 1])) // top right
                adjacents.Add(new Point(col, row + 1));
            if (col < map.GetLength(0) && row > 0 && predicate(map[col + 1, row - 1])) // bottom left
                adjacents.Add(new Point(col + 1, row - 1));
            if (col < map.GetLength(0) && row < map.GetLength(1) - 1 && predicate(map[col + 1, row + 1])) // top right
                adjacents.Add(new Point(col + 1, row + 1));
        }

        return adjacents;
    }

    public virtual List<Point> GetHorizonalHexaAdjacents<T>(T[,] map)
    {
        return GetHorizonalHexaAdjacents(map.GetLength(0), map.GetLength(1));
    }

    public List<Point> GetHorizonalHexaAdjacents(int totalCol, int totalRow)
    {
        var adjacents = new List<Point>();
        if (row % 2 == 0)
        {
            if (col > 0)
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row));
            if (row > 0)
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow- 1) 
                adjacents.Add(new Point(col, row + 1));
            if (col > 0 && row > 0)
                adjacents.Add(new Point(col - 1, row - 1));
            if (col > 0 && row < totalRow- 1)
                adjacents.Add(new Point(col - 1, row + 1));
        }
        else
        {
            if (col > 0) 
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol- 1)
                adjacents.Add(new Point(col + 1, row));
            if (row > 0) 
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow- 1) 
                adjacents.Add(new Point(col, row + 1));
            if (col < totalCol&& row > 0) 
                adjacents.Add(new Point(col + 1, row - 1));
            if (col < totalCol&& row < totalRow- 1)
                adjacents.Add(new Point(col + 1, row + 1));
        }

        return adjacents;
    }


    // vertical 
    public virtual List<Point> GetVerticalHexaAdjacents<T>(T[,] map, Func<T, bool> predicate)
    {
        int totalCol = map.GetLength(0);
        int totalRow = map.GetLength(1);
        var adjacents = new List<Point>();
        if (col % 2 == 0)
        {
            if (row > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row + 1));
            if (row > 0 && col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row - 1));
            if (row > 0 && col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row - 1));
            if (col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row));
        }
        else
        {
            if (row > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row + 1));
            if (row < totalRow - 1 && col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row + 1));
            if (row < totalRow - 1 && col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row + 1));
            if (col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row));
        }

        return adjacents;
    }

    public virtual List<Point> GetVerticalHexaAdjacents<T>(T[,] map)
    {
        return GetVerticalHexaAdjacents(map.GetLength(0), map.GetLength(1));
    }

    public List<Point> GetVerticalHexaAdjacents(int totalCol, int totalRow)
    {
        var adjacents = new List<Point>();
        if (col % 2 == 0)
        {
            if (row > 0)
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1)
                adjacents.Add(new Point(col, row + 1));
            if (row > 0 && col > 0)
                adjacents.Add(new Point(col - 1, row - 1));
            if (row > 0 && col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row - 1));
            if (col > 0)
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row));
        }
        else
        {
            if (row > 0)
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1)
                adjacents.Add(new Point(col, row + 1));
            if (row < totalRow - 1 && col > 0)
                adjacents.Add(new Point(col - 1, row + 1));
            if (row < totalRow - 1 && col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row + 1));
            if (col > 0)
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row));
        }

        return adjacents;
    }

    // triangle 
    public virtual List<Point> GetTriangleAdjacents<T>(T[,] map, Func<T, bool> predicate)
    {
        int totalCol = map.GetLength(0);
        int totalRow = map.GetLength(1);
        var adjacents = new List<Point>();
        if (col % 2 == 0)
        {
            if (row > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row + 1));
            if (row > 0 && col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row - 1));
            if (row > 0 && col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row - 1));
            if (col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row));
        }
        else
        {
            if (row > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col, row + 1));
            if (row < totalRow - 1 && col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row + 1));
            if (row < totalRow - 1 && col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row + 1));
            if (col > 0 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1 && predicate(map[col + 1, row]))
                adjacents.Add(new Point(col + 1, row));
        }

        return adjacents;
    }

    public virtual List<Point> GetTriangleAdjacents<T>(T[,] map)
    {
        return GetTriangleAdjacents(map.GetLength(0), map.GetLength(1));
    }

    public List<Point> GetTriangleAdjacents(int totalCol, int totalRow)
    {
        var adjacents = new List<Point>();
        if (col % 2 == 0)
        {
            if (row > 0)
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1)
                adjacents.Add(new Point(col, row + 1));
            if (row > 0 && col > 0)
                adjacents.Add(new Point(col - 1, row - 1));
            if (row > 0 && col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row - 1));
            if (col > 0)
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row));
        }
        else
        {
            if (row > 0)
                adjacents.Add(new Point(col, row - 1));
            if (row < totalRow - 1)
                adjacents.Add(new Point(col, row + 1));
            if (row < totalRow - 1 && col > 0)
                adjacents.Add(new Point(col - 1, row + 1));
            if (row < totalRow - 1 && col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row + 1));
            if (col > 0)
                adjacents.Add(new Point(col - 1, row));
            if (col < totalCol - 1)
                adjacents.Add(new Point(col + 1, row));
        }

        return adjacents;
    }

}
