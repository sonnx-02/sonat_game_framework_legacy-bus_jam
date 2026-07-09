using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IndexBindingScriptTweenPosition : IndexBindingScript
{
    [SerializeField] private float time = 0.3f;
    [SerializeField] private Ease ease;
    [SerializeField] private Transform[] targets;
    [SerializeField] private bool local;
    [SerializeField] private BoolBool xy;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        transform.DOKill();
        if(time > 0)
            if (local)
            {
                if(xy.value && xy.value2)
                    transform.DOLocalMove(targets[computedValue].localPosition, time).SetEase(ease);
                if (!xy.value && xy.value2)
                    transform.DOLocalMove(transform.localPosition.ChangeY(targets[computedValue].localPosition.y), time).SetEase(ease);
                if (xy.value && !xy.value2)
                    transform.DOLocalMove(transform.localPosition.ChangeX(targets[computedValue].localPosition.x), time).SetEase(ease);
            }
            else
            {
                if(xy.value && xy.value2)
                    transform.DOMove(targets[computedValue].position, time).SetEase(ease);
                if (!xy.value && xy.value2)
                    transform.DOMove(transform.position.ChangeY(targets[computedValue].position.y), time).SetEase(ease);
                if (xy.value && !xy.value2)
                    transform.DOMove(transform.position.ChangeX(targets[computedValue].position.x), time).SetEase(ease);
            }
        else
            Set(computedValue);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        0.1f.Timer(() => Set(0));
       
    }

    private void Set(int i)
    {
        if (local)
        {
            if(xy.value && xy.value2)
                transform.localPosition = targets[i].localPosition;
            if (!xy.value && xy.value2)
                transform.localPosition = transform.localPosition.ChangeY(targets[i].localPosition.y);
            if (xy.value && !xy.value2)
                transform.localPosition = transform.localPosition.ChangeX(targets[i].localPosition.x);
        }
        else
        {
            if(xy.value && xy.value2)
                transform.position = targets[i].position;
            if (!xy.value && xy.value2)
                transform.position = transform.position.ChangeY(targets[i].position.y);
            if (xy.value && !xy.value2)
                transform.position = transform.position.ChangeX(targets[i].position.x);
        }
    }
}
