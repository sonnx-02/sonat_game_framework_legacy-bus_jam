using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using UniRx;

// ReSharper disable ArrangeTypeMemberModifiers


public enum ScrollDirection
{
    Default,
    Horizon,
    Vertical
}

[RequireComponent(typeof(ScrollRect))]
public class Snap2DScrollRect : BaseFrameworkView, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool viewFirst;
    public Transform paginationContainer;
    public ToggleScript paginationPrefab;
    public float snapChangeDistance = 50f;
    public float scrollDirectionDistance = 10f;

    private readonly List<Transform> Pages = new List<Transform>();
    private readonly List<ToggleScript> Paginations = new List<ToggleScript>();

    public int startAtPage;
    [SerializeField] private bool infinitive;

    public IntReactiveProperty remapValue = new IntReactiveProperty();
    public IntReactiveProperty currentPage = new IntReactiveProperty();


//    public ScrollDirection scrollDirection;

    public ScrollRect scrollRect;

    public Button btnNext;
    public Button btnPre;

    public override void Start()
    {
        base.Start();
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        scrollRect.inertia = false;
//
//
        if (viewFirst)
            Init();
//        if (btnNext != null)
//            btnNext.onClick.AddListener(() => CurrentPage++);
//        if (btnPre != null)
//            btnPre.onClick.AddListener(() => CurrentPage--);

        currentPage.Subscribe(OnPageChanged);
    }

    private void OnPageChanged(int value)
    {
        if (!infinitive)
        {
            if (value < 0 && value >= _children.Length)
            {
                currentPage.Value = Mathf.Clamp(value, 0, _children.Length);
            }
            else
            {
                for (int i = 0; i < Paginations.Count; i++)
                    Paginations[i].OnChanged(i == value);
                if (btnNext != null)
                    btnNext.gameObject.SetActive(false);
                if (btnPre != null)
                    btnPre.gameObject.SetActive(false);
                if (btnNext != null && value < _children.Length - 1)
                    btnNext.gameObject.SetActive(true);
                if (btnPre != null && value > 0)
                    btnPre.gameObject.SetActive(true);

                MoveToPage(RealCurrentPage);
            }
        }
        else
        {
            if (value > 2 && remapValue.Value >= 1)
            {
                ChangeRemap(-1);
                currentPage.Value = value-1;
            }
            else if (value <= 0)
            {
                ChangeRemap(1);
                currentPage.Value = 1;
            }
            else if (value >= _children.Length - 1)
            {
                ChangeRemap(-1);
                currentPage.Value = _children.Length - 2;
            }
            else
            {
                for (int i = 0; i < Paginations.Count; i++)
                    Paginations[i].OnChanged(i == value);
                if (btnNext != null)
                    btnNext.gameObject.SetActive(false);
                if (btnPre != null)
                    btnPre.gameObject.SetActive(false);
                if (btnNext != null && value < _children.Length - 1)
                    btnNext.gameObject.SetActive(true);
                if (btnPre != null && value > 0)
                    btnPre.gameObject.SetActive(true);
                MoveToPage(RealCurrentPage);
            }
        }

//        math
        realCurrentPage = (currentPage.Value - remapValue.Value).Repeat(4);
    }


//    1 1 4=> 0
//    1 2 4=> 4
//    1 3 4=> 3
//    2 0 4 => 2
//    3 0 4 => 3

    public int realCurrentPage;
    public int RealCurrentPage => (currentPage.Value - remapValue.Value).Repeat(4);

    public override void LogCustom()
    {
        base.LogCustom();
        scrollRect.content.SetAnchorX(-_children[realCurrentPage].localPosition.x);
    }

    private void ChangeRemap(int value)
    {
        remapValue.Value = (remapValue.Value + value) % _children.Length;
        var last = Mathf.Abs(_children.Length - remapValue.Value) % _children.Length;
        if (value > 0)
        {
            _children[last].SetAsFirstSibling();
        }

        if (value < 0)
        {
            _children[(_children.Length - 1 - remapValue.Value).Repeat(4)].SetAsLastSibling();
        }
       // scrollRect.content.SetAnchorX(-_children[realCurrentPage].localPosition.x);
    }

    public void Clear(bool deleteChildren = false)
    {
        if (scrollRect == null)
            scrollRect = this.GetComponent<ScrollRect>();

        if (deleteChildren)
        {
            scrollRect.content.DestroyAllChildren();
            paginationContainer.DestroyAllChildren();
        }
        else
            Init();

        Paginations.Clear();
        Pages.Clear();
    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            Init();
//        }
//
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            CurrentPage--;
//        }
//
//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            CurrentPage++;
//        }
//    }


    private float _distance;

    public void Init()
    {
        if (paginationContainer == scrollRect.content)
        {
            Debug.LogError("unvalid", gameObject);
            return;
        }

        _children = GetChildren().ToArray();
        DuongExtensions.ExecuteInNextFrame(
            () => _distance = _children[1].localPosition.x - _children[0].localPosition.x);

        remapValue.Value = 0;
        currentPage.Value = 0;
//        SetPage0();
        paginationContainer.DestroyAllChildren();
        Paginations.Clear();
        Pages.Clear();

        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        bool mutilPage = scrollRect.content.ChildCountActive() > 1;
        if (mutilPage)
        {
            paginationContainer.gameObject.SetActive(true);
            for (int i = 0; i < scrollRect.content.ChildCountActive(); i++)
            {
                Pages.Add(scrollRect.content.GetActiveChild(i));
                if (Pages[i].GetComponent<VerticalScrollRect>() != null)
                {
                    Pages[i].GetComponent<VerticalScrollRect>().Snap2D = this;
                    Pages[i].GetComponent<VerticalScrollRect>().Active = true;
                }

                var pagination = Instantiate(paginationPrefab, paginationContainer, false);
                Paginations.Add(pagination);
            }
        }
        else
        {
            paginationContainer.gameObject.SetActive(false);
            for (int i = 0; i < scrollRect.content.ChildCountActive(); i++)
            {
                Pages.Add(scrollRect.content.GetActiveChild(i));
                if (Pages[i].GetComponent<VerticalScrollRect>() != null)
                {
                    Pages[i].GetComponent<VerticalScrollRect>().Snap2D = this;
                    Pages[i].GetComponent<VerticalScrollRect>().Active = false;
                }

                var pagination = Instantiate(paginationPrefab, paginationContainer, false);
                Paginations.Add(pagination);
            }
        }

        currentPage.Value = startAtPage >= 0 ? startAtPage : currentPage.Value;
//        MoveToPage(realCurrentPage);
        startAtPage = -1;
    }

    [SerializeField] [MyButton(nameof(SetPage0), nameof(Pre), nameof(Next))]
    private int testFunc;

    public void SetPage0()
    {
        currentPage.Value = 0;
        MoveToPage(RealCurrentPage, false);
    }

    public void Next()
    {
        currentPage.Value++;
    }

    public void Pre()
    {
        currentPage.Value--;
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        MoveToPage(RealCurrentPage);
    }

    public void MoveToPage(int index, bool tween = true)
    {
        var content = scrollRect.content;

//        var grid = content.GetComponent<GridLayoutGroup>();
//        if (grid != null)
//        {
//            if (index >= 0 && index < content.ChildCountActive())
//            {
//                Transform transform1;
//                (transform1 = content.transform).DOKill();
//                Vector3 destination = transform1.localPosition;
//                destination.x = -(grid.padding.left + index * grid.spacing.x + (index + 0.5f) * grid.cellSize.x);
//                if (tween && gameObject.activeInHierarchy)
//                {
//                    float time = Mathf.Abs(destination.x - content.transform.localPosition.x) /
//                                 grid.cellSize.x * 0.35f;
//                    content.transform.DOLocalMove(destination, time, true).SetEase(Ease.Linear);
//                }
//                else
//                {
//                    content.transform.localPosition = destination;
//                }
//            }
//        }

#if UNITY_EDITOR
        if (content.anchorMin.x < 0.499f
            || content.anchorMin.x > 0.5001f
            || content.anchorMax.x < 0.499f
            || content.anchorMax.x > 0.5001f
            || content.pivot.x < 0.499f
            || content.pivot.x > 0.5001f
        )
        {
            Debug.LogError("not valid");
        }

#endif

        var horizonal = content.GetComponent<HorizontalLayoutGroup>();
        if (horizonal != null)
        {
#if UNITY_EDITOR
            if (!(horizonal.childAlignment == TextAnchor.MiddleCenter
                  || horizonal.childAlignment == TextAnchor.UpperCenter
                  || horizonal.childAlignment == TextAnchor.LowerCenter))
            {
                Debug.LogError("not valid");
            }
#endif

            if (index >= 0 && index < _children.Length)
            {
                content.DOKill();
                var child = _children[index];
             
                if (tween && gameObject.activeInHierarchy)
                {
//                    scrollRect.content.SetAnchorX(-_children[realCurrentPage].localPosition.x);
                    0.25f.Timer(() =>
                    {
                        var destinationX = -child.localPosition.x;
                        float time = Mathf.Abs(destinationX - content.anchoredPosition.x) / content.sizeDelta.x * timeMoveEach;
                        content.DOAnchorPosX(destinationX, time, true).SetEase(ease);
                    });
                }
                else
                {
                    var destinationX = -child.localPosition.x;
                    content.anchoredPosition = new Vector2(destinationX, content.anchoredPosition.y);
                }
            }
        }
    }

    [SerializeField] private float timeMoveEach = 1;
    [SerializeField] private Ease ease = Ease.Linear;

    private RectTransform[] _children;

    private IEnumerable<RectTransform> GetChildren()
    {
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            if (scrollRect.content.GetChild(i).gameObject.activeSelf &&
                (scrollRect.content.GetChild(i).GetComponent<LayoutElement>() == null ||
                 !scrollRect.content.GetChild(i).GetComponent<LayoutElement>().ignoreLayout))
                yield return scrollRect.content.GetChild(i).transform as RectTransform;
        }
    }

    internal Vector2 MouseDownPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        MouseDownPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float offset = eventData.position.x - MouseDownPos.x;
        if (Mathf.Abs(offset) > snapChangeDistance)
        {
            if (offset < 0)
                currentPage.Value++;
            else
                currentPage.Value--;
        }
        else
            MoveToPage(RealCurrentPage);
    }
}