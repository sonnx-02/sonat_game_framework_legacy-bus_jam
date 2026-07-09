using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RxTurnOffScrollRectThenFocus : BaseRxAnimator
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float delayToEnable = 1f;
    [SerializeField] private float space;
    [SerializeField] private float startOffset;
    public int focusIn;
    private RectTransform Content => scrollRect.content;

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        scrollRect.enabled = false;
        yield return new WaitForSecondsRealtime(delayToEnable);
        scrollRect.enabled = true;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();

    #if UNITY_EDITOR
    private void Reset()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
    #endif
}
