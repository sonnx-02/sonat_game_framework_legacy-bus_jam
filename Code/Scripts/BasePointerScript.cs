using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

public class BasePointerScript : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerClickHandler
{
    public BoolBindingReactiveProperty onHold;
    public BoolBindingReactiveProperty onHoldValid;
    private bool _valid = true;
    public Action OnClick;
    protected virtual void Start()
    {
        onHold.Init();
        onHoldValid.Init();
        onHold.Subscribe(data => onHoldValid.Value = onHold.Value && _valid);
    }

    public virtual void SetValid(bool val)
    {
        _valid = val;
        onHoldValid.Value = onHold.Value && _valid;
    }
    

    protected virtual void OnDisable()
    {
        onHold.Value = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        onHold.Value = false;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        onHold.Value = true;
        OnClick?.Invoke();
    }

    public void ClearOnClick()
    {
        OnClick = null;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onHold.Value = false;
    }
}