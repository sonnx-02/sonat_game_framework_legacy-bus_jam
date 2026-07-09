using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollFocusScript : IndexBindingScript
{
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollAlign scrollAlign;
    [SerializeField] private float offset;
    [SerializeField] private float minMax;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var c = -1;
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).GetComponent<LayoutElement>() == null ||
                !content.GetChild(i).GetComponent<LayoutElement>().ignoreLayout)
            {
                c++;
                if (c == computedValue)
                {
                    Focus((content.GetChild(i) as RectTransform).anchoredPosition);
                }
            }
        }
    }

    public void Focus(Vector2 childAnchorPosition)
    {
        var size = content.sizeDelta;
        switch (scrollAlign)
        {
            case ScrollAlign.TopDown:
#if UNITY_EDITOR
                if(content.pivot.y < 0.995f)
                    Debug.LogError("pivot y must be 1");
#endif
                content.anchoredPosition = new Vector2(content.anchoredPosition.x,Mathf.Max(-(childAnchorPosition.y+size.y)+offset,minMax));
                break;
            case ScrollAlign.BottomUp:
                #if UNITY_EDITOR
                if(content.pivot.y > 00.01f)
                    Debug.LogError("pivot y must be zero");
                #endif
                content.anchoredPosition = new Vector2(content.anchoredPosition.x,Mathf.Min(-(childAnchorPosition.y+size.y)+offset,minMax));
                break;
            case ScrollAlign.LeftRight:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
