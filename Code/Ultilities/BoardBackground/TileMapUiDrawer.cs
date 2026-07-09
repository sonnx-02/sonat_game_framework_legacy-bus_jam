using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable ShiftExpressionRealShiftCountIsZero


public class TileMapUiDrawer : BaseFrameworkView
{
    public bool initTile;
    public bool initBoundary;
    public bool reverse;
    [SerializeField] private PoolCollection<GenericImagePoolItem> tiles;
    [SerializeField] private PoolCollection<GenericImagePoolItem> boundaries;

    public Sprite[] tileSprites;
    public Sprite[] boundarySprites;
    public Vector2 tileSize = new Vector2(100,100);
    public RectTransform[] levelContainers;
    protected override void Register()
    {
        base.Register();
        tiles.Init();
        boundaries.Init();
        tiles.ViewFirst(0);
        boundaries.ViewFirst(0);
     
    }

    [SerializeField]
    private RectTransform lastIndicator;
    
    [MyMatrix]
    public ListInt matrix;

    public Subject<Point> clickOn = new Subject<Point>();
    public override void LogCustom()
    {
        base.LogCustom();
        var listIntMap = new ListIntMap(matrix.value).MapBool();
        InitBoard(listIntMap,tileSize);
//        InitBoard();
    }

    private PointSetting _pointSetting;
    public void InitBoard(bool[,] map, Vector2 size)
    {
        Clear(); 
        _pointSetting = new PointSetting()
        {
            distance = size,
            pointType = PointType.Square
        };
        if (initTile)
        {
            map.MyPoints().ForEach(point =>
            {
                var rent = tiles.pools[0].Rent(levelContainers[0],point.GetPos(_pointSetting));
                rent.Image.sprite = point.IsEven() ? tileSprites[0] : tileSprites[1];
                if (rent.Button)
                {
                    rent.Button.onClick.RemoveAllListeners();
                    rent.transform.localEulerAngles = Vector3.zero;
                    rent.Button.onClick.AddListener(() =>
                    {
                        clickOn.OnNext(point);
                    });
                }
            });
            tiles.Container.localPosition = new Vector3((-map.GetLength(0) * 1f / 2 + .5f) * size.x, (-map.GetLength(1) * 1f / 2 +0.5f) * size.y);
        }

        if (initBoundary)
        {
            if (reverse)
                map = map.MyReverse();
            var tileMap = GetMaskMap(map);
            for (int i = 0; i < tileMap.GetLength(0); i++)
                for (int j = 0; j < tileMap.GetLength(1); j++)
                {
                    if (tileMap[i,j] != 0)
                    {
                        var point = new Point(i, j);
                        var rent = boundaries.pools[0].Rent(boundaries.Container,point.GetPos(_pointSetting));
                        rent.transform.localEulerAngles = Vector3.zero;
                        rent.Image.sprite = boundarySprites[tileMap[i, j]];
                    }
                }
            boundaries.Container.localPosition = new Vector3(-map.GetLength(0) * 1f / 2 * size.x, -map.GetLength(1) * 1f / 2 * size.y);
        }
    }

    public void SetPos(Transform tf,Point p,int level)
    {
       tf.SetParent(levelContainers[level]);
       tf.localPosition = p.GetPos(_pointSetting);
       if (lastIndicator != null)
       {
           lastIndicator.SetParent(tiles.Container);
           lastIndicator.localPosition =  p.GetPos(_pointSetting);
       }
    }

    public void InitBoard(bool[,] map)
    {
        InitBoard(map, tileSize);
    }

    private static int[,] GetMaskMap(bool[,] fillMap)
    {
        int cols = fillMap.GetLength(0);
        int rows = fillMap.GetLength(1);
        int[,] map = new int[cols + 1, rows + 1];

        for (int i = 0; i < cols + 1; i++)
            for (int j = 0; j < rows + 1; j++)
            {
                bool topL = i > 0 && j < rows && fillMap[i - 1, j];
                bool topR = i < cols && j < rows && fillMap[i, j];
                bool bottomL = i > 0 && j > 0 && fillMap[i - 1, j - 1];
                bool bottomR = i < cols && j > 0 && fillMap[i, j - 1];

                map[i, j] =
                    Convert.ToInt32(bottomL) << 0 |
                    Convert.ToInt32(bottomR) << 1 |
                    Convert.ToInt32(topR) << 2 |
                    Convert.ToInt32(topL) << 3;
            }
        return map;
    }


    public void Clear()
    {
        tiles.ReturnAll();
        boundaries.ReturnAll();
    }
}
