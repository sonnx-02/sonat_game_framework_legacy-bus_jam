using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class VerticalScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Snap2DScrollRect Snap2D;

    public ScrollDirection _scrollState;
    private ScrollDirection scrollState
    {
        get { return _scrollState; }
        set
        {
            _scrollState = value;
            if (value == ScrollDirection.Default)
            {
                myScroll.vertical = true;
            }
            else if (value == ScrollDirection.Vertical)
            {
                myScroll.vertical = true;
            }
            else if (value == ScrollDirection.Horizon)
            {
                myScroll.vertical = false;
            }
        }
    }
    ScrollRect myScroll;

    internal bool Active;

    void Awake()
    {
        myScroll = gameObject.GetComponent<ScrollRect>();
    }

    private Vector2 mouseDownPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Active)
        {
            mouseDownPos = eventData.position;
            scrollState = ScrollDirection.Default;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Active)
        {
            if (Snap2D != null)
            {
                var offset = eventData.position - mouseDownPos;
                if (scrollState == ScrollDirection.Default && Mathf.Abs(offset.x) > Mathf.Abs(offset.y) && Mathf.Abs(offset.x) > Snap2D.scrollDirectionDistance)
                {
                    Snap2D.scrollRect.OnBeginDrag(eventData);
                    Snap2D.MouseDownPos = mouseDownPos;
                    scrollState = ScrollDirection.Horizon;
                }
                else if (scrollState == ScrollDirection.Default && Mathf.Abs(offset.y) > Mathf.Abs(offset.x) && Mathf.Abs(offset.y) > Snap2D.scrollDirectionDistance)
                {
                    scrollState = ScrollDirection.Vertical;
                }

                if (scrollState == ScrollDirection.Horizon)
                    Snap2D.scrollRect.OnDrag(eventData);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Active)
        {
            if (scrollState == ScrollDirection.Horizon)
                Snap2D.OnEndDrag(eventData);

            scrollState = ScrollDirection.Default;
        }
    }
}

