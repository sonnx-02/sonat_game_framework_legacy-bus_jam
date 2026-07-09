using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class EditorMap3D : BaseFrameworkView
{
    [FormerlySerializedAs("tiles")] [SerializeField] private PoolCollection<EditorSlotView> slots;
    [SerializeField] private PoolCollection<PoolItemPoint> boundaries;
    [SerializeField] private ListPointPoint currentBoard;
    [SerializeField] private Vector2Int mapSizeUnit = new Vector2Int(10, 10);
    [SerializeField] private Vector3 adjust;
    [SerializeField] private PointReactiveProperty currentPoint;
    [SerializeField] private BoxCollider boxCollider;

    protected override void Register()
    {
        base.Register();
        slots.Init();
        boundaries.Init();
        Init(new bool[mapSizeUnit.x*2+1, mapSizeUnit.y*2+1]);
        if (input)
        {
            1f.Timer(() =>
            {
                Observable.EveryUpdate().Subscribe(data => { OnInputUpdate_Select(); });
                currentPoint.Subscribe(data =>
                {
                
//                    Debug.Log(data != null ? data.toString() : "null");
//                    if(data != null)
//                        OnPoint(data);
                });
            });
        }
    }

    public enum EditType
    {
        Point,
        Col,
        Row,
        ColNext,
        ColPre,
        RowNext,
        RowPre,
        Cross
    }

    [SerializeField] private EditType editType;

    private void OnPoint(Point p)
    {
        if (_lastmap != null && !_lastmap.OutOfRange(p))
        {
            switch (editType)
            {
                case EditType.Point:
                    _lastmap.SetAtPoint(p,true);
                    break;
                case EditType.Col:
                    _lastmap.SetAtCol(p,true);
                    break;
                case EditType.Row:
                    _lastmap.SetAtRow(p,true); 
                    break;
                case EditType.ColNext:
                    _lastmap.SetAtColNext(p,true,true); 
                    break;
                case EditType.ColPre:
                    _lastmap.SetAtColNext(p,true,false); 
                    break;
                case EditType.RowNext:
                    _lastmap.SetAtRowNext(p,true,true); 
                    break;
                case EditType.RowPre:
                    _lastmap.SetAtRowNext(p,true,false); 
                    break;
                case EditType.Cross:
                    _lastmap.SetAtCol(p,true);
                    _lastmap.SetAtRow(p,true); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
            
            _boundariesMap = _lastmap.GetMaskMap(true);
            _boundariesMergedMap = merge ? _boundariesMap.GetMergeMap() : null;
            UpdateBoundaryTile(1);
        }
    }

    [SerializeField] private Camera cam;
    [SerializeField] private bool input;
    [SerializeField] private bool inputClear;
    private void OnInputUpdate_Select()
    {
        if (Input.GetMouseButton(0))
        {
            if (initSlot)
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                var human = RayCastHuman(ray);
                var p = human != null ? human.Point : null;
                if (p != currentPoint.Value)
                {
                    currentPoint.Value = p;
                    OnPoint(p);
                }
            }
            else if (boxCollider != null)
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                
                bool hitPlane = (boxCollider.Raycast(ray, out RaycastHit hit, 1000));
                if (hitPlane)
                {
                    var posHit = new Vector2(hit.point.x ,hit.point.z) + _centeredPos;
                    currentPoint.Value = new Point(Mathf.FloorToInt(posHit.x/_distance),Mathf.FloorToInt(posHit.y/_distance));
                    OnPoint(currentPoint.Value);
                }
            }
        }
    }
    
   
    
    private EditorSlotView RayCastHuman(Ray ray)
    {
        return DuongExtensions.RayCast3DNonAllocMin<EditorSlotView>(ray, ~0, ValidToPickup, 10);
        bool ValidToPickup(EditorSlotView human) => true;
    }



    [MyButton(nameof(Clear2),nameof(Test))] [SerializeField]
    private int testInitMap;

    public void Test()
    {
        var array = new bool[mapSizeUnit.x * 2 + 1, mapSizeUnit.y * 2 + 1];
        Init(array);
        foreach (var pointPointParameter in currentBoard.points)
            array.SetAtPoint(pointPointParameter, true);
    }
    
    public void Clear2()
    {
        _boundariesMap = null;
        _boundariesMergedMap = null;
    }

    public Vector2Int fillTo;
    public bool upSize;
    public bool merge;
    public bool initSlot = true;
    private bool[,] _lastmap;
    public void Init(bool[,] map, float distance = 1)
    {
        if (!upSize)
            DoInit(map, distance);
        else
            Init(map, fillTo, distance);
        
        currentBoard.points.Clear();
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
    }


    public void Init(bool[,] map, Vector2Int upToSize, float distance = 1)
    {
        Clear();
       
        if (map.GetLength(0) >= upToSize.x)
            upToSize.x = map.GetLength(0) + 6;
        if (map.GetLength(1) >= upToSize.y)
            upToSize.y = map.GetLength(1) + 6;
        bool[,] expandMap = new bool[upToSize.x, upToSize.y];
        var fromX = (expandMap.GetLength(0) - map.GetLength(0)) / 2;
        var fromY = (expandMap.GetLength(1) - map.GetLength(1)) / 2;
        fromY = 0;
        for (int i = 0; i < map.GetLength(0); i++)
        for (int j = 0; j < map.GetLength(1); j++)
            expandMap[i + fromX, j + fromY] = map[i, j];

        DoInit(map, distance);
        
        currentBoard.points.Clear();
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
    }


    private int[,] _boundariesMap;
    private Vector3Int[,] _boundariesMergedMap;

    public void DoInit(bool[,] map, float distance = 1)
    {
       
        var thisMap = map.GetMaskMap(true);
        var thisMap2 = merge ? thisMap.GetMergeMap() : null;
      

        if (_boundariesMap == null 
            ||_boundariesMap.GetLength(0) != thisMap.GetLength(0)
            || _boundariesMap.GetLength(1) != thisMap.GetLength(1) 
//            || merge
            )
        {
            _boundariesMap = thisMap;
            _boundariesMergedMap = thisMap2;
            InitNewBoundaryTile(distance);
        }
        else
        {
            _boundariesMap = thisMap;
            _boundariesMergedMap = thisMap2;
            UpdateBoundaryTile(distance);
        }
        
        if(!initSlot)
            slots.ReturnAll();

        if (initSlot && (_lastmap == null || _lastmap.GetLength(0) != map.GetLength(0)
                                          || _lastmap.GetLength(1) != map.GetLength(1) ))
        {
            _lastmap = map;
            InitSlot(distance);
        }
        else
        {
            _lastmap = map;
        }
        boxCollider.size = new Vector3(map.GetLength(0), boxCollider.size.y,map.GetLength(1));
    
        boundaries.Container.localPosition = PointGetPosition.GetBoardCenteredPosition(PointType.Square, map, distance, distance).ToXz() + adjust +  Adjust;
        _centeredPos = -boundaries.Container.localPosition.GetXz() + new Vector2(distance/2,distance/2);
        _distance = distance;
    }

    private Vector2 _centeredPos;
    private float _distance;

    private void InitSlot(float distance)
    {
        slots.ReturnAll();
        for (int i = 0; i < _lastmap.GetLength(0); i++)
        for (int j = 0; j < _lastmap.GetLength(1); j++)
        {
            var point = new Point(i, j);
            var tile = slots.pools[0].Rent(slots.Container,
                point.GetPosXz(PointType.Square, distance));
            tile.transform.localScale = Vector3.one;
            tile.name = $"slot {i}:{j}";
            tile.SetPointOnly(point);
        }
    }

    private void InitNewBoundaryTile(float distance)
    {
        var adjustForBoundary =- new Vector3(distance / 2, distance / 2).ToXz();
        Clear();
        currentBoard.points.Clear();
        if (_boundariesMergedMap == null)
        {
            for (int i = 0; i < _boundariesMap.GetLength(0); i++)
            for (int j = 0; j < _boundariesMap.GetLength(1); j++)
            {
                if (_boundariesMap[i, j] != 0)
                {
                    var point = new Point(i, j);
                    var tile = boundaries.pools[_boundariesMap[i, j]].Rent(boundaries.Container,
                        point.GetPosXz(PointType.Square, distance)+adjustForBoundary);
                    tile.transform.localScale = Vector3.one;
                    tile.name = $"point {i}:{j} : {_boundariesMap[i, j]}";
                    tile.setValue.x = _boundariesMap[i, j];
                    tile.SetPointOnly(point);
                }
            }
        }
        else
        {
            // merge
            _boundariesMergedMap = _boundariesMap.GetMergeMap();
            for (int i = 0; i < _boundariesMergedMap.GetLength(0); i++)
            for (int j = 0; j < _boundariesMergedMap.GetLength(1); j++)
            {
                if (_boundariesMergedMap[i, j].x != 0)
                {
                    var pos1 = new Point(i, j).GetPosXz(PointType.Square, distance);
                    if (!(_boundariesMergedMap[i, j].y == 1 && _boundariesMergedMap[i, j].z == 1))
                    {
                        var pos2 = new Point(i + _boundariesMergedMap[i, j].y - 1,  j + _boundariesMergedMap[i, j].z - 1) .GetPosXz(PointType.Square, distance);
                        pos1 = new List<Vector3> {pos1, pos2}.CenterOfVectors();
                    }

                    var tile = boundaries.pools[_boundariesMap[i, j]].Rent(boundaries.Container, pos1+adjustForBoundary);
                    tile.transform.localScale =
                        new Vector3(_boundariesMergedMap[i, j].y, 1, _boundariesMergedMap[i, j].z);
                    tile.name = $"point {i}:{j} : {_boundariesMap[i, j]}";
                    tile.setValue = new Vector3Int(_boundariesMap[i, j],_boundariesMergedMap[i, j].y,_boundariesMergedMap[i, j].z);
                    tile.SetPointOnly(new Point(i, j));
                }
            }
        }
    }

    private void UpdateBoundaryTile(float distance)
    {
        var adjustForBoundary =- new Vector3(distance / 2, distance / 2).ToXz();

        currentBoard.points.Clear();
        var currentTiles = boundaries.GetActives().ToList();
        if (_boundariesMergedMap == null)
        {
            for (int i = 0; i < _boundariesMap.GetLength(0); i++)
            for (int j = 0; j < _boundariesMap.GetLength(1); j++)
            {
                if (_boundariesMap[i, j] != 0)
                {
                    var point = new Point(i, j);
                    var find = currentTiles.Find(x => x.Point == point);
                    if (find != null && find.setValue.x == _boundariesMap[i, j])
                    {
                        currentTiles.Remove(find);
                        continue;
                    }
                    
                    var tile = boundaries.pools[_boundariesMap[i, j]].Rent(boundaries.Container,
                        point.GetPosXz(PointType.Square, distance)+adjustForBoundary);
                    tile.transform.localScale = Vector3.one;
                    tile.name = $"point {i}:{j} : {_boundariesMap[i, j]}";
                    tile.setValue.x = _boundariesMap[i, j];
                    tile.SetPointOnly(point);
                }
            }
        }
        else
        {
            for (int i = 0; i < _boundariesMergedMap.GetLength(0); i++)
            for (int j = 0; j < _boundariesMergedMap.GetLength(1); j++)
                if (_boundariesMergedMap[i, j].x != 0)
                {
                    var point = new Point(i, j);
                    var find = currentTiles.Find(x => x.Point == point);
                    if (find != null && find.setValue.x == _boundariesMap[i, j]
                                     && find.setValue.y == _boundariesMergedMap[i, j].y
                                     && find.setValue.z == _boundariesMergedMap[i, j].z
                                     )
                    {
                        currentTiles.Remove(find);
                        continue;
                    }

                    var pos1 = new Point(i, j).GetPosXz(PointType.Square, distance);
                    if (!(_boundariesMergedMap[i, j].y == 1 && _boundariesMergedMap[i, j].z == 1))
                    {
                        var pos2 = new Point(i + _boundariesMergedMap[i, j].y - 1, j + _boundariesMergedMap[i, j].z - 1)
                            .GetPosXz(PointType.Square, distance);
                        pos1 = new List<Vector3> {pos1, pos2}.CenterOfVectors();
                    }

                    pos1 += adjustForBoundary;

                    var tile = boundaries.pools[_boundariesMap[i, j]].Rent(boundaries.Container, pos1);
                    tile.transform.localScale =
                        new Vector3(_boundariesMergedMap[i, j].y, 1, _boundariesMergedMap[i, j].z);
                    tile.name = $"point {i}:{j} : {_boundariesMap[i, j]}";
                    tile.setValue = new Vector3Int(_boundariesMap[i, j], _boundariesMergedMap[i, j].y,
                        _boundariesMergedMap[i, j].z);
                    tile.SetPointOnly(new Point(i, j));
                } 
        }
        foreach (var poolItemPoint in currentTiles)
            poolItemPoint.Return();
    }
    
    public void Clear()
    {
        boundaries.ReturnAll();
    }

    private Vector3 Adjust { get; set; }

    public void SetAdjust(Vector3 cached)
    {
        Adjust = cached;
    }
}