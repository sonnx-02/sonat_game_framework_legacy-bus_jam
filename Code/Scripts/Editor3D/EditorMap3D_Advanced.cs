using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class EditorMap3D_Advanced : BaseFrameworkView
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform container;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private PoolCollection<EditorSlotView> inputSlots;
    [SerializeField] private PoolCollection<PoolItemPoint> boundaries;
    [SerializeField] private PoolCollection<PoolItemPoint> fillSlots;
    [SerializeField] private Vector3 adjustPos;
    [SerializeField] private PointDirection inputDirection;
    [SerializeField] private int length;
    [SerializeField] private PointReactiveProperty currentPoint;
    [SerializeField] private bool createSlot = true;
    [SerializeField] private bool createSlotOnStart;
    [SerializeField] private bool initFillSlot;
    [SerializeField] private PointDirection alignZeroAs = PointDirection.Bottom;

    private IDisposable _inputDisposable;
    public LogicMap3D logicMap3D;
    
    [SerializeField] private float addZDistance;

    /// <summary>
    /// Init the Logic Map - Logic Map Change into Map Update UI
    /// </summary>
    protected override void Register()
    {
        base.Register();
        logicMap3D.SetDistance(1);
        inputSlots.Init();
        boundaries.Init();
        fillSlots.Init();
        logicMap3D.onMapSizeChanged += OnMapSizeUpdated;
        logicMap3D.onMapChanged += OnMapUpdated;

        if (createSlotOnStart)
        {
            CreateNew();
            1f.Timer(() => StartInput());
        }
    }

    public void Clear()
    {
        inputSlots.ReturnAll();
        boundaries.ReturnAll();
        fillSlots.ReturnAll();
    }
    public float GetAdjustZ() => addZDistance * logicMap3D.Distance;
    
    public void StartInput(bool stop = false)
    {
        _inputDisposable?.Dispose();
        if (!stop)
            _inputDisposable = Observable.EveryUpdate().Subscribe(data => { OnInputUpdate_Select(); });
    }

    /// <summary>
    /// Clear Input slot - Reinit tiles, box collider of map, boundaries
    /// </summary>
    private void OnMapSizeUpdated()
    {
        var map = logicMap3D.GetMap();
        if (!createSlot)
            inputSlots.ReturnAll();
        if (createSlot)
        {
            inputSlots.ReturnAll();
            for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
            {
                var point = new Point(i, j);
                var tile = inputSlots.pools[0].Rent(inputSlots.Container,
                    point.GetPosXz(PointType.Square, logicMap3D.Distance));
                tile.transform.localScale = Vector3.one;
                tile.name = $"slot {i}:{j}";
                tile.SetPointOnly(point);
            }
        }

        var col = map.GetLength(0);
        var row = map.GetLength(1);
        boxCollider.size = new Vector3(col, boxCollider.size.y, row);
        boundaries.Container.localPosition = logicMap3D.CenteredPos + adjustPos + Adjust;
        if (alignZeroAs == PointDirection.Bottom)
            boundaries.Container.SetLocalZ(0 + addZDistance * logicMap3D.Distance);
        if (_originalSize.x % 2 == 1)
            boundaries.Container.AddLocalX(-logicMap3D.Distance / 2f);
        if (alignZeroAs != PointDirection.Bottom)
            if (_originalSize.y % 2 == 1)
                boundaries.Container.AddLocalZ(-logicMap3D.Distance / 2f);
    }
    
    public void OnMapUpdated()
    {
        var boundariesMergedMap = logicMap3D.MergeBoundaryMap;
        var currentTiles = boundaries.GetActives().ToList();

        for (int i = 0; i < boundariesMergedMap.GetLength(0); i++)
        for (int j = 0; j < boundariesMergedMap.GetLength(1); j++)
            if (boundariesMergedMap[i, j].x != 0)
            {
                var point = new Point(i, j);
                var find = currentTiles.Find(x => x.Point == point);
                if (find != null && find.setValue.x == boundariesMergedMap[i, j].x
                                 && find.setValue.y == boundariesMergedMap[i, j].y
                                 && find.setValue.z == boundariesMergedMap[i, j].z
                )
                {
                    currentTiles.Remove(find);
                    continue;
                }

                var pos1 = new Point(i, j).GetPosXz(PointType.Square, logicMap3D.Distance);
                if (!(boundariesMergedMap[i, j].y == 1 && boundariesMergedMap[i, j].z == 1))
                {
                    var pos2 = new Point(i + boundariesMergedMap[i, j].y - 1, j + boundariesMergedMap[i, j].z - 1)
                        .GetPosXz(PointType.Square, logicMap3D.Distance);
                    pos1 = new List<Vector3> {pos1, pos2}.CenterOfVectors();
                }


                var tile = boundaries.pools[boundariesMergedMap[i, j].x].Rent(boundaries.Container, pos1);
                tile.transform.localScale =
                    new Vector3(boundariesMergedMap[i, j].y, 1, boundariesMergedMap[i, j].z);
                tile.name = $"point {i}:{j} : {boundariesMergedMap[i, j].x}";
                tile.setValue = new Vector3Int(boundariesMergedMap[i, j].x, boundariesMergedMap[i, j].y,
                    boundariesMergedMap[i, j].z);
                tile.SetPointOnly(new Point(i, j));
            }

        if (initFillSlot)
        {
            fillSlots.ReturnAll();
            foreach (var pointParameterBase in logicMap3D.MapData)
              fillSlots.pools[0].Rent(boundaries.Container, pointParameterBase.GetPosXz(PointType.Square, logicMap3D.Distance,0.5f));
        }
     

        currentTiles.ReturnAll();
    }

    public void CreateNew()
    {
        boundaries.ReturnAll();
        fillSlots.ReturnAll();
        SetData(logicMap3D.size, new List<PointParameterBase>());
        Init();
#if UNITY_EDITOR
        if (logicMap3D.Distance <= 0.1f)
        {
            Debug.LogError("distance is zero");
        }
#endif
    }


    public void Init()
    {
        logicMap3D.RecalculateMapPosition();
        logicMap3D.RecalculateMap();
    }


    private Vector2Int _originalSize;
    private Vector2Int _upSize;
    public Vector2Int OriginalSize => _originalSize;

    public IEnumerable<PointParameterBase> TrueMapData()
    {
        foreach (var pointParameterBase in logicMap3D.mapData)
        {
            var point = GetOriginalPoint(pointParameterBase, _originalSize);
            var clone = pointParameterBase.CloneCurrentBase();
            clone.col = point.col;
            clone.row = point.row;
            yield return clone;
        }
    }

    public void PrintOriginalPoint(Point p)
    {
        var originalPoint = p + Point.Create(-(_upSize.x - _originalSize.x) / 2, 0);
        Debug.Log("originalPoint" + originalPoint.toString() + "click:" + p.toString() + "-_originalSize:" +
                  _originalSize + "-_upSize:" + _upSize);
    }

 
    

    public Point GetOriginalPoint(Point p, Vector2Int startArea)
    {
        var originalPoint2 = p + Point.Create(-(_upSize.x - startArea.x) / 2, 0);
        return originalPoint2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="startArea">center area of map</param>
    /// <returns></returns>
    public bool IsInRange(Point p, Vector2Int startArea)
    {
        var originalPoint2 = p + Point.Create(-(_upSize.x - startArea.x) / 2, 0);
        return originalPoint2.IsInRange(startArea);
    }

    public void SetData(bool[,] map, Vector2Int upSize, float distance = 1)
    {
        var convert = Convert(UpSizeToTop(map, upSize), ignoreMergeConvert);
        SetData(convert.Item1, convert.Item2, distance);
        _originalSize = new Vector2Int(map.GetLength(0), map.GetLength(1));
    }

    [SerializeField] private bool ignoreMergeConvert;

    public void SetData(bool[,] map, float distance = 1)
    {
        var convert = Convert(map, ignoreMergeConvert);
        _originalSize = map.GetVectorSize();
        SetData(convert.Item1, convert.Item2, distance);
    }


    public void SetData(Vector2Int size, List<PointParameterBase> mapData, float distance = 1)
    {
        _originalSize = size;
        _upSize = size;
        logicMap3D.size = size;
        logicMap3D.mapData = mapData;
        logicMap3D.SetDistance(distance);
    }

    public List<PointParameterBase> ConvertPointToFromCenter(Vector2Int size, List<PointParameterBase> mapData)
    {
        for (var i = 0; i < mapData.Count; i++)
        {
            mapData[i].col = mapData[i].col + size.x / 2;
            mapData[i].row = mapData[i].row + size.y / 2;
        }

        return mapData;
    }


    public static (Vector2Int, List<PointParameterBase>) Convert(bool[,] map, bool ignoreMerge)
    {
        var size = new Vector2Int(map.GetLength(0), map.GetLength(1));
        var list = new List<PointParameterBase>();

        for (int i = 0; i < map.GetLength(0); i++)
        for (int j = 0; j < map.GetLength(1); j++)
        {
            if (map[i, j])
            {
                var point = new PointParameterBase(i, j, 0);
                if (!ignoreMerge)
                    for (int k = j + 1; k < map.GetLength(1); k++)
                    {
                        if (map[i, k])
                        {
                            point.value = (int) PointDirection.Top;
                            point.value2 += 1;
                            j++;
                        }
                        else
                            break;
                    }

                list.Add(point);
            }
        }

        return (size, list);
    }

    public static bool[,] UpSizeToTop(bool[,] map, Vector2Int upToSize)
    {
        if (map.GetLength(0) > upToSize.x)
            upToSize.x = map.GetLength(0) + 6;
        if (map.GetLength(1) > upToSize.y)
            upToSize.y = map.GetLength(1) + 6;
        bool[,] expandMap = new bool[upToSize.x, upToSize.y];
        var fromX = (expandMap.GetLength(0) - map.GetLength(0)) / 2;
        var fromY = 0;
        // var fromY = (expandMap.GetLength(1) - map.GetLength(1)) / 2;
        for (int i = 0; i < map.GetLength(0); i++)
        for (int j = 0; j < map.GetLength(1); j++)
            expandMap[i + fromX, j + fromY] = map[i, j];

        return expandMap;
    }


    private void OnPoint(Point p)
    {
        AddWall(p);
    }

    private void OnPointUp(Point p)
    {
    }

    public void AddWall(Point p)
    {
        if (logicMap3D != null && !logicMap3D.size.OutOfRange(p))
            logicMap3D.AddDistinct(new PointParameterBase(p.col, p.row, (int) inputDirection, length));
    }

    public void AddLine(Point p, int direction, int lengthValue = 0)
    {
        if (logicMap3D != null && !logicMap3D.size.OutOfRange(p))
            logicMap3D.AddDistinct(new PointParameterBase(p.col, p.row, direction, lengthValue));
    }


    public bool HasWall(Point p)
    {
        if (logicMap3D != null && !logicMap3D.size.OutOfRange(p))
            return logicMap3D.Exist(new PointParameterBase(p.col, p.row, (int) inputDirection, length));
        return false;
    }

    public void RemoveWall(Point p)
    {
        logicMap3D.Remove(p);
    }

    public Action<Point> onPointDown;
    public Action<Point> onPoint;
    public Action<Point> onPointUp;
    public Action<Vector3> onDrag;
    public bool StopInput { get; set; }
    public bool StopCreate { get; set; }
    [SerializeField] private bool log;

    private Vector3 mouseDownPos;
    private Vector3 lastChange;

    private void OnInputUpdate_Select()
    {
        if (StopInput)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            var p = GetMousePoint();
            if (p != null)
            {
                Debug.Log(p.toString());
                onPointDown?.Invoke(p);
            }
            mouseDownPos = GetPos();
            if (snap > 0)
                mouseDownPos = mouseDownPos.Snap(snap, logicMap3D.Distance);
            lastChange = mouseDownPos;
        }


        if (Input.GetMouseButton(0))
        {
            var p = GetMousePoint();
            if (p != currentPoint.Value && p != null)
            {
                currentPoint.Value = p;
                if (!StopCreate)
                    OnPoint(p);
                onPoint?.Invoke(p);
            }

            var pos = GetPos();
            if (snap > 0)
                pos = pos.Snap(snap, logicMap3D.Distance);
            if (Vector3.Distance(pos, lastChange) > 0.05f)
            {
                lastChange = pos;
                onDrag?.Invoke(lastChange - mouseDownPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            var p = GetMousePoint();
            if (p != null)
            {
                currentPoint.Value = p;
                if (!StopCreate)
                    OnPointUp(p);
                onPointUp?.Invoke(p);
            }
            else
            {
                onPointUp?.Invoke(null);
            }

            currentPoint.Value = Point.MinusOne;
        }
    }

    [SerializeField] private int snap;

    public Vector3 SnapPos(Vector3 vec) => vec.Snap(Snap, logicMap3D.Distance);
    public int Snap => snap;

    private Point GetMousePoint()
    {
        if (createSlot)
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            var human = RaycastSlot(ray);
            var p = human != null ? human.Point : null;
            if (p != currentPoint.Value)
            {
                currentPoint.Value = p;
            }
        }
        else if (boxCollider != null)
        {
            boxCollider.enabled = true;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            bool hitPlane = (boxCollider.Raycast(ray, out RaycastHit hit, 1000));
            if (hitPlane)
            {
                Vector2 mouseHitPosition = new Vector2(hit.point.x, hit.point.z);
                Vector2 bottomLeft = boundaries.Container.localPosition.GetXz();
                return PositionToPointFromBottomLeft(mouseHitPosition, bottomLeft, logicMap3D.Distance);
            }
        }

        return null;
    }

    private static Point PositionToPointFromBottomLeft(Vector2 input, Vector2 bottomLeft, float distance)
    {
        var posHit = input - bottomLeft;
        return new Point(Mathf.FloorToInt(posHit.x / distance), Mathf.FloorToInt(posHit.y / distance));
    }

    public Point GetPoint(Vector2 position, int divide = 1)
    {
        Vector2 bottomLeft = boundaries.Container.localPosition.GetXz();
        return PositionToPointFromBottomLeft(position, bottomLeft, logicMap3D.Distance / divide);
    }

    public Point GetPointFromCenter(Vector2 position, int divide = 1)
    {
        position -= new Vector2(logicMap3D.Distance / 2, logicMap3D.Distance / 2);
        return GetPoint(position, divide) - GetCenterPoint(divide);
    }

    private Point GetCenterPoint(int divide = 1)
    {
        return new Point((_upSize.x / 2) * divide, (_upSize.y / 2) * divide);
    }
    
    public PointParameterBase PointFromCenterToMapPoint(PointParameterBase centerPoint)
    {
        var p =  centerPoint.CloneCurrentBase();
        p.col += _upSize.x / 2;
        p.row += _upSize.x / 2;
        return p;
    }

    public Point PointFromCenterToMapPoint(Point centerPoint)
    {
        var p =  centerPoint.Clone();
        p.col += _upSize.x / 2;
        p.row += _upSize.x / 2;
        return p;
    }
    
    public PointParameterBase MapPointToPointFromCenter(PointParameterBase centerPoint)
    {
        var p =  centerPoint.CloneCurrentBase();
        p.col -= _upSize.x / 2;
        p.row -= _upSize.x / 2;
        return p;
    }

    public Point MapPointToPointFromCenter(Point centerPoint)
    {
        var p =  centerPoint.Clone();
        p.col -= _upSize.x / 2;
        p.row -= _upSize.x / 2;
        return p;
    }

    public Point GetPointFromCenter(Point point, int divide = 1)
    {
        return point - GetCenterPoint(divide);
    }

    private Vector3 GetPos()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            bool hitPlane = (boxCollider.Raycast(ray, out RaycastHit hit, 1000));
            if (hitPlane)
            {
                _lastHit = hit.point;
            }
        }

        return _lastHit;
    }

    private Vector3 _lastHit;

    private EditorSlotView RaycastSlot(Ray ray)
    {
        return DuongExtensions.RayCast3DNonAllocMin<EditorSlotView>(ray, ~0, ValidToPickup, 10);
        bool ValidToPickup(EditorSlotView human) => true;
    }

    // init or update slot and boundaries

    private Vector3 Adjust { get; set; }
    public void SetAdjust(Vector3 cached) => Adjust = cached;

    [MyButton(nameof(CreateNew), nameof(OnMapUpdated))] [SerializeField]
    private int testInitMap;

    public float CamSize() => cam.orthographicSize;

   
}