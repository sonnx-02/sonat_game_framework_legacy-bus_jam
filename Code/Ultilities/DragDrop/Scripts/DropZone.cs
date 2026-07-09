using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DropZone2 targetZone;
    protected void Start()
    {
        foreach (var componentsInChild in GetComponentsInChildren<Draggable>())
        {
            componentsInChild.SetDragContainer(this,containerDrag);
        }
        foreach (var componentsInChild in GetComponentsInChildren<DraggableToTargetZone2>())
        {
            componentsInChild.targetZone = targetZone;
        }
    }

    [SerializeField] private Transform containerDrag;
    public virtual void OnPointerEnter(PointerEventData eventData) {

        if (eventData.pointerDrag == null)
            return;
    }

    public virtual void OnPointerExit(PointerEventData eventData) {

        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
    }

    public virtual void OnDrop(PointerEventData eventData) {
        Debug.Log(eventData.pointerDrag.name + " dropped on " + gameObject.name);
    }
    
    public void AfterDrop()
    {
        afterDrop?.Invoke();
    }
    
    public Action afterDrop;

    public Action<DraggableEvent> dropDraggableEvent;
    public Action<PairGameObject> dropEvent;
    public Action<DraggableEvent> dragDraggableEvent;
    public Action<PairGameObject> dragEvent;
   
    public void Drop(DraggableEvent drop)
    {
        dropDraggableEvent?.Invoke(drop);
        dropEvent?.Invoke(new PairGameObject(drop.Draggable.gameObject,transform.GetChild(drop.AtIndex).gameObject));
    }
   
    public void Drag(DraggableEvent drag)
    {
        dragDraggableEvent?.Invoke(drag);
        dragEvent?.Invoke(new PairGameObject(drag.Draggable.gameObject,transform.GetChild(drag.AtIndex).gameObject));
    }

   
    public class DraggableEvent
    {
        public Draggable Draggable;
        public int AtIndex;

        public DraggableEvent(Draggable draggable, int atIndex)
        {
            Draggable = draggable;
            AtIndex = atIndex;
        }
    }

    
}
