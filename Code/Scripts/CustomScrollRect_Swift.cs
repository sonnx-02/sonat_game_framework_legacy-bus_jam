using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect_Swift : ScrollRect
{
    [SerializeField] private bool disable;
    public Action<int> onScrollRectEndDrag;
    public float threshold = 50f;
    private Vector2 _mouseDownPos;

    
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if(disable) return;
        base.OnInitializePotentialDrag(eventData);
    }
    
    
    #if UNITY_EDITOR
    private int _count;
    #endif
    public override void OnBeginDrag(PointerEventData eventData) 
    {
        if (disable)
        {   
#if UNITY_EDITOR
            _count++;
            if(_count == 1)
                Debug.LogError("Scroll Rect currently disable, see in debug mode");
#endif
            return;
        }
        
           
        base.OnBeginDrag(eventData); 
        _mouseDownPos = eventData.position;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if(disable) return;
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if(disable) return;
        base.OnEndDrag(eventData);
        float offset = eventData.position.x - _mouseDownPos.x;
        if (Mathf.Abs(offset) > threshold)
        {
            if (offset < 0)
                onScrollRectEndDrag.Invoke(1);
            else
                onScrollRectEndDrag.Invoke(-1);
        }
        else
            onScrollRectEndDrag.Invoke(0);
    }
}
