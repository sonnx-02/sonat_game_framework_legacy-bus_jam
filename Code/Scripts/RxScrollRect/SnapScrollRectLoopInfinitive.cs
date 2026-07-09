using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScrollRectLoopInfinitive : InfinitiveScrollRectBase<RxScrollItemGeneric>, IBeginDragHandler,
    IEndDragHandler, IDragHandler
{
    [SerializeField] [MyButtonInt(nameof(Populate), nameof(Pre), nameof(Next))]
    private int testFunc = -1;

    [SerializeField] [MyButtonInt(nameof(SetPosItem), nameof(Recenter))]
    private int testFunc2;

    [SerializeField] private float centered = 5;
    [SerializeField] private int remap = 5;
    [SerializeField] private float startPos;
    [SerializeField] private int adjustFocus = 0;
    [SerializeField] private float autoNext = 0;
    public Transform paginationContainer;
    public ToggleScript paginationPrefab;
    private readonly List<ToggleScript> _paginations = new List<ToggleScript>();
    public IntReactiveProperty current = new IntReactiveProperty();
    public IntReactiveProperty Current => current;
    private RxScrollItemGeneric[] _currentItems;
    public RxScrollItemGeneric[] CurrentItems => _currentItems;

    [SerializeField] private ToggleScript[] hasItem;

    // x = current, y = left or right
    public Subject<bool> onChanged = new Subject<bool>(); // prior left

    private float _centerMinorAdjust;
    private float _startXCentered;

    private int _n;
    public void Populate(int n = -1)
    {
        _n = n;
        var active = n == -1 ? nActive : n * 2;
        
        if(active > activeItems.Length)
            Debug.LogError("not enough room");
        _currentItems = new RxScrollItemGeneric[active];
        for (int i = 0; i < activeItems.Length; i++)
        {
            if (i < active)
            {
                _currentItems[i] = activeItems[i];
                _currentItems[i].gameObject.SetActive(true);
                _currentItems[i].Index = i;
            }
            else
                activeItems[i].gameObject.SetActive(false);
        }

        var totalSize = SetSize(nInit);
        adjustFocus = Mathf.FloorToInt(_currentItems.Length / 2f);
        _lastTempAdjust = int.MinValue;
        startPos = -totalSize / 2;
        centered = nInit / 2f;
        _centerMinorAdjust = nInit % 2 == 0 ? 0.5f : -0.5f;
        _startXCentered = startPos - padding.left - size.x * (centered + _centerMinorAdjust);
        remap = 0;
        if(Application.isPlaying)
            UpdateCenter();
        onChanged.OnNext(true);
        
        Focus(0);
        if (Application.isPlaying)
        {
            _paginiationDisposable?.Dispose();
            paginationContainer.DestroyAllChildren();
            _paginations.Clear();
            
            if (n < 0)
            {
                for (int i = 0; i < _currentItems.Length; i++)
                {
                    var pagination = Instantiate(paginationPrefab, paginationContainer, false);
                    pagination.gameObject.SetActive(true);
                    _paginations.Add(pagination);
                }
                
                _paginiationDisposable = current.Subscribe(data =>
                {
                    if (_currentItems != null && _paginations != null)
                        for (var i = 0; i < _paginations.Count; i++)
                            _paginations[i].OnChanged(data.Repeat(_currentItems.Length - 1) == i);
                });

                GetComponent<ScrollRect>().enabled = true;
            }
            else if (n > 1)
            {
                for (int i = 0; i < n; i++)
                {
                    var pagination = Instantiate(paginationPrefab, paginationContainer, false);
                    pagination.gameObject.SetActive(true);
                    _paginations.Add(pagination);
                }

                onChanged.OnNext(true);
            }

            GetComponent<ScrollRect>().enabled = n != 1;
            hasItem.OnChanged(n != 0);
        }
    }

    public void ChangePagination(int index)
    {
        if (_currentItems != null && _paginations != null)
            for (var i = 0; i < _paginations.Count; i++)
                _paginations[i].OnChanged(index== i);
    }

    private IDisposable _paginiationDisposable;

    private float _t;

    private IDisposable _nextDisposable;

    private void OnEnable()
    {
        if (autoNext > 0.5f)
        {
            _t = Time.time;
            _nextDisposable?.Dispose();
            _nextDisposable = Observable.EveryUpdate()
                .TakeUntilDisable(this).Subscribe(data =>
                {
                    if (Time.time - _t > autoNext)
                    {
                        _t = Time.time;
                        Pre();
                    }
                });
        }
    }

    [MyButton(nameof(UpdateCenter))] public int abcTest;


    private int _lastTempAdjust = -1;

    [SerializeField] private bool sensitive;

//    [SerializeField] private RxScrollItemGeneric previousSlot;
//    [SerializeField] private RxScrollItemGeneric currentSlot;
//    [SerializeField] private RxScrollItemGeneric nextSlot;
    private int[] _order;
    public int[] Order => _order;

    public IList<int> GetCentered()
    {
        return CurrentItems
            .OrderBy(x => Mathf.Abs(x.RectTransform.anchoredPosition.x + content.anchoredPosition.x))
            .Select(x =>x.Index).ToArray();
    }
    
    public IList<int> GetCenteredCenterPreNext()
    {
        var list = GetCentered();
        if (_currentItems.Length > 2 && _currentItems[1].RectTransform.anchoredPosition.x > _currentItems[0].RectTransform.anchoredPosition.x)
        {
            var temp = list[2];
            list[2] = list[1];
            list[1] = temp;
        }

        return list;
    }

    private bool UpdateCenter()
    {
//        #if UNITY_EDITOR
//        if (!Application.isPlaying)
//            return false;
//        #endif
        float calculate =  -(content.anchoredPosition.x - startPos - padding.left - size.x * (centered + _centerMinorAdjust)) / size.x;
        var round = Mathf.RoundToInt(calculate);
        if (sensitive)
        {
            var isSensitive = Mathf.Abs(calculate % 1) > 0.05f && Mathf.Abs(calculate % 1) < 0.50f;
            var forward = isSensitive ? (calculate % 1 < 0 ? -1 : 1) : 0;
            round = forward + Mathf.RoundToInt(calculate);
        }
        if (round != _lastTempAdjust)
        {
            _lastTempAdjust = round;
            List<int> a = new List<int>();
            for (var i = 0; i < _currentItems.Length; i++)
                a.Add((i - adjustFocus - 1 - round).Repeat(_currentItems.Length - 1));
            for (var i = 0; i < a.Count; i++)
                a[i] = (a[i] - remap).Repeat(_currentItems.Length - 1);
            current.Value = remap + round;
            for (var i = 0; i < _currentItems.Length; i++)
                _currentItems[i].RectTransform.SetAnchorX(GetPos(a[i]));
            _order = new int[a.Count];
            for (int i = 0; i < a.Count; i++)
                _order[a[i]] = i;
            
            
            return true;
            float GetPos(int i)
            {
                return startPos + padding.left +
                       (i + 0.5f + centered - adjustFocus + round) * size.x;
            }
        }

        return false;
    }

    private void Recenter()
    {
        float calculate =  -(content.anchoredPosition.x - startPos - padding.left - size.x * (centered + _centerMinorAdjust)) / size.x;
        var round = Mathf.RoundToInt(calculate);
        var pos1 = content.anchoredPosition.x + size.x * Mathf.RoundToInt(calculate);
        content.SetAnchorX(pos1);
        calculate = -(pos1 - startPos - padding.left - size.x * (centered + _centerMinorAdjust)) / size.x;
        remap += round;
        remap = remap.Repeat(_currentItems.Length - 1);
        List<int> a = new List<int>();
        for (var i = 0; i < _currentItems.Length; i++)
            a.Add((i - adjustFocus - 1 - remap).Repeat(_currentItems.Length - 1));
        for (var i = 0; i < _currentItems.Length; i++)
            _currentItems[i].RectTransform.SetAnchorX(GetPos(a[i]));
        //   Debug.LogError(string.Join(',', a.Select(x => x.ToString())));

        float GetPos(int i)
        {
            var pos = startPos + padding.left +
                      (i + 0.5f + centered - adjustFocus + Mathf.RoundToInt(calculate)) * size.x;
            return pos;
        }

        current.Value = remap;
        _lastTempAdjust = int.MinValue;
    }

    private void SetPosItem()
    {
        for (var i = 0; i < _currentItems.Length; i++)
        {
            _currentItems[i].RectTransform.anchorMin = new Vector2(.5f, .5f);
            _currentItems[i].RectTransform.anchorMax = new Vector2(.5f, .5f);
            _currentItems[i].RectTransform
                .SetAnchorX(startPos + padding.left + (i + 0.5f + centered - adjustFocus) * size.x);
        }
    }

    private void Focus(int index)
    {
        if(_currentItems.Length > 0)
            content.SetAnchorX(-_currentItems[index].transform.localPosition.x);
    }

    [SerializeField] private Ease ease = Ease.OutBack;

    public override void TweenTo(int index)
    {
        var pos = -_currentItems[index].transform.localPosition.x;
        var distance = Mathf.Abs(content.anchoredPosition.x - pos);
        content.DOKill();
        content.DOAnchorPosX(pos, distance / 1000 * tweenSpeed).SetDelay(delayTween)
            .OnComplete(Recenter)
            .SetEase(ease);
#if UNITY_EDITOR
        if (!Application.isPlaying)
            Focus(index);
#endif
    }

    public void TweenToCurrent()
    {
        TweenTo(current.Value);
    }

    public void Next()
    {
        if(_n != 1)
            TweenTo((remap - 1).Repeat(_currentItems.Length - 1));
    }

    public void Pre()
    {
        if(_n != 1)
            TweenTo((remap + 1).Repeat(_currentItems.Length - 1));
    }

    internal Vector2 MouseDownPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastTempAdjust = int.MaxValue;
        content.DOKill();
        MouseDownPos = eventData.position;
        _t = Time.time;
        _nextDisposable?.Dispose();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _t = Time.time;
    }

    public float snapChangeDistance = 50f;

    public void OnEndDrag(PointerEventData eventData)
    {
        _t = Time.time;
        _lastTempAdjust = int.MaxValue;
//        UpdateCenter();
        Recenter();
        float offset = eventData.position.x - MouseDownPos.x;
        if (Mathf.Abs(offset) > snapChangeDistance && Mathf.Abs(offset) < size.x / 2)
        {
            if (offset < 0)
                Pre();
            else
                Next();
        }
        else
        {
            TweenToCurrent();
        }

        if (autoNext > 0.5f)
        {
            _t = Time.time;
            _nextDisposable?.Dispose();
            _nextDisposable = Observable.EveryUpdate()
                .TakeUntilDisable(this).Subscribe(data =>
                {
                    if (Time.time - _t > autoNext)
                    {
                        _t = Time.time;
                        Pre();
                    }
                });
        }
    }

    [SerializeField] private bool populateOnStart;

    public override void Start()
    {
        base.Start();
        if (populateOnStart)
        {
            Populate();
        }
        
        
        GetComponent<ScrollRect>().onValueChanged.AddListener(t => WhenValueChanged());
    }

    protected override void WhenValueChanged(bool force = false)
    {
       onChanged.OnNext(UpdateCenter());
    }
}