using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected GameObject placeHolder = null;
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        placeHolder = GetPlaceHolder(_returnContainer, "place1");
    }

    protected GameObject GetPlaceHolder(Transform container, string n)
    {
        var p = new GameObject();
        p.name = n;
        p.transform.SetParent(container);
        LayoutElement le = p.AddComponent<LayoutElement>();
        le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;
        p.transform.SetSiblingIndex(transform.GetSiblingIndex());
        transform.SetParent(_containerDrag);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        return p;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        var transform1 = transform;
        transform1.position = eventData.position;
        placeHolder.transform.SetSiblingIndex(_returnContainer.GetClosestChildrenIndex(transform1.position,out float closest));
    }

  
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_returnContainer);
        transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeHolder);
        _dropZone.AfterDrop();
    }

    protected DropZone _dropZone;
    protected Transform _containerDrag;
    protected Transform _returnContainer;

    public void SetDragContainer(DropZone dropZone, Transform containerDrag)
    {
        _dropZone = dropZone;
        _containerDrag = containerDrag;
        _returnContainer = transform.parent;
    }
}