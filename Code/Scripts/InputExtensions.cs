using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputExtensions
{
    public static bool IsCastingOnUi()
    {
        PointerEventData pe = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);
        foreach (var raycastResult in hits)
        {
            return true;
        }

        return false;
    }
}