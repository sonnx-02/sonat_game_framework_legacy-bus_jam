using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILineBetweenScript : MonoBehaviour
{
    [SerializeField] private Vector2 adjustPos;
    
    
    [SerializeField] private RectTransform rectTransform1;
    [SerializeField] private RectTransform rectTransform2;

    private RectTransform _rt;
    public RectTransform RectTransform
    {
        get
        {
            if (_rt == null)
                _rt = GetComponent<RectTransform>();
            return _rt;
        }
    }

    [MyButton(nameof(Set))] public int test;

    public void Set()
    {
        Set(rectTransform1, rectTransform2);
    }

    public void Set(RectTransform from, RectTransform to)
    {
        Set(@from.anchoredPosition, to.anchoredPosition);
    }
    
    public void Set(Vector2 posFrom, Vector2 posTo)
    {
        RectTransform.anchoredPosition = (posFrom + posTo)/2 + adjustPos;
        var relativeVec = (posFrom - posTo);
        var distance = relativeVec.magnitude;
        RectTransform.sizeDelta = new Vector2(distance,RectTransform.sizeDelta.y);
        RectTransform.eulerAngles = new Vector3(0,0, Mathf.Atan2(relativeVec.y, relativeVec.x) * Mathf.Rad2Deg + 0);
    }
}
