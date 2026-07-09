using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggableToTargetZone2 : Draggable
{
    public DropZone2 targetZone { get; set; }
    protected GameObject placeHolder2 = null;

    private int _startIndex;
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _startIndex = transform.GetSiblingIndex();
        base.OnBeginDrag(eventData);
        placeHolder2 = GetPlaceHolder(targetZone.transform,"place2");
        placeHolder2.transform.SetAsLastSibling();
        targetZone.Calculate();
    }
    
    public override void OnDrag(PointerEventData eventData)
    {
        //  base.OnDrag(eventData);
        transform.position = eventData.position;
        int newSiblingIndex = targetZone.transform.GetClosestChildrenIndex(eventData.position,out float closest);
        if (closest < targetZone.minDistance && newSiblingIndex >= 0)
        {
            placeHolder2.transform.SetSiblingIndex(newSiblingIndex);
            targetZone.Drag(new DropZone2.DraggableEvent(this,newSiblingIndex));
        }
        else
            placeHolder2.transform.SetAsLastSibling();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        int newSiblingIndex = targetZone.transform.GetClosestChildrenIndex(eventData.position,out float closest);
        if (closest < targetZone.minDistance&& newSiblingIndex >= 0)
        {
            placeHolder2.transform.SetSiblingIndex(newSiblingIndex);
            targetZone.Drop(new DropZone2.DraggableEvent(this,newSiblingIndex));
        }
        else
            placeHolder2.transform.SetAsLastSibling();
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(_returnContainer);
        transform.SetSiblingIndex(_startIndex);
        Destroy(placeHolder);
        Destroy(placeHolder2);
    }
}