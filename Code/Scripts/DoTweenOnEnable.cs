using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoTweenOnEnable : MonoBehaviour
{

    [SerializeField] private Vector2 @from;
    [SerializeField] private Vector2 to;
    [SerializeField] private float time;
    [SerializeField] private Ease ease = Ease.Linear;

    private void OnEnable()
    {
        StartTween();
    }

    void OnDisable()
    {
        StopTween();
    }

    public void StartTween()
    {
        transform.localPosition = @from;
        transform.DOLocalMove(to, time).SetEase(ease).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
    }

    public void StopTween()
    {
        transform.DOKill();
    }
}
