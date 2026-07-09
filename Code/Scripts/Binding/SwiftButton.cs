using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwiftButton : BaseFrameworkView, IBeginDragHandler,
    IEndDragHandler, IDragHandler,IPointerClickHandler,IPointerDownHandler
{
    [SerializeField] private BaseBaseActionScript[] onClick;
    [SerializeField] private BaseBaseActionScript[] onSwiftRight;
    [SerializeField] private BaseBaseActionScript[] onSwiftLeft;
    
    [SerializeField] private UnityEvent onClick2;
    [SerializeField] private UnityEvent onSwiftRight2;
    [SerializeField] private UnityEvent onSwiftLeft2;

    [SerializeField] private bool natural = true;
    [SerializeField] protected string customShortcutName;
    public Action ActionOnClick { get; set; }
    public Action ActionOnSwiftLeft { get; set; }
    public Action ActionOnSwiftRight { get; set; }
    
    private Vector2 _mouseDownPos;
    public float snapChangeDistance = 50f;

    private bool _click;
    private bool _beginDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _mouseDownPos = eventData.position;
        _click = false;
        _beginDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float offset = eventData.position.x - _mouseDownPos.x;
        if (_beginDrag && Check(offset))
            _beginDrag = false;
     
    }

    private bool Check(float offset)
    {
        if (Mathf.Abs(offset) > snapChangeDistance)
        {
            
            if (offset < 0)
            {
              if(natural)
                  Right();
              else
                  Left();
            }
            else
            {
                if(natural)
                    Left();
                else
                    Right();
            }

            return true;
        }

        return false;
    }

    private void Left()
    {
//        Debug.Log("swift left");
        onSwiftLeft2.Invoke();
        foreach (var baseActionScript in onSwiftLeft)
            baseActionScript.StartAction(0);
        ActionOnSwiftLeft?.Invoke();
    }

    private void Right()
    {
        onSwiftRight2.Invoke();
        foreach (var baseActionScript in onSwiftRight)
            baseActionScript.StartAction(0);
        ActionOnSwiftRight?.Invoke();

   //     Debug.Log("swift right");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_beginDrag)
        {
            float offset = eventData.position.x - _mouseDownPos.x;
            if (Check(offset))
                _beginDrag = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_click)
        {
            onClick2.Invoke();
            foreach (var baseActionScript in onClick)
                baseActionScript.StartAction(0);
            ActionOnClick?.Invoke();
            
            if(!string.IsNullOrEmpty(customShortcutName))
                BaseGameController.LogShortcut(customShortcutName);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _click = true;
    }
}