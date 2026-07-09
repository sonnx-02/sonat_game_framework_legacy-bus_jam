using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundBoard3D : BaseFrameworkView
{
    public Transform container;
    public PoolCollection<PoolItem> tiles;
//    [ListPointPointName(
//        new int[] { },
//        //new int []{10}, 
//        new[]
//        {
//            nameof(CustomParameter.intValue), nameof(CustomParameter.intValue2), nameof(CustomParameter.intValue3),
//            nameof(CustomParameter.boolValue)
//        }
//        , new[] {"Index", "Type", ""}
//        , new[] {40, 40, 30}, ListPointType.Absolute
//        , 7, 6
//        , new string[] {null, null}
//    )]
//    public ListPointPoint testBoard;
//    
    [ListPointPointName(
        new int[] { },
        //new int []{10}, 
        new[]
        {
            nameof(CustomParameter.intValue), nameof(CustomParameter.intValue2), nameof(CustomParameter.intValue3),
            nameof(CustomParameter.boolValue)
        }
        , new[] {"Index", "Type", ""}
        , new[] {40, 40, 30}, ListPointType.Absolute
        , 11, 12
        , new string[] {null, null}
    )]
    public ListPointPoint currentBoard;
    
    


    [MyButton(nameof(Test))] [SerializeField]
    private int test123;

    protected override void Register()
    {
        base.Register();
        tiles.Init();
        tiles.OverrideContainer(container);
    }

    public void Test()
    {
        var array = new bool[7, 6];
        foreach (var pointPointParameter in currentBoard.points)
            array.SetAtPoint(pointPointParameter,true);
        
        Init(array);
    }

    public Vector2Int fillTo;
    public bool upSize;
    public bool merge;
    public void Init(bool[,] map,float distance = 1)
    {
        if (!upSize)
        {
            Clear();
            currentBoard.points.Clear();
            var tileMap = map.GetMaskMap(true);
            for (int i = 0; i < tileMap.GetLength(0); i++)
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j] != 0)
                {
                    var point = new Point(i, j);
                    var tile = tiles.pools[tileMap[i, j]].Rent(container, point.GetPosXz(PointType.Square, distance));
                    tile.name = $"point {i}:{j} : {tileMap[i, j]}";
                }
            }

            for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j])
                    currentBoard.points.Add(new PointPointParameter()
                    {
                        col = i,
                        row = j,
                    });
                container.localPosition = new Vector3(-map.GetLength(0) * 1f / 2 * distance, 0,
                                              -map.GetLength(1) / 2f * distance) + adjust;
            }
        }
        else Init(map,fillTo,distance);
    }

    public void Init(bool[,] map, Vector2Int upToSize, float distance = 1)
    {
        Clear();
        currentBoard.points.Clear();
        if (map.GetLength(0) >= upToSize.x)
            upToSize.x = map.GetLength(0) + 6;
        if (map.GetLength(1) >= upToSize.y)
            upToSize.y = map.GetLength(1) + 6;
        bool[,] expandMap = new bool[upToSize.x,upToSize.y];
        var fromX = (expandMap.GetLength(0) - map.GetLength(0))/2;
        var fromY = (expandMap.GetLength(1) - map.GetLength(1)) / 2;
        fromY = 0;
        for (int i = 0; i < map.GetLength(0); i++)
        for (int j = 0; j < map.GetLength(1); j++)
        {
            expandMap[i + fromX, j + fromY] = map[i, j];
        }
       
        var tileMap = expandMap.GetMaskMap(true);

        if (!merge)
        {
            for (int i = 0; i < tileMap.GetLength(0); i++)
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j] != 0)
                {
                    var point = new Point(i, j);
                    var tile = tiles.pools[tileMap[i, j]].Rent(container, point.GetPosXz(PointType.Square, distance));
                    tile.transform.localScale = Vector3.one;
                    tile.name = $"point {i}:{j} : {tileMap[i, j]}";
                }
            } 
        }
        else
        {
            // merge
            var mergeMap = tileMap.GetMergeMap();
            for (int i = 0; i < mergeMap.GetLength(0); i++)
            for (int j = 0; j < mergeMap.GetLength(1); j++)
            {
                if (mergeMap[i, j].x != 0)
                {
                    var pos1 = new Point(i, j).GetPosXz(PointType.Square, distance);
                    if (!(mergeMap[i, j].y == 1 && mergeMap[i, j].z == 1))
                    {
                        var pos2 = new Point(i + mergeMap[i, j].y -1, j+ mergeMap[i, j].z -1)
                            .GetPosXz(PointType.Square, distance);
                        pos1 = new List<Vector3> {pos1, pos2}.CenterOfVectors();
                    }
                    var tile = tiles.pools[tileMap[i, j]].Rent(container, pos1);
                    tile.transform.localScale = new Vector3(mergeMap[i, j].y,1,mergeMap[i, j].z);
                    tile.name = $"point {i}:{j} : {tileMap[i, j]}";
                }
            } 
        }
        
        for (int i = 0; i < map.GetLength(0); i++)
        for (int j = 0; j < map.GetLength(1); j++)
        {
            if (map[i, j])
                currentBoard.points.Add(new PointPointParameter()
                {
                    col = i,
                    row = j,
                });
        }
        container.localPosition = new Vector3((-expandMap.GetLength(0) * 1f  ) / 2 * distance + (map.GetLength(0) % 2 == 0 ? 0.5f : 0) * distance, 
                                      0,-map.GetLength(1) / 2f * distance) + adjust + Adjust;
//        container.localPosition = new Vector3((-expandMap.GetLength(0) * (1f + (map.GetLength(0) % 2 == 0 ? 0.5f : 0)) ) / 2 * distance, 0,
//                                      -map.GetLength(1) / 2f * distance) + adjust + Adjust;
    }

    [SerializeField] private Vector3 adjust;

    public void Clear()
    {
        tiles.ReturnAll();
    }

    private Vector3 Adjust { get; set; }
    public void SetAdjust(Vector3 cached)
    {
        Adjust = cached;
    }
}