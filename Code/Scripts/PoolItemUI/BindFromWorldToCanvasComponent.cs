using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindFromWorldToCanvasComponent : PoolItem
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 offset;
    [SerializeField] private IndexBindingScriptBase[] indexBindingScripts;
    [SerializeField] private BaseRxAnimator[] anims;
    [SerializeField] private float returnAfter;
    [SerializeField] private Transform target;

    public override void OnRent()
    {
        base.OnRent();
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_In();
        if (returnAfter > 0.1f)
            returnAfter.Timer(Return);
    }

    public override void LogCustom()
    {
        base.LogCustom();
        UpdatePosFromTarget();
    }

    public void UpdatePosFromTarget()
    {
        UpdatePos(target.position);
    }


    public void UpdatePos(Vector3 worldPosition)
    {
        rectTransform.anchoredPosition = FrameworkExtensions.WorldToPositionInCanvas(worldPosition) + offset;
    }

    public void SetIndex(int index)
    {
        indexBindingScripts.OnChanged(index);
    }
} 
