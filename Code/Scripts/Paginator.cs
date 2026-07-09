using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Paginator : BaseFrameworkView
{
    [SerializeField] private bool tween = true;
    [SerializeField] private IntBool startIndex;
    [SerializeField] private RectTransform container;
    [SerializeField] private BaseIndexPropertySource propertyRetriever;
    [SerializeField] private PageScript[] pages;
    [SerializeField] private Button[] buttons;
    [SerializeField] private IndexBindingScriptBase[] pageBindings;
    [SerializeField] private float timeMove = 1;
    [SerializeField] private Ease ease = Ease.OutSine;
    [SerializeField] private CustomScrollRect_Swift scrollRectEventHandler;
    [SerializeField] private float snap = 0.3f;
    [SerializeField] 
    private IntReactiveProperty property;

    [SerializeField] private PaginateUnlockEquipHandler[] handlers;


    public IntReactiveProperty Property => property;
    private void OnEnable()
    {
        CheckKernelLoaded();
        if(!IsKernelLoaded())
            return;
        if (startIndex.value2)
            Set(startIndex.value,false);
        else
            SetPos(Current);
    }

    private int Current
    {
        get => property.Value;
        set => property.Value = value;
    }

    protected override void Register()
    {
        base.Register();
        for (var i = 0; i < buttons.Length; i++)
        {
            var i1 = i;
            buttons[i].onClick.AddListener(() => { TweenTo(i1); });
        }
        if (scrollRectEventHandler != null)
            scrollRectEventHandler.onScrollRectEndDrag += ScrollRectEnd;
    }

    [SerializeField] private bool presetPosition;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        property = propertyRetriever != null ? propertyRetriever.GetProperty() : new IntReactiveProperty();
        property.Subscribe(data =>
        {
            pageBindings.OnChanged(data);
            if(!gameObject.activeInHierarchy && presetPosition)
                SetPos(data);
        });
        pageBindings.OnChanged(Current);
        Set(startIndex.value,true);
        
        foreach (var handler in handlers)
            handler.Setup(property);
        SetPos(Current);
    }

    private void ScrollRectEnd(int add)
    {
        var last = Current;
        Current = -1;
        TweenTo(last+add);
    }

    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(scrollRectEventHandler.horizontalNormalizedPosition);
    }

//    protected void OnEnable()
//    {
//        if (IsKernelLoaded() && setStartIndexOnEnable.value2)
//        {
//            Current = setStartIndexOnEnable.value;
//            Set(Current);
//        }
//    }

    [MyButton(nameof(PreviousPage), nameof(NextPage))] [SerializeField]
    private int test;

    public void TweenTo(int to)
    {
        Tween(to - Current);
    }

    private void Tween(int add)
    {
        var last = Current;
        Current = Mathf.Clamp(Current + add, 0, pages.Length - 1);
        if (last != Current)
        {
            for (var index = 0; index < pages.Length; index++)
                pages[index].SetCurrent(index == Current);

            if (gameObject.activeInHierarchy && tween)
            {
                    var current = container.anchoredPosition.x;
                    var to = -pages[Current].transform.localPosition.x;
                    container.DOKill();
                    container.DOAnchorPos(new Vector3(-pages[Current].transform.localPosition.x, 0, 0),
                        Mathf.Min(Mathf.Abs(to - current) / 1080 * timeMove,timeMove)).SetEase(ease);
            }
            else
                SetPos(Current);
        }
    }
    

    private void Set(int index,bool force = false)
    {
        if (Current != index || force)
        {
            Current = index;
            SetPos(Current);
            for (var i = 0; i < pages.Length; i++)
                pages[i].SetCurrent(i == Current);
        }
    }


    //populate is for non center
    [SerializeField] private bool populate = true;
    [MyButton(nameof(Populate))]
    [SerializeField] private int testSize;
    public void Populate()
    {
        if (populate)
        {
            var rt = RootView.rootView == null ? FindObjectOfType<ScreenRoot>().GetComponent<RectTransform>() :  ScreenRoot.RtCanvas;
            if (rt != null)
            {
                var startSize = -(pages.Length / 2f - 0.5f) * rt.sizeDelta.x;
                scrollRectEventHandler.content.sizeDelta = new Vector2(rt.sizeDelta.x * pages.Length,rt.sizeDelta.y);
                for (var i = 0; i < pages.Length; i++)
                {
                    pages[i].RectTransform.anchorMin = new Vector2(0.5f,0.5f);
                    pages[i].RectTransform.anchorMax = new Vector2(0.5f,0.5f);
                    pages[i].RectTransform.pivot = new Vector2(0.5f,0.5f);
                    pages[i].RectTransform.sizeDelta = rt.sizeDelta;
                    pages[i].RectTransform.anchoredPosition = new Vector2(startSize + rt.sizeDelta.x * i,0);
                }
            }
        }
    }

    public void SetCurrentPos()
    {
        SetPos(Current);
    }
    
    private void SetPos(int index)
    {

        Populate();

        //      container.localPosition = new Vector3(-pages[index].transform.localPosition.x, 0, 0);
        container.anchoredPosition = new Vector3(-pages[index].RectTransform.localPosition.x, 0, 0);
    }

    public void NextPage()
    {
        Tween(1);
    }

    public void PreviousPage()
    {
        Tween(-1);
    }
}