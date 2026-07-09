using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BackgroundBoard2D : MonoBehaviour
{
    public bool InitTile;
    [ConditionalField("InitTile")]
    public Transform TileParent;
    [ConditionalField("InitTile")]
    public BackgroundTile2D TilePrefab;
    [ConditionalField("InitTile")]
    public Sprite OddTileSprite;
    [ConditionalField("InitTile")]
    public Sprite EvenTileSprite;
    public List<BackgroundTile2D> Tiles;

    public bool InitBoundaryTile;
    [ConditionalField("InitBoundaryTile")]
    public Transform BoundaryTileParent;
    [ConditionalField("InitBoundaryTile")]
    public BackgroundTile2D BoundaryTilePrefab;
    public Sprite[] BoundaryTileSprites;
    public List<BackgroundTile2D> BoundaryTiles;

    void Start()
    {
        Load();
    }

    public void InitBoard(bool[,] map, Vector2 size)
    {
        Clear();
        if (InitTile)
        {
            if (TilePrefab != null)
                map.MyPoints().ForEach(x => GetATile(x));
            TileParent.transform.localPosition = new Vector3((-map.GetLength(0) * 1f / 2 + .5f) * size.x, (-map.GetLength(1) * 1f / 2 + .5f) * size.y);
        }

        if (InitBoundaryTile)
        {
            var tileMap = GetMaskMap(map);
            for (int i = 0; i < tileMap.GetLength(0); i++)
                for (int j = 0; j < tileMap.GetLength(1); j++)
                {
                    var point = new Point(i, j);
                    var tile = GetABgTile(point);
                    tile.spriteRenderer.sprite = BoundaryTileSprites[tileMap[i, j]];
                }
            BoundaryTileParent.transform.localPosition = new Vector3(-map.GetLength(0) * 1f / 2 * size.x, -map.GetLength(1) * 1f / 2 * size.y);
        }
    }

    public void InitBoard(bool[,] map)
    {
        InitBoard(map, Vector2.one);
    }

    BackgroundTile2D GetATile(Point point)
    {
        var tile = Tiles.Find(x => !x.gameObject.activeSelf);
        if (tile != null)
            tile.gameObject.SetActive(true);
        else
        {
            tile = Instantiate(TilePrefab);
            Tiles.Add(tile);
            tile.transform.SetParent(TileParent);
        }

        tile.gameObject.name = "Tile " + point.col + "/" + point.row;
        tile.transform.localPosition = point.GetPos();
        if (EvenTileSprite != null)
            tile.spriteRenderer.sprite = point.IsEven() ? OddTileSprite : EvenTileSprite;
        return tile;
    }

    BackgroundTile2D GetABgTile(Point point)
    {
        var tile = BoundaryTiles.Find(x => !x.gameObject.activeSelf);
        if (tile != null)
            tile.gameObject.SetActive(true);
        else
        {
            tile = Instantiate(BoundaryTilePrefab);
            tile.gameObject.SetActive(true);
            BoundaryTiles.Add(tile);
            tile.transform.SetParent(BoundaryTileParent);
        }

        tile.gameObject.name = "Tile " + point.col + "/" + point.row;
        tile.transform.localPosition = point.GetPos();
        return tile;
    }

    public static int[,] GetMaskMap(bool[,] fillMap)
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

    public void Test()
    {
        var map = new bool[10, 10];
        map.Populate(true);
        InitBoard(map, Vector2.one);
    }

    public void Test2()
    {
        var map = new bool[10, 10];
        map[4, 4] = true;
        map[4, 5] = true;
        map[5, 4] = true;
        map[5, 5] = true;
        InitBoard(map, Vector2.one);
    }

    public void Clear()
    {
        BoundaryTiles.ForEach(x =>
        {
            if (x != null)
                x.gameObject.SetActive(false);
        });

        Tiles.ForEach(x =>
        {
            if (x != null)
                x.gameObject.SetActive(false);
        });
    }


    public void Delete()
    {
        BoundaryTiles.ForEach(x =>
        {
            if (x != null)
                DestroyImmediate(x.gameObject);
        });
        BoundaryTiles.Clear();
    }

    public void Load()
    {
        if (InitBoundaryTile)
        {
            BoundaryTiles = BoundaryTileParent.GetComponentsInChildren<BackgroundTile2D>().ToList();
            BoundaryTiles.ForEach(x => x.gameObject.SetActive(false));
            Clear();
        }
    }
}
